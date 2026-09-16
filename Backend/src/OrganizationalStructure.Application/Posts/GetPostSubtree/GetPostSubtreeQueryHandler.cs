using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Constants;

namespace OrganizationalStructure.Application.Posts.GetPostSubtree;

/// <summary>
/// پردازش‌گر پرس‌وجوی زیرشاخه چندسطحی پست.
/// </summary>
/// <remarks>
/// پست‌های سازمان در یک کوئری خوانده و درخت در حافظه ساخته می‌شود
/// (برای اندازه‌های متعارف ساختار سازمانی مناسب است).
/// </remarks>
public sealed class GetPostSubtreeQueryHandler : IRequestHandler<GetPostSubtreeQuery, Result<PostTreeDto>>
{
    private const int DefaultMaxDepth = 20;

    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostSubtreeQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PostTreeDto>> Handle(
        GetPostSubtreeQuery request,
        CancellationToken cancellationToken)
    {
        var root = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (root is null)
        {
            return Result<PostTreeDto>.Failure(PostErrors.NotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(root.OrganizationId))
        {
            return Result<PostTreeDto>.Failure(AccessErrors.Forbidden());
        }

        var maxDepth = request.MaxDepth ?? DefaultMaxDepth;

        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p => p.OrganizationId == root.OrganizationId)
            .Select(p => new { p.Id, p.ParentId, p.Code, p.Title, p.IsActive })
            .ToListAsync(cancellationToken);

        var signingPostIds = await (
            from a in _db.AuthorityAssignments.AsNoTracking()
            join u in _db.Authorities.AsNoTracking() on a.AuthorityId equals u.Id
            where a.OrganizationId == root.OrganizationId
                && a.IsActive && a.EndDate == null
                && u.Code == AuthorityCodes.SigningAuthority
            select a.PostId)
            .ToListAsync(cancellationToken);
        var signingSet = signingPostIds.ToHashSet();

        var childrenByParent = posts
            .Where(p => p.ParentId.HasValue)
            .GroupBy(p => p.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Code).ToList());

        PostTreeDto BuildTree(Guid id, int depth)
        {
            var current = posts.First(p => p.Id == id);
            var node = new PostTreeDto
            {
                Id = current.Id,
                Code = current.Code,
                Title = current.Title,
                HasSigningAuthority = signingSet.Contains(current.Id),
                IsActive = current.IsActive
            };

            if (depth >= maxDepth || !childrenByParent.TryGetValue(id, out var children))
            {
                return node;
            }

            foreach (var child in children)
            {
                node.Children.Add(BuildTree(child.Id, depth + 1));
            }

            return node;
        }

        return Result<PostTreeDto>.Success(BuildTree(root.Id, 0));
    }
}