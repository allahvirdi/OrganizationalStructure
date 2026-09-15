using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostChildren;

/// <summary>
/// پردازش‌گر پرس‌وجوی فرزندان مستقیم پست.
/// </summary>
public sealed class GetPostChildrenQueryHandler
    : IRequestHandler<GetPostChildrenQuery, Result<IReadOnlyList<PostDto>>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostChildrenQueryHandler(IAppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(
        GetPostChildrenQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await _db.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!exists)
        {
            return Result<IReadOnlyList<PostDto>>.Failure(PostErrors.NotFound(request.PostId));
        }

        var children = await _db.Posts
            .AsNoTracking()
            .Where(p => p.ParentId == request.PostId)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PostDto>>.Success(_mapper.Map<IReadOnlyList<PostDto>>(children));
    }
}