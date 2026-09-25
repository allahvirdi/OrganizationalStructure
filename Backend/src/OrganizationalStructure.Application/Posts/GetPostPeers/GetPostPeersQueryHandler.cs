using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.GetPostPeers;

/// <summary>
/// پردازش‌گر پرس‌وجوی هم‌سطح‌های مستقیم یک پست (پست‌هایی با والد مشترک در همان سازمان).
/// </summary>
public sealed class GetPostPeersQueryHandler
    : IRequestHandler<GetPostPeersQuery, Result<IReadOnlyList<PostSummaryDto>>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostPeersQueryHandler(IAppDbContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PostSummaryDto>>> Handle(
        GetPostPeersQuery request,
        CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<IReadOnlyList<PostSummaryDto>>.Failure(PostErrors.NotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(post.OrganizationId))
        {
            return Result<IReadOnlyList<PostSummaryDto>>.Failure(AccessErrors.Forbidden());
        }

        if (post.ParentId is null)
        {
            // پست ریشه است؛ هم‌سطحی ندارد.
            return Result<IReadOnlyList<PostSummaryDto>>.Success(Array.Empty<PostSummaryDto>());
        }

        var peersQuery = _db.Posts
            .AsNoTracking()
            .Where(p => p.ParentId == post.ParentId
                && p.OrganizationId == post.OrganizationId);

        if (!request.IncludeSelf)
        {
            peersQuery = peersQuery.Where(p => p.Id != request.PostId);
        }

        var peers = await peersQuery
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PostSummaryDto>>.Success(
            _mapper.Map<IReadOnlyList<PostSummaryDto>>(peers));
    }
}
