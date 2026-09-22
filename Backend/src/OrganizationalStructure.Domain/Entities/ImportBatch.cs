using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// شناسنامه یک بارگذاری واسط (Staging Batch).
/// </summary>
/// <remarks>
/// مطابق DEC-030: بدون TenantId، بدون Soft Delete (حذف فیزیکی پس از ۱۰ روز).
/// چرخه حیات: AwaitingRows → Ready → Committed / Rejected.
/// </remarks>
public sealed class ImportBatch : AuditableEntity
{
    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private ImportBatch()
    {
    }

    /// <summary>
    /// شناسه سازمان مالک (مرجع IAM؛ بدون FK فیزیکی).
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// منشأ بارگذاری.
    /// </summary>
    public ImportBatchSource Source { get; private set; }

    /// <summary>
    /// نام فایل اصلی (فقط مسیر فایل).
    /// </summary>
    public string? FileName { get; private set; }

    /// <summary>
    /// وضعیت چرخه حیات.
    /// </summary>
    public ImportBatchStatus Status { get; private set; }

    /// <summary>
    /// تعداد کل ردیف‌های بارگذاری‌شده.
    /// </summary>
    public int TotalRows { get; private set; }

    /// <summary>
    /// تعداد ردیف‌های معتبر.
    /// </summary>
    public int ValidRows { get; private set; }

    /// <summary>
    /// تعداد ردیف‌های نامعتبر.
    /// </summary>
    public int InvalidRows { get; private set; }

    /// <summary>
    /// تعداد ثبت‌شده نهایی در لحظه Commit.
    /// </summary>
    public int? CommittedCount { get; private set; }

    /// <summary>
    /// یادداشت بازبین (به‌ویژه هنگام رد).
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// شناسه کاربر تأیید/ردکننده.
    /// </summary>
    public Guid? ReviewedById { get; private set; }

    /// <summary>
    /// زمان تأیید/رد.
    /// </summary>
    public DateTimeOffset? ReviewedAt { get; private set; }

    /// <summary>
    /// زمان ثبت نهایی.
    /// </summary>
    public DateTimeOffset? CommittedAt { get; private set; }

    /// <summary>
    /// Token کنترل همروندی (rowversion).
    /// </summary>
    public byte[] Version { get; private set; } = Array.Empty<byte>();

    /// <summary>
    /// ایجاد بارگذاری جدید از مسیر فایل.
    /// </summary>
    /// <param name="id">شناسه بارگذاری</param>
    /// <param name="organizationId">شناسه سازمان</param>
    /// <param name="fileName">نام فایل</param>
    /// <returns>بارگذاری ایجادشده</returns>
    public static ImportBatch CreateFromFile(Guid id, Guid organizationId, string? fileName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("شناسه بارگذاری معتبر نیست.", nameof(id));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("شناسه سازمان معتبر نیست.", nameof(organizationId));
        }

        return new ImportBatch
        {
            Id = id,
            OrganizationId = organizationId,
            Source = ImportBatchSource.File,
            FileName = fileName,
            Status = ImportBatchStatus.Ready,
            TotalRows = 0,
            ValidRows = 0,
            InvalidRows = 0
        };
    }

    /// <summary>
    /// ایجاد بارگذاری جدید از مسیر بیرونی.
    /// </summary>
    /// <param name="id">شناسه بارگذاری</param>
    /// <param name="organizationId">شناسه سازمان</param>
    /// <returns>بارگذاری ایجادشده با وضعیت AwaitingRows</returns>
    public static ImportBatch CreateExternal(Guid id, Guid organizationId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("شناسه بارگذاری معتبر نیست.", nameof(id));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("شناسه سازمان معتبر نیست.", nameof(organizationId));
        }

        return new ImportBatch
        {
            Id = id,
            OrganizationId = organizationId,
            Source = ImportBatchSource.External,
            Status = ImportBatchStatus.AwaitingRows,
            TotalRows = 0,
            ValidRows = 0,
            InvalidRows = 0
        };
    }

    /// <summary>
    /// به‌روزرسانی آمار ردیف‌ها پس از اعتبارسنجی.
    /// </summary>
    /// <param name="totalRows">تعداد کل</param>
    /// <param name="validRows">تعداد معتبر</param>
    /// <param name="invalidRows">تعداد نامعتبر</param>
    public void UpdateRowCounts(int totalRows, int validRows, int invalidRows)
    {
        TotalRows = totalRows;
        ValidRows = validRows;
        InvalidRows = invalidRows;
    }

    /// <summary>
    /// انتقال به وضعیت آماده (برای مسیر بیرونی پس از درج ردیف‌ها).
    /// </summary>
    public void MarkReady()
    {
        if (Status != ImportBatchStatus.AwaitingRows)
        {
            throw new InvalidOperationException("فقط بارگذاری در وضعیت AwaitingRows می‌تواند Ready شود.");
        }

        Status = ImportBatchStatus.Ready;
    }

    /// <summary>
    /// ثبت نهایی بارگذاری.
    /// </summary>
    /// <param name="committedCount">تعداد ثبت‌شده</param>
    /// <param name="reviewedById">شناسه تأییدکننده</param>
    /// <param name="committedAt">زمان ثبت</param>
    public void Commit(int committedCount, Guid reviewedById, DateTimeOffset committedAt)
    {
        if (Status != ImportBatchStatus.Ready)
        {
            throw new InvalidOperationException("فقط بارگذاری در وضعیت Ready می‌تواند Commit شود.");
        }

        Status = ImportBatchStatus.Committed;
        CommittedCount = committedCount;
        ReviewedById = reviewedById;
        ReviewedAt = committedAt;
        CommittedAt = committedAt;
    }

    /// <summary>
    /// رد بارگذاری.
    /// </summary>
    /// <param name="notes">یادداشت بازبین</param>
    /// <param name="reviewedById">شناسه ردکننده</param>
    /// <param name="reviewedAt">زمان رد</param>
    public void Reject(string? notes, Guid reviewedById, DateTimeOffset reviewedAt)
    {
        if (Status != ImportBatchStatus.Ready)
        {
            throw new InvalidOperationException("فقط بارگذاری در وضعیت Ready می‌تواند Reject شود.");
        }

        Status = ImportBatchStatus.Rejected;
        Notes = notes;
        ReviewedById = reviewedById;
        ReviewedAt = reviewedAt;
    }
}