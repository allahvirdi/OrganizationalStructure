using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.AssignPost;

/// <summary>
/// پردازش‌گر دستور انتساب پرسنل به پست.
/// </summary>
public sealed class AssignPostCommandHandler : IRequestHandler<AssignPostCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AssignPostCommandHandler(IAppDbContext db, IClock clock, ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(AssignPostCommand request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees
            .Include(e => e.Assignments)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(request.EmployeeId));
        }

        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result.Failure(EmployeeErrors.PostNotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(post.OrganizationId))
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        // پست باید متعلق به سازمان مالک پرسنل باشد (ADR-004: هر سازمان درخت پست مستقل خود را دارد).
        if (post.OrganizationId != employee.OrganizationId)
        {
            return Result.Failure(EmployeeErrors.PostOrganizationMismatch());
        }

        try
        {
            var assignment = employee.AssignToPost(
                request.PostId,
                request.FromDate,
                request.ToDate,
                request.IsPrimary,
                _clock.UtcNow);

            _db.Assignments.Add(assignment);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(EmployeeErrors.AssignmentConflict(ex.Message));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}