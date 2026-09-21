using System.Text;
using FluentAssertions;
using OrganizationalStructure.Application.Import;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پارسر و قالب CSV ورود پرسنل.
/// </summary>
public sealed partial class ImportEmployeesTests
{
    /// <summary>
    /// پارس CSV معتبر باید ردیف‌ها را برگرداند.
    /// </summary>
    [Fact]
    public void CsvParser_ValidFile_ShouldReturnRows()
    {
        var national = MakeNationalCode("000000010");
        var content = $"{EmployeesImportHeaders.CsvHeaderLine}\r\n" +
                      $"00000001,علی,رضایی,{national},09120000000,1360/05/12,12,3,09190000000\r\n";

        var result = _csvParser.Parse(CreateCsvStream(content));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].PersonnelCode.Should().Be("00000001");
        result.Value[0].NationalCode.Should().Be(national);
        result.Value[0].Mobile.Should().Be("09120000000");
    }

    /// <summary>
    /// CSV با ستون کم باید خطای ردیف بدهد.
    /// </summary>
    [Fact]
    public void CsvParser_MissingColumns_ShouldReturnRowError()
    {
        var content = $"{EmployeesImportHeaders.CsvHeaderLine}\r\n" +
                      "00000001,علی,رضایی,0013542419,09120000000\r\n";

        var result = _csvParser.Parse(CreateCsvStream(content));

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.EmployeeRowInvalid");
    }

    /// <summary>
    /// کد ملی تکراری در CSV باید بدون افشای کد ملی خطا بدهد.
    /// </summary>
    [Fact]
    public void CsvParser_DuplicateNationalCodeInFile_ShouldNotExposeNationalCode()
    {
        var national = MakeNationalCode("000000010");
        var content = $"{EmployeesImportHeaders.CsvHeaderLine}\r\n" +
                      $"00000001,علی,رضایی,{national},09120000000,,,,\r\n" +
                      $"00000002,مریم,احمدی,{national},09121111111,,,,\r\n";

        var result = _csvParser.Parse(CreateCsvStream(content));

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.DuplicateNationalCodeInFile");
        result.Error.Message.Should().NotContain(national);
    }

    /// <summary>
    /// قالب CSV باید هدر فارسی و ردیف نمونه داشته باشد.
    /// </summary>
    [Fact]
    public void CsvTemplate_ShouldContainPersianHeaders()
    {
        var text = Encoding.UTF8.GetString(EmployeesCsvParser.BuildTemplate()).TrimStart((char)0xFEFF);

        text.Should().Contain(EmployeesImportHeaders.CsvHeaderLine);
        text.Should().Contain("00000001,علی,رضایی");
    }
}