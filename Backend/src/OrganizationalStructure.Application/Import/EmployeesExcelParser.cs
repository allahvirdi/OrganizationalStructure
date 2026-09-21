using ClosedXML.Excel;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// پارسر فایل اکسل پرسنل (فرمت .xlsx با هدر فارسی).
/// </summary>
/// <remarks>
/// ستون‌ها: کد پرسنلی | نام | نام خانوادگی | کد ملی | موبایل | تاریخ تولد (شمسی) | سال سابقه | ماه سابقه | موبایل پژواک.
/// </remarks>
public sealed partial class EmployeesExcelParser
{
    /// <summary>حداکثر تعداد ردیف داده مجاز در هر بارگذاری.</summary>
    public const int MaxRows = 500;

    /// <summary>حداکثر حجم فایل مجاز (بایت) — ۵ مگابایت.</summary>
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    /// <summary>
    /// خواندن ردیف‌های پرسنل از جریان فایل اکسل.
    /// </summary>
    /// <param name="stream">جریان فایل اکسل</param>
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

        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(stream);
        }
        catch
        {
            return FileFailure("فایل اکسل معتبر نیست یا آسیب دیده است.");
        }

        using (workbook)
        {
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet is null)
            {
                return FileFailure("فایل حاوی شیت نیست.");
            }

            var lastRow = worksheet.LastRowUsed();
            if (lastRow is null || lastRow.RowNumber() < 2)
            {
                return FileFailure("فایل حاوی ردیف داده نیست (فقط هدر).");
            }

            if (lastRow.RowNumber() - 1 > MaxRows)
            {
                return Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(
                    ImportErrors.RowLimitExceeded(MaxRows));
            }

            var rows = new List<ImportEmployeeRowDto>();
            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var nationalCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var dataRow = 0;

            for (var excelRow = 2; excelRow <= lastRow.RowNumber(); excelRow++)
            {
                dataRow++;
                var row = worksheet.Row(excelRow);

                var personnelCode = PersianDigitNormalizer.NormalizeAndTrim(row.Cell(1).GetString());
                var firstName = row.Cell(2).GetString().Trim();
                var lastName = row.Cell(3).GetString().Trim();
                var nationalCode = PersianDigitNormalizer.NormalizeAndTrim(row.Cell(4).GetString());
                var mobile = PersianDigitNormalizer.NormalizeAndTrim(row.Cell(5).GetString());
                var birthDate = row.Cell(6).GetString().Trim();
                var serviceYears = PersianDigitNormalizer.NormalizeAndTrim(row.Cell(7).GetString());
                var serviceMonths = PersianDigitNormalizer.NormalizeAndTrim(row.Cell(8).GetString());
                var pezhvakMobile = PersianDigitNormalizer.NormalizeAndTrim(row.Cell(9).GetString());

                if (IsEmptyRow(personnelCode, firstName, lastName, nationalCode))
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
    }
}