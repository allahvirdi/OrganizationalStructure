using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// خطای ثبت‌شده برای یک ردیف یا سطح بارگذاری واسط.
/// </summary>
/// <remarks>
/// مطابق DEC-030 و ADR-006: پیام خطا فاقد مقدار حساس است (فقط «ردیف + علت»).
/// حذف فیزیکی همراه با Batch پس از ۱۰ روز.
/// </remarks>
public sealed class ImportError : BaseEntity
{
    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private ImportError()
    {
    }

    /// <summary>
    /// شناسه بارگذاری والد.
    /// </summary>
    public Guid BatchId { get; private set; }

    /// <summary>
    /// شناسه ردیف واسط مرتبط (NULL برای خطاهای سطح فایل/بارگذاری).
    /// </summary>
    public Guid? StagingRowId { get; private set; }

    /// <summary>
    /// شماره ردیف.
    /// </summary>
    public int? RowNumber { get; private set; }

    /// <summary>
    /// نام ستون مرتبط.
    /// </summary>
    public string? ColumnName { get; private set; }

    /// <summary>
    /// کد خطا (مانند Import.EmployeeRowInvalid).
    /// </summary>
    public string ErrorCode { get; private set; } = string.Empty;

    /// <summary>
    /// پیام خطا — بدون افشای مقدار حساس.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// زمان ایجاد خطا.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// ایجاد خطای ردیف.
    /// </summary>
    /// <param name="id">شناسه خطا</param>
    /// <param name="batchId">شناسه بارگذاری</param>
    /// <param name="stagingRowId">شناسه ردیف (اختیاری)</param>
    /// <param name="rowNumber">شماره ردیف</param>
    /// <param name="columnName">نام ستون (اختیاری)</param>
    /// <param name="errorCode">کد خطا</param>
    /// <param name="message">پیام خطا</param>
    /// <param name="createdAt">زمان ایجاد</param>
    /// <returns>خطا</returns>
    public static ImportError Create(
        Guid id,
        Guid batchId,
        Guid? stagingRowId,
        int? rowNumber,
        string? columnName,
        string errorCode,
        string message,
        DateTimeOffset createdAt)
    {
        return new ImportError
        {
            Id = id,
            BatchId = batchId,
            StagingRowId = stagingRowId,
            RowNumber = rowNumber,
            ColumnName = columnName,
            ErrorCode = errorCode,
            Message = message,
            CreatedAt = createdAt
        };
    }
}