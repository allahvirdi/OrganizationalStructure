using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.UpdatePost;

/// <summary>
/// پردازش‌گر دستور ویرایش پست سازمانی.
/// </summary>
public sealed class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public UpdatePostCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(
            p => p.Id == request.PostId,
            cancellationToken);

        if (post is null)
        {
            return Result.Failure(PostErrors.NotFound(request.PostId));
        }

        var code = request.Code.Trim();
        if (!string.Equals(post.Code, code, StringComparison.Ordinal))
        {
            var duplicate = await _db.Posts.AnyAsync(
                p => p.OrganizationId == post.OrganizationId
                    && p.Code == code
                    && p.Id != post.Id,
                cancellationToken);

            if (duplicate)
            {
                return Result.Failure(PostErrors.DuplicateCode(code));
            }

            post.ChangeCode(code, _clock.UtcNow);
        }

        post.UpdateDetails(request.Title, request.Description, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}