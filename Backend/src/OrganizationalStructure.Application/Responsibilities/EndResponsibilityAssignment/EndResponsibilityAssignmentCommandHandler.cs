using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Responsibilities.EndResponsibilityAssignment;

/// <summary>
/// پردازش‌گر دستور پایان انتساب مسئولیت به پست.
/// </summary>
public sealed class EndResponsibilityAssignmentCommandHandler
    : IRequestHandler<EndResponsibilityAssignmentCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public EndResponsibilityAssignmentCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        EndResponsibilityAssignmentCommand request,
        CancellationToken cancellationToken)
    {
        var assignment = await _db.ResponsibilityAssignments.FirstOrDefaultAsync(
            a => a.Id == request.AssignmentId,
            cancellationToken);

        if (assignment is null)
        {
            return Result.Failure(ResponsibilityErrors.AssignmentNotFound(request.AssignmentId));
        }

        var responsibility = await _db.Responsibilities
            .Include(r => r.Assignments)
            .FirstOrDefaultAsync(r => r.Id == assignment.ResponsibilityId, cancellationToken);

        if (responsibility is null)
        {
            return Result.Failure(ResponsibilityErrors.NotFound(assignment.ResponsibilityId));
        }

        try
        {
            responsibility.EndAssignment(request.AssignmentId, request.EndDate, _clock.UtcNow);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(ResponsibilityErrors.AssignmentNotFound(request.AssignmentId));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}