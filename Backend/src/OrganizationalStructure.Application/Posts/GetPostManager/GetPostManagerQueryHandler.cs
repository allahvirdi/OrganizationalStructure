using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.GetPostManager;

/// <summary>
/// پردازش‌گر پرس‌وجوی رئیس مستقیم (پست والد) یک پست.
/// </summary>
public sealed class GetPostManagerQueryHandler
    : IRequestHandler<GetPostManagerQuery, Result<PostSummaryDto>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostManagerQueryHandler(IAppDbContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PostSummaryDto>> Handle(
        GetPostManagerQuery request,
        CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<PostSummaryDto>.Failure(PostErrors.NotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(post.OrganizationId))
        {
            return Result<PostSummaryDto>.Failure(AccessErrors.Forbidden());
        }

        if (post.ParentId is null)
        {
            return Result<PostSummaryDto>.Failure(PostErrors.NoParent(request.PostId));
        }

        var parent = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == post.ParentId, cancellationToken);

        if (parent is null)
        {
            return Result<PostSummaryDto>.Failure(PostErrors.ParentNotFound(post.ParentId.Value));
        }

        return Result<PostSummaryDto>.Success(_mapper.Map<PostSummaryDto>(parent));
    }
}
