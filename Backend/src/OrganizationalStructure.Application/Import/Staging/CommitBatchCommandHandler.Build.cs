using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Enums;
using OrganizationalStructure.Domain.ValueObjects;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// توابع کمکی پردازش‌گر ثبت نهایی بارگذاری واسط.
/// </summary>
public sealed partial class CommitBatchCommandHandler
{
    /// <summary>
    /// دریافت کدهای پرسنلی موجود در دیتابیس برای ردیف‌های معتبر بارگذاری.
    /// </summary>
    private async Task<HashSet<string>> GetExistingPersonnelCodes(
        IReadOnlyList<EmployeeStagingRow> rows,
        CancellationToken cancellationToken)
    {
        var rowCodes = rows
            .Where(r => r.ValidationStatus == StagingRowValidationStatus.Valid)
            .Select(r => r.PersonnelCode)
            .ToArray();

        var existing = await _db.Employees
            .Where(e => rowCodes.Contains(e.PersonnelCode))
            .Select(e => e.PersonnelCode)
            .ToListAsync(cancellationToken);

        return new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// دریافت پیام‌های خطای ثبت‌شده برای ردیف‌های نامعتبر بارگذاری.
    /// </summary>
    private async Task<Dictionary<Guid, string>> GetInvalidRowErrors(
        Guid batchId,
        IReadOnlyList<EmployeeStagingRow> rows,
        CancellationToken cancellationToken)
    {
        var invalidRowIds = rows
            .Where(r => r.ValidationStatus == StagingRowValidationStatus.Invalid)
            .Select(r => r.Id)
            .ToList();

        if (invalidRowIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var existingErrors = await _db.ImportErrors
            .Where(e => e.BatchId == batchId && e.StagingRowId != null
                && invalidRowIds.Contains(e.StagingRowId!.Value))
            .ToListAsync(cancellationToken);

        return existingErrors
            .GroupBy(e => e.StagingRowId!.Value)
            .ToDictionary(g => g.Key, g => string.Join("؛ ", g.Select(e => e.Message)));
    }

    /// <summary>
    /// ساخت موجودیت پرسنل از یک ردیف واسط.
    /// </summary>
    private static Employee BuildEmployeeFromRow(
        EmployeeStagingRow row,
        Guid tenantId,
        Guid organizationId,
        DateTimeOffset occurredOn)
    {
        HerasatServiceRecord? serviceRecord = null;
        if (row.ServiceYears.HasValue && row.ServiceMonths.HasValue)
        {
            serviceRecord = new HerasatServiceRecord(row.ServiceYears.Value, row.ServiceMonths.Value);
        }

        return Employee.Create(
            Guid.NewGuid(),
            tenantId,
            organizationId,
            row.PersonnelCode,
            row.FirstName,
            row.LastName,
            row.NationalCode,
            row.Mobile,
            null, // userId
            occurredOn,
            row.BirthDate,
            serviceRecord,
            null, // pezhvakMobile
            null); // pezhvakIsActive
    }
}