using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پردازش‌گر رد بارگذاری واسط.
/// </summary>
/// <remarks>
/// مطابق DEC-030: بارگذاری فقط در وضعیت <c>Ready</c> قابل رد است.
/// </remarks>
public sealed class RejectBatchCommandHandler
    : IRequestHandler<RejectBatchCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="db">زمینه داده</param>
    /// <param name="clock">ساعت</param>
    /// <param name="currentUser">کاربر جاری</param>
    public RejectBatchCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        RejectBatchCommand request,
        CancellationToken cancellationToken)
    {
        var batch = await _db.ImportBatches
            .FirstOrDefaultAsync(b => b.Id == request.BatchId, cancellationToken);

        if (batch is null)
        {
            return Result.Failure(
                new Error("Import.BatchNotFound", "بارگذاری یافت نشد.", ErrorType.NotFound));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(batch.OrganizationId))
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        if (batch.Status != ImportBatchStatus.Ready)
        {
            return Result.Failure(
                new Error("Import.BatchNotReady", "بارگذاری در وضعیت آماده نیست.", ErrorType.Validation));
        }

        batch.Reject(request.Notes, _currentUser.UserId, _clock.UtcNow);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}