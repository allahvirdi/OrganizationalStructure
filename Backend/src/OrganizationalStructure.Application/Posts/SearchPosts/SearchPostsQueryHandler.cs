using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.SearchPosts;

/// <summary>
/// پردازش‌گر پرس‌وجوی جستجوی صفحه‌بندی‌شده پست‌ها.
/// </summary>
public sealed class SearchPostsQueryHandler : IRequestHandler<SearchPostsQuery, Result<PagedResult<PostDto>>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchPostsQueryHandler(IAppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<PostDto>>> Handle(
        SearchPostsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Posts.AsNoTracking().AsQueryable();

        if (request.OrganizationId.HasValue)
        {
            query = query.Where(p => p.OrganizationId == request.OrganizationId.Value);
        }

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

        var result = new PagedResult<PostDto>(
            _mapper.Map<IReadOnlyList<PostDto>>(items),
            totalCount,
            request.Page,
            request.PageSize);

        return Result<PagedResult<PostDto>>.Success(result);
    }
}