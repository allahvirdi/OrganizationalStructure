using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Responsibilities.AssignResponsibility;

/// <summary>
/// پردازش‌گر دستور انتساب مسئولیت به پست.
/// </summary>
public sealed class AssignResponsibilityCommandHandler
    : IRequestHandler<AssignResponsibilityCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AssignResponsibilityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(
        AssignResponsibilityCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.ResponsibilityCode.Trim();

        var responsibility = await _db.Responsibilities
            .Include(r => r.Assignments)
            .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);

        if (responsibility is null)
        {
            return Result<Guid>.Failure(ResponsibilityErrors.NotFoundByCode(code));
        }

        if (!responsibility.IsActive)
        {
            return Result<Guid>.Failure(ResponsibilityErrors.Inactive(code));
        }

        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<Guid>.Failure(ResponsibilityErrors.PostNotFound(request.PostId));
        }

        Guid assignmentId;
        try
        {
            var assignment = responsibility.AssignToPost(
                post.OrganizationId,
                post.Id,
                request.StartDate,
                request.EndDate,
                _clock.UtcNow);

            _db.ResponsibilityAssignments.Add(assignment);
            assignmentId = assignment.Id;
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Failure(ResponsibilityErrors.AssignmentConflict(ex.Message));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(assignmentId);
    }
}