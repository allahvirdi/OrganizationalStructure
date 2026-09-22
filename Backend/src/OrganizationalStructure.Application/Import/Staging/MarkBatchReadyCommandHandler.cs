using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Import.Staging.DTOs;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پردازش‌گر اعلام آماده بودن بارگذاری بیرونی.
/// </summary>
/// <remarks>
/// تعداد ردیف‌های درج‌شده بیرونی را شمارش و آمار بارگذاری را به‌روز می‌کند.
/// اعتبارسنجی ردیف‌های بیرونی در لحظه درج انجام نشده؛ وضعیت‌ها در لحظه
/// اعلام آماده بودن بر اساس حضور فیلدهای اجباری تعیین می‌شود.
/// </remarks>
public sealed class MarkBatchReadyCommandHandler
    : IRequestHandler<MarkBatchReadyCommand, Result<StagingUploadResultDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="db">زمینه داده</param>
    /// <param name="currentUser">کاربر جاری</param>
    public MarkBatchReadyCommandHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<StagingUploadResultDto>> Handle(
        MarkBatchReadyCommand request,
        CancellationToken cancellationToken)
    {
        var batch = await _db.ImportBatches
            .FirstOrDefaultAsync(b => b.Id == request.BatchId, cancellationToken);

        if (batch is null)
        {
            return Result<StagingUploadResultDto>.Failure(
                new Error("Import.BatchNotFound", "بارگذاری یافت نشد.", ErrorType.NotFound));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(batch.OrganizationId))
        {
            return Result<StagingUploadResultDto>.Failure(AccessErrors.Forbidden());
        }

        if (batch.Status != ImportBatchStatus.AwaitingRows)
        {
            return Result<StagingUploadResultDto>.Failure(
                new Error("Import.BatchNotAwaiting", "بارگذاری در وضعیت انتظار درج نیست.", ErrorType.Validation));
        }

        var totalRows = await _db.EmployeeStagingRows
            .CountAsync(r => r.BatchId == request.BatchId, cancellationToken);

        if (totalRows == 0)
        {
            return Result<StagingUploadResultDto>.Failure(
                new Error("Import.NoRowsInserted", "هیچ ردیفی در جدول واسط درج نشده است.", ErrorType.Validation));
        }

        var validRows = await _db.EmployeeStagingRows
            .CountAsync(r => r.BatchId == request.BatchId && r.ValidationStatus == StagingRowValidationStatus.Valid, cancellationToken);

        var invalidRows = totalRows - validRows;

        batch.UpdateRowCounts(totalRows, validRows, invalidRows);
        batch.MarkReady();

        await _db.SaveChangesAsync(cancellationToken);

        return Result<StagingUploadResultDto>.Success(
            new StagingUploadResultDto(request.BatchId, totalRows, validRows, invalidRows));
    }
}