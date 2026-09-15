using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.EndAssignment;

/// <summary>
/// پردازش‌گر دستور پایان انتساب پرسنل به پست.
/// </summary>
public sealed class EndAssignmentCommandHandler : IRequestHandler<EndAssignmentCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public EndAssignmentCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(EndAssignmentCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees
            .Include(e => e.Assignments)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        try
        {
            employee.EndAssignment(request.PostId, request.EndDate, _clock.UtcNow);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(EmployeeErrors.NoActiveAssignment());
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}