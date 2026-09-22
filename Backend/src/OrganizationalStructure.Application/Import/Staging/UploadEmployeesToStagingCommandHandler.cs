using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Application.Import.Staging.DTOs;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پردازش‌گر بارگذاری فایل پرسنل در جدول واسط.
/// </summary>
/// <remarks>
/// مطابق DEC-030: هر ردیف مستقل اعتبارسنجی می‌شود؛ ردیف‌های نامعتبر با
/// ValidationStatus=Invalid + ImportError ثبت می‌شوند و مانع بقیه نمی‌شوند.
/// </remarks>
public sealed partial class UploadEmployeesToStagingCommandHandler
    : IRequestHandler<UploadEmployeesToStagingCommand, Result<StagingUploadResultDto>>
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
    public UploadEmployeesToStagingCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<StagingUploadResultDto>> Handle(
        UploadEmployeesToStagingCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Rows.Count == 0)
        {
            return Result<StagingUploadResultDto>.Failure(ImportErrors.InvalidFile("فایل حاوی ردیف داده نیست."));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(request.OrganizationId))
        {
            return Result<StagingUploadResultDto>.Failure(AccessErrors.Forbidden());
        }

        var now = _clock.UtcNow;
        var batchId = Guid.NewGuid();
        var batch = ImportBatch.CreateFromFile(batchId, request.OrganizationId, request.FileName);

        var validCount = 0;
        var invalidCount = 0;
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var seenNationalCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in request.Rows)
        {
            var errors = ValidateRow(row, seenCodes, seenNationalCodes);

            var stagingRow = EmployeeStagingRow.Create(
                Guid.NewGuid(),
                batchId,
                row.RowNumber,
                row.PersonnelCode,
                row.FirstName,
                row.LastName,
                row.NationalCode,
                row.Mobile,
                ParseBirthDate(row.BirthDateRaw),
                ParseInt(row.ServiceYearsRaw),
                ParseInt(row.ServiceMonthsRaw),
                now);

            if (errors.Count == 0)
            {
                stagingRow.SetValidationStatus(StagingRowValidationStatus.Valid);
                validCount++;
            }
            else
            {
                stagingRow.SetValidationStatus(StagingRowValidationStatus.Invalid);
                invalidCount++;

                foreach (var error in errors)
                {
                    _db.ImportErrors.Add(ImportError.Create(
                        Guid.NewGuid(),
                        batchId,
                        stagingRow.Id,
                        row.RowNumber,
                        null,
                        "Import.EmployeeRowInvalid",
                        error,
                        now));
                }
            }

            _db.EmployeeStagingRows.Add(stagingRow);
        }

        batch.UpdateRowCounts(request.Rows.Count, validCount, invalidCount);
        _db.ImportBatches.Add(batch);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<StagingUploadResultDto>.Success(
            new StagingUploadResultDto(batchId, request.Rows.Count, validCount, invalidCount));
    }
}