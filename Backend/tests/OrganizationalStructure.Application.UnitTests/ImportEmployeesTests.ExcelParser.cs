using ClosedXML.Excel;
using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پارسر و قالب اکسل ورود پرسنل.
/// </summary>
public sealed partial class ImportEmployeesTests
{
    private static string ToPersian(string value)
    {
        var chars = value.ToArray();
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] >= '0' && chars[i] <= '9')
            {
                chars[i] = (char)(0x06F0 + (chars[i] - '0'));
            }
        }

        return new string(chars);
    }

    /// <summary>
    /// پارس فایل اکسل معتبر باید ردیف‌ها را برگرداند و ارقام فارسی را نرمال‌سازی کند.
    /// </summary>
    [Fact]
    public void ExcelParser_ValidFile_ShouldReturnRowsAndNormalizeDigits()
    {
        var national = MakeNationalCode("000000010");
        var bytes = CreateExcelBytes(new[]
        {
            ToPersian("00000001"),
            "علی",
            "رضایی",
            ToPersian(national),
            ToPersian("09120000000"),
            ToPersian("1360/05/12"),
            ToPersian("12"),
            ToPersian("3"),
            ToPersian("09190000000")
        });

        var result = _excelParser.Parse(bytes);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].PersonnelCode.Should().Be("00000001");
        result.Value[0].NationalCode.Should().Be(national);
        result.Value[0].Mobile.Should().Be("09120000000");
        result.Value[0].ServiceYearsRaw.Should().Be("12");
        result.Value[0].ServiceMonthsRaw.Should().Be("3");
    }

    /// <summary>
    /// فایل اکسل فقط هدر باید خطا بدهد.
    /// </summary>
    [Fact]
    public void ExcelParser_HeaderOnly_ShouldReturnError()
    {
        var bytes = CreateExcelBytes();

        var result = _excelParser.Parse(bytes);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.InvalidFile");
    }

    /// <summary>
    /// کد پرسنلی تکراری در فایل اکسل باید خطا بدهد.
    /// </summary>
    [Fact]
    public void ExcelParser_DuplicatePersonnelCodeInFile_ShouldReturnError()
    {
        var bytes = CreateExcelBytes(
            new[] { "00000001", "علی", "رضایی", MakeNationalCode("000000010"), "09120000000", "", "", "", "" },
            new[] { "00000001", "مریم", "احمدی", MakeNationalCode("000000020"), "09121111111", "", "", "", "" });

        var result = _excelParser.Parse(bytes);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.DuplicatePersonnelCodeInFile");
    }

    /// <summary>
    /// کد ملی تکراری در فایل اکسل باید بدون افشای کد ملی خطا بدهد.
    /// </summary>
    [Fact]
    public void ExcelParser_DuplicateNationalCodeInFile_ShouldNotExposeNationalCode()
    {
        var national = MakeNationalCode("000000010");
        var bytes = CreateExcelBytes(
            new[] { "00000001", "علی", "رضایی", national, "09120000000", "", "", "", "" },
            new[] { "00000002", "مریم", "احمدی", national, "09121111111", "", "", "", "" });

        var result = _excelParser.Parse(bytes);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.DuplicateNationalCodeInFile");
        result.Error.Message.Should().NotContain(national);
    }

    /// <summary>
    /// ردیف اکسل با کد پرسنلی خالی باید خطای ردیف بدهد.
    /// </summary>
    [Fact]
    public void ExcelParser_MissingPersonnelCode_ShouldReturnRowError()
    {
        var bytes = CreateExcelBytes(
            new[] { "", "علی", "رضایی", MakeNationalCode("000000010"), "09120000000", "", "", "", "" });

        var result = _excelParser.Parse(bytes);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.EmployeeRowInvalid");
    }

    /// <summary>
    /// قالب اکسل باید هدرهای فارسی و ردیف نمونه داشته باشد.
    /// </summary>
    [Fact]
    public void ExcelTemplate_ShouldContainPersianHeaders()
    {
        var bytes = EmployeesExcelParser.BuildTemplate();
        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        worksheet.Cell(1, 1).GetString().Should().Be(EmployeesImportHeaders.ColumnHeaders[0]);
        worksheet.Cell(1, 4).GetString().Should().Be(EmployeesImportHeaders.ColumnHeaders[3]);
        worksheet.Cell(1, 6).GetString().Should().Be(EmployeesImportHeaders.ColumnHeaders[5]);
        worksheet.Cell(2, 1).GetString().Should().Be("00000001");
    }

    /// <summary>
    /// تبدیل تاریخ شمسی معتبر باید به میلادی درست انجام شود.
    /// </summary>
    [Fact]
    public void JalaliConverter_ValidDate_ShouldConvertToGregorian()
    {
        JalaliConverter.ParseToGregorian("1360/05/12")
            .Should().Be(new DateOnly(1981, 8, 3));

        JalaliConverter.ParseToGregorian(ToPersian("1360/05/12"))
            .Should().Be(new DateOnly(1981, 8, 3));
    }

    /// <summary>
    /// نرمال‌ساز ارقام باید ارقام فارسی را به لاتین تبدیل کند.
    /// </summary>
    [Fact]
    public void PersianDigitNormalizer_ShouldConvertPersianDigits()
    {
        PersianDigitNormalizer.NormalizeAndTrim(ToPersian("01239"))
            .Should().Be("01239");
    }
}