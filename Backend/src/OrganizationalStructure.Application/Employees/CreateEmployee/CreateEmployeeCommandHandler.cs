using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.ValueObjects;

namespace OrganizationalStructure.Application.Employees.CreateEmployee;

/// <summary>
/// پردازش‌گر دستور ثبت پرسنل.
/// </summary>
public sealed class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public CreateEmployeeCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;
        var code = request.PersonnelCode.Trim();

        var duplicate = await _db.Employees.AnyAsync(
            e => e.PersonnelCode == code,
            cancellationToken);

        if (duplicate)
        {
            return Result<Guid>.Failure(EmployeeErrors.DuplicatePersonnelCode(code));
        }

        HerasatServiceRecord? serviceRecord = request.ServiceYears.HasValue
            ? new HerasatServiceRecord(request.ServiceYears.Value, request.ServiceMonths!.Value)
            : null;

        var employee = Employee.Create(
            Guid.NewGuid(),
            tenantId,
            code,
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.Mobile,
            request.UserId,
            _clock.UtcNow,
            request.BirthDate,
            serviceRecord,
            request.PezhvakMobile);

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(employee.Id);
    }
}