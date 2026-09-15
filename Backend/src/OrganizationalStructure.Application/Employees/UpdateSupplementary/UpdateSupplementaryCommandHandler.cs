using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.ValueObjects;

namespace OrganizationalStructure.Application.Employees.UpdateSupplementary;

/// <summary>
/// پردازش‌گر دستور ویرایش اطلاعات تکمیلی پرسنل.
/// </summary>
public sealed class UpdateSupplementaryCommandHandler : IRequestHandler<UpdateSupplementaryCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public UpdateSupplementaryCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateSupplementaryCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(
            e => e.Id == request.EmployeeId,
            cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        HerasatServiceRecord? serviceRecord = request.ServiceYears.HasValue
            ? new HerasatServiceRecord(request.ServiceYears.Value, request.ServiceMonths!.Value)
            : null;

        employee.UpdateSupplementaryInfo(
            request.BirthDate,
            serviceRecord,
            request.PezhvakMobile,
            _clock.UtcNow);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}