using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پردازش‌گر رد بارگذاری و موارد تکمیلی ثبت نهایی.
/// </summary>
public sealed partial class ImportStagingTests
{
    /// <summary>
    /// ثبت نهایی باید ردیف‌های با کد تکراری در دیتابیس را رد کند.
    /// </summary>
    [Fact]
    public async Task CommitBatch_DuplicateInDatabase_ShouldSkip()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var batch = CreateReadyBatch(organizationId);
        var row = CreateStagingRow(batch.Id, 1, StagingRowValidationStatus.Valid, "00000001");

        var existingEmployee = Domain.Entities.Employee.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            organizationId,
            "00000001",
            "محمد",
            "احمدی",
            MakeNationalCode("000000010"),
            "09121111111",
            null,
            DateTimeOffset.UtcNow);

        db.ImportBatches.Add(batch);
        db.EmployeeStagingRows.Add(row);
        db.Employees.Add(existingEmployee);
        await db.SaveChangesAsync();

        var handler = new CommitBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CommitBatchCommand(batch.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CommittedCount.Should().Be(0);
        result.Value.SkippedCount.Should().Be(1);
        result.Value.Errors.Should().HaveCount(1);
        result.Value.Errors[0].Message.Should().Contain("از قبل در دیتابیس");
    }

    /// <summary>
    /// ثبت نهایی بارگذاری غیرآماده باید خطای Validation برگرداند.
    /// </summary>
    [Fact]
    public async Task CommitBatch_NotReady_ShouldReturnValidation()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        // CreateExternal creates a batch with AwaitingRows status (not Ready)
        var batch = ImportBatch.CreateExternal(Guid.NewGuid(), organizationId);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new CommitBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CommitBatchCommand(batch.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.BatchNotReady");
    }

    /// <summary>
    /// رد بارگذاری باید وضعیت را به Rejected تغییر دهد.
    /// </summary>
    [Fact]
    public async Task RejectBatch_Valid_ShouldMarkRejected()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var batch = CreateReadyBatch(organizationId);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new RejectBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new RejectBatchCommand(batch.Id, "تکراری است"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var updatedBatch = await db.ImportBatches.FirstAsync(b => b.Id == batch.Id);
        updatedBatch.Status.Should().Be(ImportBatchStatus.Rejected);
        updatedBatch.Notes.Should().Be("تکراری است");
        updatedBatch.ReviewedById.Should().NotBeNull();
    }

    /// <summary>
    /// رد بارگذاری یافت‌نشده باید خطای NotFound برگرداند.
    /// </summary>
    [Fact]
    public async Task RejectBatch_NotFound_ShouldReturnNotFound()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var handler = new RejectBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new RejectBatchCommand(Guid.NewGuid(), null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// رد بارگذاری خارج از محدوده باید خطای Forbidden برگرداند.
    /// </summary>
    [Fact]
    public async Task RejectBatch_OutOfScope_ShouldReturnForbidden()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());

        var batch = CreateReadyBatch(organizationId);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new RejectBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new RejectBatchCommand(batch.Id, null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    /// <summary>
    /// رد بارگذاری غیرآماده باید خطای Validation برگرداند.
    /// </summary>
    [Fact]
    public async Task RejectBatch_NotReady_ShouldReturnValidation()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        // CreateExternal creates a batch with AwaitingRows status (not Ready)
        var batch = ImportBatch.CreateExternal(Guid.NewGuid(), organizationId);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new RejectBatchCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new RejectBatchCommand(batch.Id, null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.BatchNotReady");
    }
}