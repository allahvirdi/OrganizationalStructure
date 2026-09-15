using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.UpdateEmployee;

/// <summary>
/// پردازش‌گر دستور ویرایش اطلاعات پرسنلی.
/// </summary>
public sealed class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public UpdateEmployeeCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(
            e => e.Id == request.EmployeeId,
            cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        employee.UpdatePersonalInfo(
            request.FirstName,
            request.LastName,
            request.NationalCode,
            request.Mobile,
            _clock.UtcNow);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}