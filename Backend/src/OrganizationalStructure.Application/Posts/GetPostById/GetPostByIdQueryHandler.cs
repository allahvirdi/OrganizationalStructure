using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostById;

/// <summary>
/// پردازش‌گر پرس‌وجوی دریافت پست با شناسه.
/// </summary>
public sealed class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Result<PostDto>>
{
    private readonly IAppDbContext _db;
    private readonly IMapper _mapper;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostByIdQueryHandler(IAppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<PostDto>.Failure(PostErrors.NotFound(request.PostId));
        }

        return Result<PostDto>.Success(_mapper.Map<PostDto>(post));
    }
}