using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.SearchPosts;

/// <summary>
/// پردازش‌گر پرس‌وجوی جستجوی صفحه‌بندی‌شده پست‌ها.
/// </summary>
public sealed class SearchPostsQueryHandler : IRequestHandler<SearchPostsQuery, Result<PagedResult<PostSummaryDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchPostsQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<PostSummaryDto>>> Handle(
        SearchPostsQuery request,
        CancellationToken cancellationToken)
    {
        var scope = _currentUser.VisibleOrganizationIds.ToHashSet();

        if (request.OrganizationId.HasValue)
        {
            if (!scope.Contains(request.OrganizationId.Value))
            {
                return Result<PagedResult<PostSummaryDto>>.Failure(AccessErrors.Forbidden());
            }

            scope = new HashSet<Guid> { request.OrganizationId.Value };
        }

        var query = _db.Posts
            .AsNoTracking()
            .Where(p => scope.Contains(p.OrganizationId));

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(p => p.Code.Contains(term) || p.Title.Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Code)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var organizationNames = _currentUser.VisibleOrganizations
            .ToDictionary(organization => organization.Id, organization => organization.Name);

        var postIds = items.Select(p => p.Id).ToList();

        var responsibilityTitles = await (
            from assignment in _db.ResponsibilityAssignments.AsNoTracking()
            join responsibility in _db.Responsibilities.AsNoTracking()
                on assignment.ResponsibilityId equals responsibility.Id
            where postIds.Contains(assignment.PostId) && assignment.IsActive && assignment.EndDate == null
            orderby responsibility.Code
            select new { assignment.PostId, Title = responsibility.Title })
            .GroupBy(x => x.PostId)
            .ToDictionaryAsync(group => group.Key, group => group.Select(x => x.Title).ToList(), cancellationToken);

        var authorityAssignments = await (
            from assignment in _db.AuthorityAssignments.AsNoTracking()
            join authority in _db.Authorities.AsNoTracking()
                on assignment.AuthorityId equals authority.Id
            where postIds.Contains(assignment.PostId) && assignment.IsActive && assignment.EndDate == null
            orderby authority.Code
            select new { assignment.PostId, authority.Code, authority.Title })
            .ToListAsync(cancellationToken);

        var authorityTitles = authorityAssignments
            .GroupBy(x => x.PostId)
            .ToDictionary(group => group.Key, group => group.Select(x => x.Title).ToList());

        // نشان «صاحب امضا»: پست‌هایی که حداقل یک انتساب جاری حق امضا دارند (DEC-035).
        var hasSigningAuthority = authorityAssignments
            .Select(x => x.PostId)
            .ToHashSet();

        var summaries = items.Select(post => new PostSummaryDto
        {
            Id = post.Id,
            OrganizationId = post.OrganizationId,
            OrganizationName = organizationNames.GetValueOrDefault(post.OrganizationId),
            Code = post.Code,
            Title = post.Title,
            ParentId = post.ParentId,
            IsActive = post.IsActive,
            HasSigningAuthority = hasSigningAuthority.Contains(post.Id),
            ResponsibilityTitles = responsibilityTitles.GetValueOrDefault(post.Id) ?? new List<string>(),
            AuthorityTitles = authorityTitles.GetValueOrDefault(post.Id) ?? new List<string>()
        }).ToList();

        var result = new PagedResult<PostSummaryDto>(
            summaries,
            totalCount,
            request.Page,
            request.PageSize);

        return Result<PagedResult<PostSummaryDto>>.Success(result);
    }
}