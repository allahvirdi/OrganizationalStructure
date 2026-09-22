using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پردازش‌گر بارگذاری فایل در جدول واسط.
/// </summary>
public sealed partial class ImportStagingTests
{
    /// <summary>
    /// ردیف‌های معتبر باید بارگذاری و وضعیت معتبر بگیرند.
    /// </summary>
    [Fact]
    public async Task UploadToStaging_ValidRows_ShouldCreateBatchAndRows()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new UploadEmployeesToStagingCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new UploadEmployeesToStagingCommand(organizationId, "test.xlsx", new[]
            {
                ValidRow(1, "00000001"),
                ValidRow(2, "00000002")
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalRows.Should().Be(2);
        result.Value.ValidRows.Should().Be(2);
        result.Value.InvalidRows.Should().Be(0);

        db.ImportBatches.Count().Should().Be(1);
        db.EmployeeStagingRows.Count().Should().Be(2);
        db.ImportErrors.Count().Should().Be(0);
    }

    /// <summary>
    /// ردیف با کد ملی نامعتبر باید نامعتبر علامت‌گذاری شود و خطا ثبت گردد.
    /// </summary>
    [Fact]
    public async Task UploadToStaging_InvalidNationalCode_ShouldMarkInvalidAndLogError()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new UploadEmployeesToStagingCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new UploadEmployeesToStagingCommand(organizationId, "test.xlsx", new[]
            {
                ValidRow(1) with { NationalCode = "0000000000" }
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ValidRows.Should().Be(0);
        result.Value.InvalidRows.Should().Be(1);

        var row = await db.EmployeeStagingRows.SingleAsync();
        row.ValidationStatus.Should().Be(StagingRowValidationStatus.Invalid);

        var error = await db.ImportErrors.SingleAsync();
        error.StagingRowId.Should().Be(row.Id);
        error.Message.Should().NotContain("0000000000");
    }

    /// <summary>
    /// ترکیب ردیف معتبر و نامعتبر باید هر دو را ثبت کند.
    /// </summary>
    [Fact]
    public async Task UploadToStaging_MixedRows_ShouldCountCorrectly()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new UploadEmployeesToStagingCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new UploadEmployeesToStagingCommand(organizationId, "test.xlsx", new[]
            {
                ValidRow(1, "00000001"),
                ValidRow(2, "00000002") with { NationalCode = "0000000000" }
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalRows.Should().Be(2);
        result.Value.ValidRows.Should().Be(1);
        result.Value.InvalidRows.Should().Be(1);
    }

    /// <summary>
    /// کد پرسنلی تکراری در فایل باید ردیف دوم را نامعتبر کند.
    /// </summary>
    [Fact]
    public async Task UploadToStaging_DuplicatePersonnelCode_ShouldMarkSecondInvalid()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new UploadEmployeesToStagingCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new UploadEmployeesToStagingCommand(organizationId, "test.xlsx", new[]
            {
                ValidRow(1, "00000001"),
                ValidRow(2, "00000001") with { NationalCode = MakeNationalCode("000000011") }
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ValidRows.Should().Be(1);
        result.Value.InvalidRows.Should().Be(1);
    }

    /// <summary>
    /// لیست خالی باید خطای اعتبارسنجی برگرداند.
    /// </summary>
    [Fact]
    public async Task UploadToStaging_EmptyRows_ShouldFail()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new UploadEmployeesToStagingCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new UploadEmployeesToStagingCommand(organizationId, "test.xlsx", Array.Empty<ImportEmployeeRowDto>()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.InvalidFile");
    }

    /// <summary>
    /// سازمان خارج از محدوده باید Forbidden بدهد.
    /// </summary>
    [Fact]
    public async Task UploadToStaging_OutOfScope_ShouldReturnForbidden()
    {
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());
        var handler = new UploadEmployeesToStagingCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new UploadEmployeesToStagingCommand(Guid.NewGuid(), "test.xlsx", new[] { ValidRow() }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }
}