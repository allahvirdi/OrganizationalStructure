using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.SetPostStatus;

/// <summary>
/// پردازش‌گر دستور تعیین وضعیت فعال/غیرفعال پست.
/// </summary>
public sealed class SetPostStatusCommandHandler : IRequestHandler<SetPostStatusCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SetPostStatusCommandHandler(IAppDbContext db, IClock clock, ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(SetPostStatusCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(
            p => p.Id == request.PostId,
            cancellationToken);

        if (post is null)
        {
            return Result.Failure(PostErrors.NotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(post.OrganizationId))
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        if (request.IsActive)
        {
            post.Activate(_clock.UtcNow);
        }
        else
        {
            post.Deactivate(_clock.UtcNow);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}