using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های اعتبارسنجی ردیف‌ها و Validator ورود پرسنل.
/// </summary>
public sealed partial class ImportEmployeesTests
{
    /// <summary>
    /// کد ملی نامعتبر باید خطای ردیف بدهد و مقدار حساس افشا نشود.
    /// </summary>
    [Fact]
    public async Task Handle_InvalidNationalCode_ShouldReturnRowError()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[]
            {
                ValidRow() with { NationalCode = "0000000000" }
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.EmployeeRowInvalid");
        result.Error.Message.Should().NotContain("0000000000");
    }

    /// <summary>
    /// شماره همراه نامعتبر باید خطای ردیف بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_InvalidMobile_ShouldReturnRowError()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[]
            {
                ValidRow() with { Mobile = "0912" }
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.EmployeeRowInvalid");
    }

    /// <summary>
    /// تاریخ تولد شمسی نامعتبر باید خطای ردیف بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_InvalidBirthDate_ShouldReturnRowError()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[]
            {
                ValidRow() with { BirthDateRaw = "1400/13/01" }
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.EmployeeRowInvalid");
    }

    /// <summary>
    /// سال سابقه بدون ماه باید خطای ردیف بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_ServiceYearsWithoutMonths_ShouldReturnRowError()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[]
            {
                ValidRow() with { ServiceMonthsRaw = null }
            }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.EmployeeRowInvalid");
    }

    /// <summary>
    /// Validator باید لیست ردیف خالی را رد کند.
    /// </summary>
    [Fact]
    public void Validator_EmptyRows_ShouldBeInvalid()
    {
        var validator = new ImportEmployeesCommandValidator();

        var result = validator.Validate(
            new ImportEmployeesCommand(Guid.NewGuid(), Array.Empty<ImportEmployeeRowDto>()));

        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Validator باید شناسه سازمان خالی را رد کند.
    /// </summary>
    [Fact]
    public void Validator_EmptyOrganizationId_ShouldBeInvalid()
    {
        var validator = new ImportEmployeesCommandValidator();

        var result = validator.Validate(
            new ImportEmployeesCommand(Guid.Empty, new[] { ValidRow() }));

        result.IsValid.Should().BeFalse();
    }
}