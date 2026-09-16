using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.MovePost;

/// <summary>
/// پردازش‌گر دستور جابجایی پست در درخت سازمان.
/// </summary>
/// <remarks>
/// کنترل‌های هم‌سازمانی بودن و نبود چرخه در اینجا انجام می‌شود،
/// چون نیازمند دسترسی به درخت (پایگاه داده) است.
/// </remarks>
public sealed class MovePostCommandHandler : IRequestHandler<MovePostCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public MovePostCommandHandler(IAppDbContext db, IClock clock, ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(MovePostCommand request, CancellationToken cancellationToken)
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

        if (request.NewParentId.HasValue)
        {
            var parent = await _db.Posts.FirstOrDefaultAsync(
                p => p.Id == request.NewParentId.Value,
                cancellationToken);

            if (parent is null)
            {
                return Result.Failure(PostErrors.ParentNotFound(request.NewParentId.Value));
            }

            if (parent.OrganizationId != post.OrganizationId)
            {
                return Result.Failure(PostErrors.CrossOrganizationMove());
            }

            var createsCycle = await CreatesCycleAsync(post.Id, parent.Id, cancellationToken);
            if (createsCycle)
            {
                return Result.Failure(PostErrors.CycleDetected());
            }
        }

        post.ChangeParent(request.NewParentId, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// بررسی می‌کند که آیا قرار دادن پست زیر والد جدید باعث چرخه می‌شود یا خیر.
    /// </summary>
    private async Task<bool> CreatesCycleAsync(
        Guid postId,
        Guid newParentId,
        CancellationToken cancellationToken)
    {
        var currentId = (Guid?)newParentId;

        while (currentId.HasValue)
        {
            if (currentId.Value == postId)
            {
                return true;
            }

            currentId = await _db.Posts
                .Where(p => p.Id == currentId.Value)
                .Select(p => p.ParentId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return false;
    }
}