using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.GetPostChildren;

/// <summary>
/// پردازش‌گر پرس‌وجوی فرزندان مستقیم پست.
/// </summary>
public sealed class GetPostChildrenQueryHandler
    : IRequestHandler<GetPostChildrenQuery, Result<IReadOnlyList<PostSummaryDto>>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostChildrenQueryHandler(IAppDbContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PostSummaryDto>>> Handle(
        GetPostChildrenQuery request,
        CancellationToken cancellationToken)
    {
        var parent = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (parent is null)
        {
            return Result<IReadOnlyList<PostSummaryDto>>.Failure(PostErrors.NotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(parent.OrganizationId))
        {
            return Result<IReadOnlyList<PostSummaryDto>>.Failure(AccessErrors.Forbidden());
        }

        var children = await _db.Posts
            .AsNoTracking()
            .Where(p => p.ParentId == request.PostId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PostSummaryDto>>.Success(
            _mapper.Map<IReadOnlyList<PostSummaryDto>>(children));
    }
}