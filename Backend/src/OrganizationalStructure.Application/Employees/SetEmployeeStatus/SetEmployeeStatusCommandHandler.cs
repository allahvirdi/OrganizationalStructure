using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.SetEmployeeStatus;

/// <summary>
/// پردازش‌گر دستور تعیین وضعیت فعال/غیرفعال پرسنل.
/// </summary>
/// <remarks>
/// غیرفعال‌سازی به معنی حذف اطلاعات نیست (DEC-022).
/// </remarks>
public sealed class SetEmployeeStatusCommandHandler : IRequestHandler<SetEmployeeStatusCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SetEmployeeStatusCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(SetEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(
            e => e.Id == request.EmployeeId,
            cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        if (request.IsActive)
        {
            employee.Activate(_clock.UtcNow);
        }
        else
        {
            employee.Deactivate(_clock.UtcNow);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}