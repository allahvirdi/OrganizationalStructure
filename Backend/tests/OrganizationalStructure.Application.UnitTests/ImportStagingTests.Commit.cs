using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پردازش‌گر ثبت نهایی بارگذاری واسط.
/// </summary>
public sealed partial class ImportStagingTests
{
    private static ImportBatch CreateReadyBatch(
        Guid organizationId,
        string fileName = "test.xlsx")
    {
        var batch = ImportBatch.CreateFromFile(Guid.NewGuid(), organizationId, fileName);
        batch.UpdateRowCounts(3, 2, 1);
        return batch;
    }

    private static EmployeeStagingRow CreateStagingRow(
        Guid batchId,
        int rowNumber,
        StagingRowValidationStatus validationStatus,
        string personnelCode = "00000001")
    {
        var row = EmployeeStagingRow.Create(
            Guid.NewGuid(),
            batchId,
            rowNumber,
            personnelCode,
            "علی",
            "رضایی",
            MakeNationalCode(personnelCode + "0"),
            "09120000000",
            new DateOnly(1990, 1, 1),
            5,
            3,
            DateTimeOffset.UtcNow);

        row.SetValidationStatus(validationStatus);
        return row;
    }

    /// <summary>
    /// ثبت نهایی باید ردیف‌های معتبر را به جدول پرسنل منتقل کند.
    /// </summary>
    [Fact]
    public async Task CommitBatch_ValidRows_ShouldCreateEmployees()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var batch = CreateReadyBatch(organizationId);
        var validRow1 = CreateStagingRow(batch.Id, 1, StagingRowValidationStatus.Valid, "00000001");
        var validRow2 = CreateStagingRow(batch.Id, 2, StagingRowValidationStatus.Valid, "00000002");
        var invalidRow = CreateStagingRow(batch.Id, 3, StagingRowValidationStatus.Invalid, "00000003");

        db.ImportBatches.Add(batch);
        db.EmployeeStagingRows.AddRange(validRow1, validRow2, invalidRow);
        await db.SaveChangesAsync();

        var handler = new CommitBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CommitBatchCommand(batch.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CommittedCount.Should().Be(2);
        result.Value.SkippedCount.Should().Be(1);
        result.Value.Errors.Should().HaveCount(1);

        var employees = await db.Employees.ToListAsync();
        employees.Should().HaveCount(2);

        var updatedBatch = await db.ImportBatches.FirstAsync(b => b.Id == batch.Id);
        updatedBatch.Status.Should().Be(ImportBatchStatus.Committed);
        updatedBatch.CommittedCount.Should().Be(2);

        validRow1.CommitStatus.Should().Be(StagingRowCommitStatus.Committed);
        validRow2.CommitStatus.Should().Be(StagingRowCommitStatus.Committed);
        invalidRow.CommitStatus.Should().Be(StagingRowCommitStatus.Skipped);
    }

    /// <summary>
    /// ثبت نهایی بارگذاری یافت‌نشده باید خطای NotFound برگرداند.
    /// </summary>
    [Fact]
    public async Task CommitBatch_NotFound_ShouldReturnNotFound()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var handler = new CommitBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CommitBatchCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// ثبت نهایی بارگذاری خارج از محدوده باید خطای Forbidden برگرداند.
    /// </summary>
    [Fact]
    public async Task CommitBatch_OutOfScope_ShouldReturnForbidden()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());

        var batch = CreateReadyBatch(organizationId);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new CommitBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CommitBatchCommand(batch.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }
}