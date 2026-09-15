using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Posts.SetSigningAuthority;

/// <summary>
/// پردازش‌گر دستور تعیین صاحب‌امضا بودن پست.
/// </summary>
public sealed class SetSigningAuthorityCommandHandler
    : IRequestHandler<SetSigningAuthorityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SetSigningAuthorityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(SetSigningAuthorityCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(
            p => p.Id == request.PostId,
            cancellationToken);

        if (post is null)
        {
            return Result.Failure(PostErrors.NotFound(request.PostId));
        }

        post.SetSigningAuthority(request.HasSigningAuthority, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}