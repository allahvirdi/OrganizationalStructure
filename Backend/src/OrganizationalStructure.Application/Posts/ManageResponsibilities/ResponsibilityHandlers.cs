using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.ValueObjects;

namespace OrganizationalStructure.Application.Posts.ManageResponsibilities;

/// <summary>
/// پردازش‌گر دستور افزودن مسئولیت به پست.
/// </summary>
public sealed class AddResponsibilityCommandHandler : IRequestHandler<AddResponsibilityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AddResponsibilityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(AddResponsibilityCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(
            p => p.Id == request.PostId,
            cancellationToken);

        if (post is null)
        {
            return Result.Failure(PostErrors.NotFound(request.PostId));
        }

        post.AddResponsibility(new Responsibility(request.Title, request.Description), _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

/// <summary>
/// پردازش‌گر دستور حذف مسئولیت از پست.
/// </summary>
public sealed class RemoveResponsibilityCommandHandler : IRequestHandler<RemoveResponsibilityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public RemoveResponsibilityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RemoveResponsibilityCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(
            p => p.Id == request.PostId,
            cancellationToken);

        if (post is null)
        {
            return Result.Failure(PostErrors.NotFound(request.PostId));
        }

        post.RemoveResponsibility(request.Title, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}