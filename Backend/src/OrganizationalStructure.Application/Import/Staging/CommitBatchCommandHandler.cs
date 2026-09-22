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
/// پردازش‌گر ثبت نهایی بارگذاری واسط.
/// </summary>
/// <remarks>
/// مطابق DEC-030: ثبت معتبرها + اعلام نامعتبرها.
/// ردیف‌های معتبر به <c>Employee</c> تبدیل می‌شوند (بدون <c>PezhvakMobile</c> و بدون انتساب پست).
/// ردیف‌های نامعتبر یا تکراری در دیتابیس به‌عنوان Skipped علامت‌گذاری شده و در پاسخ گزارش می‌شوند.
/// </remarks>
public sealed partial class CommitBatchCommandHandler
    : IRequestHandler<CommitBatchCommand, Result<StagingCommitResultDto>>
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
    public CommitBatchCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<StagingCommitResultDto>> Handle(
        CommitBatchCommand request,
        CancellationToken cancellationToken)
    {
        var batch = await _db.ImportBatches
            .FirstOrDefaultAsync(b => b.Id == request.BatchId, cancellationToken);

        if (batch is null)
        {
            return Result<StagingCommitResultDto>.Failure(
                new Error("Import.BatchNotFound", "بارگذاری یافت نشد.", ErrorType.NotFound));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(batch.OrganizationId))
        {
            return Result<StagingCommitResultDto>.Failure(AccessErrors.Forbidden());
        }

        if (batch.Status != ImportBatchStatus.Ready)
        {
            return Result<StagingCommitResultDto>.Failure(
                new Error("Import.BatchNotReady", "بارگذاری در وضعیت آماده نیست.", ErrorType.Validation));
        }

        var rows = await _db.EmployeeStagingRows
            .Where(r => r.BatchId == request.BatchId)
            .OrderBy(r => r.RowNumber)
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
        {
            return Result<StagingCommitResultDto>.Failure(
                new Error("Import.NoRowsInserted", "هیچ ردیفی در جدول واسط وجود ندارد.", ErrorType.Validation));
        }

        var existingCodes = await GetExistingPersonnelCodes(rows, cancellationToken);

        var now = _clock.UtcNow;
        var tenantId = _currentUser.TenantId;
        var errors = new List<StagingCommitErrorDto>();
        var committedCount = 0;
        var skippedCount = 0;

        var errorsByRowId = await GetInvalidRowErrors(request.BatchId, rows, cancellationToken);

        foreach (var row in rows)
        {
            if (row.ValidationStatus == StagingRowValidationStatus.Invalid)
            {
                row.MarkSkipped();
                skippedCount++;
                var message = errorsByRowId.TryGetValue(row.Id, out var msg) ? msg : "ردیف نامعتبر.";
                errors.Add(new StagingCommitErrorDto(row.RowNumber, message));
                continue;
            }

            if (existingCodes.Contains(row.PersonnelCode))
            {
                row.MarkSkipped();
                skippedCount++;
                errors.Add(new StagingCommitErrorDto(
                    row.RowNumber,
                    $"کد پرسنلی {row.PersonnelCode} از قبل در دیتابیس وجود دارد."));
                continue;
            }

            var employee = BuildEmployeeFromRow(row, tenantId, batch.OrganizationId, now);
            _db.Employees.Add(employee);
            row.MarkCommitted(employee.Id);
            committedCount++;
            existingCodes.Add(row.PersonnelCode);
        }

        batch.Commit(committedCount, _currentUser.UserId, now);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<StagingCommitResultDto>.Success(
            new StagingCommitResultDto(request.BatchId, committedCount, skippedCount, errors));
    }
}