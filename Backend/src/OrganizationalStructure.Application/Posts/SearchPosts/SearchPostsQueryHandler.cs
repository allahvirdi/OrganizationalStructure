using MapsterMapper;
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
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchPostsQueryHandler(IAppDbContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
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

        var result = new PagedResult<PostSummaryDto>(
            _mapper.Map<IReadOnlyList<PostSummaryDto>>(items),
            totalCount,
            request.Page,
            request.PageSize);

        return Result<PagedResult<PostSummaryDto>>.Success(result);
    }
}