using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Posts.CreatePost;

/// <summary>
/// پردازش‌گر دستور ایجاد پست سازمانی.
/// </summary>
public sealed class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public CreatePostCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;
        var code = request.Code.Trim();

        var duplicate = await _db.Posts.AnyAsync(
            p => p.OrganizationId == request.OrganizationId && p.Code == code,
            cancellationToken);

        if (duplicate)
        {
            return Result<Guid>.Failure(PostErrors.DuplicateCode(code));
        }

        if (request.ParentId.HasValue)
        {
            var parent = await _db.Posts.FirstOrDefaultAsync(
                p => p.Id == request.ParentId.Value,
                cancellationToken);

            if (parent is null)
            {
                return Result<Guid>.Failure(PostErrors.ParentNotFound(request.ParentId.Value));
            }

            if (parent.OrganizationId != request.OrganizationId)
            {
                return Result<Guid>.Failure(PostErrors.CrossOrganizationMove());
            }
        }

        var post = Post.Create(
            Guid.NewGuid(),
            tenantId,
            request.OrganizationId,
            code,
            request.Title,
            request.Description,
            request.ParentId,
            request.HasSigningAuthority,
            _clock.UtcNow);

        _db.Posts.Add(post);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(post.Id);
    }
}