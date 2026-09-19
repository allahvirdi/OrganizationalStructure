using ClosedXML.Excel;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// پارسر فایل اکسل ساختار پست‌ها (فرمت .xlsx با هدر فارسی).
/// </summary>
/// <remarks>
/// ستون‌ها: کد پست | عنوان پست | شرح | کد پست والد.
/// ردیف اول هدر است و ردیف‌های بعدی داده.
/// </remarks>
public sealed class PostsExcelParser
{
    /// <summary>حداکثر تعداد ردیف داده مجاز در هر بارگذاری.</summary>
    public const int MaxRows = 500;

    /// <summary>حداکثر حجم فایل مجاز (بایت) — ۵ مگابایت.</summary>
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    /// <summary>
    /// خواندن ردیف‌های پست از جریان فایل اکسل.
    /// </summary>
    /// <param name="stream">جریان فایل اکسل</param>
    /// <returns>نتیجه شامل لیست ردیف‌ها یا خطا</returns>
    public Result<IReadOnlyList<ImportPostRowDto>> Parse(Stream stream)
    {
        if (stream is null || stream.Length == 0)
        {
            return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                ImportErrors.InvalidFile("فایل خالی است."));
        }

        if (stream.Length > MaxFileSizeBytes)
        {
            return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                ImportErrors.InvalidFile("حجم فایل از ۵ مگابایت بیشتر است."));
        }

        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(stream);
        }
        catch
        {
            return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                ImportErrors.InvalidFile("فایل اکسل معتبر نیست یا آسیب دیده است."));
        }

        using (workbook)
        {
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet is null)
            {
                return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                    ImportErrors.InvalidFile("فایل حاوی شیت نیست."));
            }

            var lastRow = worksheet.LastRowUsed();
            if (lastRow is null || lastRow.RowNumber() < 2)
            {
                return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                    ImportErrors.InvalidFile("فایل حاوی ردیف داده نیست (فقط هدر)."));
            }

            var dataRowCount = lastRow.RowNumber() - 1;
            if (dataRowCount > MaxRows)
            {
                return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                    ImportErrors.RowLimitExceeded(MaxRows));
            }

            var rows = new List<ImportPostRowDto>(dataRowCount);
            var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var rowNumber = 2; rowNumber <= lastRow.RowNumber(); rowNumber++)
            {
                var row = worksheet.Row(rowNumber);
                var code = row.Cell(1).GetString().Trim();
                var title = row.Cell(2).GetString().Trim();
                var description = row.Cell(3).GetString().Trim();
                var parentCode = row.Cell(4).GetString().Trim();

                // ردیف‌هایی که کد و عنوان هر دو خالی هستند نادیده گرفته می‌شوند
                if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(title))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(code))
                {
                    return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                        ImportErrors.InvalidFile($"ردیف {rowNumber - 1}: کد پست خالی است."));
                }

                if (string.IsNullOrEmpty(title))
                {
                    return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                        ImportErrors.InvalidFile($"ردیف {rowNumber - 1}: عنوان پست خالی است."));
                }

                if (code.Length > 50)
                {
                    return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                        ImportErrors.InvalidFile($"ردیف {rowNumber - 1}: کد پست بیش از ۵۰ کاراکتر است."));
                }

                if (title.Length > 200)
                {
                    return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                        ImportErrors.InvalidFile($"ردیف {rowNumber - 1}: عنوان پست بیش از ۲۰۰ کاراکتر است."));
                }

                if (!string.IsNullOrEmpty(description) && description.Length > 1000)
                {
                    return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                        ImportErrors.InvalidFile($"ردیف {rowNumber - 1}: شرح پست بیش از ۱۰۰۰ کاراکتر است."));
                }

                if (!seenCodes.Add(code))
                {
                    return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                        ImportErrors.DuplicateCodeInFile(code));
                }

                rows.Add(new ImportPostRowDto(
                    code,
                    title,
                    string.IsNullOrEmpty(description) ? null : description,
                    string.IsNullOrEmpty(parentCode) ? null : parentCode));
            }

            if (rows.Count == 0)
            {
                return Result<IReadOnlyList<ImportPostRowDto>>.Failure(
                    ImportErrors.InvalidFile("هیچ ردیف داده معتبری در فایل یافت نشد."));
            }

            return Result<IReadOnlyList<ImportPostRowDto>>.Success(rows);
        }
    }

    /// <summary>
    /// ساخت فایل قالب نمونه با هدر فارسی و دو ردیف نمونه.
    /// </summary>
    /// <returns>آرایه بایت‌های فایل اکسل قالب</returns>
    public static byte[] BuildTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("ساختار پست‌ها");

        // هدرها
        worksheet.Cell(1, 1).Value = "کد پست";
        worksheet.Cell(1, 2).Value = "عنوان پست";
        worksheet.Cell(1, 3).Value = "شرح";
        worksheet.Cell(1, 4).Value = "کد پست والد";

        // ردیف نمونه اول (ریشه)
        worksheet.Cell(2, 1).Value = "MGR-001";
        worksheet.Cell(2, 2).Value = "مدیرعامل";
        worksheet.Cell(2, 3).Value = "بالاترین سطح مدیریتی";
        worksheet.Cell(2, 4).Value = "";

        // ردیف نمونه دوم (فرزند)
        worksheet.Cell(3, 1).Value = "MGR-002";
        worksheet.Cell(3, 2).Value = "معاون فنی";
        worksheet.Cell(3, 3).Value = "";
        worksheet.Cell(3, 4).Value = "MGR-001";

        // عرض ستون‌ها
        worksheet.Column(1).Width = 18;
        worksheet.Column(2).Width = 30;
        worksheet.Column(3).Width = 40;
        worksheet.Column(4).Width = 18;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}