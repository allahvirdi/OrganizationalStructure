using System.Text;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// پارسر فایل CSV پرسنل (UTF-8، جداکننده ویرگول، هدر فارسی).
/// </summary>
/// <remarks>
/// از نقل‌قول‌دوتایی RFC 4180 پشتیبانی می‌کند. ستون‌ها مانند قالب اکسل است.
/// </remarks>
public sealed partial class EmployeesCsvParser
{
    /// <summary>حداکثر تعداد ردیف داده مجاز در هر بارگذاری.</summary>
    public const int MaxRows = 500;

    /// <summary>حداکثر حجم فایل مجاز (بایت) — ۵ مگابایت.</summary>
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private const int ColumnCount = 9;

    /// <summary>
    /// خواندن ردیف‌های پرسنل از جریان فایل CSV.
    /// </summary>
    /// <param name="stream">جریان فایل CSV</param>
    /// <returns>نتیجه شامل ردیف‌های پارس‌شده یا خطا</returns>
    public Result<IReadOnlyList<ImportEmployeeRowDto>> Parse(Stream stream)
    {
        if (stream is null || stream.Length == 0)
        {
            return FileFailure("فایل خالی است.");
        }

        if (stream.Length > MaxFileSizeBytes)
        {
            return FileFailure("حجم فایل از ۵ مگابایت بیشتر است.");
        }

        List<string[]> csvRows;
        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            csvRows = ReadRows(reader);
        }
        catch
        {
            return FileFailure("فایل CSV معتبر نیست یا آسیب دیده است.");
        }

        if (csvRows.Count < 2)
        {
            return FileFailure("فایل حاوی ردیف داده نیست (فقط هدر).");
        }

        if (csvRows[0].Length < ColumnCount)
        {
            return FileFailure($"هدر فایل باید حداقل {ColumnCount} ستون داشته باشد.");
        }

        if (csvRows.Count - 1 > MaxRows)
        {
            return Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(
                ImportErrors.RowLimitExceeded(MaxRows));
        }

        var rows = new List<ImportEmployeeRowDto>();
        var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var nationalCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 1; i < csvRows.Count; i++)
        {
            var cols = csvRows[i];
            var dataRow = i;

            if (cols.Length == 1 && string.IsNullOrWhiteSpace(cols[0]))
            {
                continue;
            }

            if (cols.Length < ColumnCount)
            {
                return RowFailure(dataRow, $"تعداد ستون‌ها ({cols.Length}) کمتر از {ColumnCount} مورد انتظار است.");
            }

            var personnelCode = PersianDigitNormalizer.NormalizeAndTrim(cols[0]);
            var firstName = cols[1].Trim();
            var lastName = cols[2].Trim();
            var nationalCode = PersianDigitNormalizer.NormalizeAndTrim(cols[3]);
            var mobile = PersianDigitNormalizer.NormalizeAndTrim(cols[4]);
            var birthDate = cols[5].Trim();
            var serviceYears = PersianDigitNormalizer.NormalizeAndTrim(cols[6]);
            var serviceMonths = PersianDigitNormalizer.NormalizeAndTrim(cols[7]);
            var pezhvakMobile = PersianDigitNormalizer.NormalizeAndTrim(cols[8]);

            if (string.IsNullOrEmpty(personnelCode) &&
                string.IsNullOrEmpty(firstName) &&
                string.IsNullOrEmpty(lastName) &&
                string.IsNullOrEmpty(nationalCode))
            {
                continue;
            }

            if (string.IsNullOrEmpty(personnelCode))
            {
                return RowFailure(dataRow, "کد پرسنلی خالی است.");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                return RowFailure(dataRow, "نام خالی است.");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                return RowFailure(dataRow, "نام خانوادگی خالی است.");
            }

            if (string.IsNullOrEmpty(nationalCode))
            {
                return RowFailure(dataRow, "کد ملی خالی است.");
            }

            if (!codes.Add(personnelCode))
            {
                return Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(
                    ImportErrors.DuplicatePersonnelCodeInFile(dataRow, personnelCode));
            }

            if (!nationalCodes.Add(nationalCode))
            {
                return Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(
                    ImportErrors.DuplicateNationalCodeInFile(dataRow));
            }

            rows.Add(new ImportEmployeeRowDto(
                dataRow,
                personnelCode,
                firstName,
                lastName,
                nationalCode,
                NullIfEmpty(mobile),
                NullIfEmpty(birthDate),
                NullIfEmpty(serviceYears),
                NullIfEmpty(serviceMonths),
                NullIfEmpty(pezhvakMobile)));
        }

        if (rows.Count == 0)
        {
            return FileFailure("هیچ ردیف داده معتبری در فایل یافت نشد.");
        }

        return Result<IReadOnlyList<ImportEmployeeRowDto>>.Success(rows);
    }

    /// <summary>
    /// ساخت خطای فایل نامعتبر.
    /// </summary>
    private static Result<IReadOnlyList<ImportEmployeeRowDto>> FileFailure(string detail) =>
        Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(ImportErrors.InvalidFile(detail));

    /// <summary>
    /// ساخت خطای ردیف نامعتبر.
    /// </summary>
    private static Result<IReadOnlyList<ImportEmployeeRowDto>> RowFailure(int row, string detail) =>
        Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(ImportErrors.EmployeeRowError(row, detail));

    /// <summary>
    /// تبدیل رشته خالی به null.
    /// </summary>
    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;
}