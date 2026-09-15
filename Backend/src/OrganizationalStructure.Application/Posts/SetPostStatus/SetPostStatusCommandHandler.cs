using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.SetPostStatus;

/// <summary>
/// پردازش‌گر دستور تعیین وضعیت فعال/غیرفعال پست.
/// </summary>
public sealed class SetPostStatusCommandHandler : IRequestHandler<SetPostStatusCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SetPostStatusCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(SetPostStatusCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(
            p => p.Id == request.PostId,
            cancellationToken);

        if (post is null)
        {
            return Result.Failure(PostErrors.NotFound(request.PostId));
        }

        if (request.IsActive)
        {
            post.Activate(_clock.UtcNow);
        }
        else
        {
            post.Deactivate(_clock.UtcNow);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}