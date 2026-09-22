using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پردازش‌گرهای بارگذاری بیرونی و اعلام آماده بودن.
/// </summary>
public sealed partial class ImportStagingTests
{
    /// <summary>
    /// ایجاد بارگذاری بیرونی باید وضعیت AwaitingRows داشته باشد.
    /// </summary>
    [Fact]
    public async Task CreateExternalBatch_Valid_ShouldCreateAwaitingBatch()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new CreateExternalBatchCommandHandler(db, user);

        var result = await handler.Handle(
            new CreateExternalBatchCommand(organizationId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        var batch = await db.ImportBatches.SingleAsync();
        batch.Source.Should().Be(ImportBatchSource.External);
        batch.Status.Should().Be(ImportBatchStatus.AwaitingRows);
    }

    /// <summary>
    /// شناسه سازمان خالی باید خطای اعتبارسنجی بدهد.
    /// </summary>
    [Fact]
    public async Task CreateExternalBatch_EmptyOrganization_ShouldFail()
    {
        var (db, clock, user) = CreateContext(new[] { Guid.NewGuid() });
        var handler = new CreateExternalBatchCommandHandler(db, user);

        var result = await handler.Handle(
            new CreateExternalBatchCommand(Guid.Empty),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Validation);
    }

    /// <summary>
    /// سازمان خارج از محدوده باید Forbidden بدهد.
    /// </summary>
    [Fact]
    public async Task CreateExternalBatch_OutOfScope_ShouldReturnForbidden()
    {
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());
        var handler = new CreateExternalBatchCommandHandler(db, user);

        var result = await handler.Handle(
            new CreateExternalBatchCommand(Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    /// <summary>
    /// اعلام آماده بودن با ردیف‌های درج‌شده باید آمار را به‌روز کند.
    /// </summary>
    [Fact]
    public async Task MarkReady_WithRows_ShouldUpdateCountsAndStatus()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var batch = ImportBatch.CreateExternal(Guid.NewGuid(), organizationId);
        db.ImportBatches.Add(batch);
        db.EmployeeStagingRows.Add(EmployeeStagingRow.Create(
            Guid.NewGuid(), batch.Id, 1, "00000001", "علی", "رضایی",
            "0000000000", null, null, null, null, clock.UtcNow));
        await db.SaveChangesAsync();

        var handler = new MarkBatchReadyCommandHandler(db, user);
        var result = await handler.Handle(
            new MarkBatchReadyCommand(batch.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalRows.Should().Be(1);

        var updated = await db.ImportBatches.SingleAsync();
        updated.Status.Should().Be(ImportBatchStatus.Ready);
    }

    /// <summary>
    /// بارگذاری یافت‌نشده باید NotFound بدهد.
    /// </summary>
    [Fact]
    public async Task MarkReady_NotFound_ShouldFail()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new MarkBatchReadyCommandHandler(db, user);

        var result = await handler.Handle(
            new MarkBatchReadyCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// بارگذاری بدون ردیف نباید آماده شود.
    /// </summary>
    [Fact]
    public async Task MarkReady_NoRows_ShouldFail()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var batch = ImportBatch.CreateExternal(Guid.NewGuid(), organizationId);
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new MarkBatchReadyCommandHandler(db, user);
        var result = await handler.Handle(
            new MarkBatchReadyCommand(batch.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.NoRowsInserted");
    }

    /// <summary>
    /// بارگذاری غیر AwaitingRows نباید آماده شود.
    /// </summary>
    [Fact]
    public async Task MarkReady_NotAwaiting_ShouldFail()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });

        var batch = ImportBatch.CreateFromFile(Guid.NewGuid(), organizationId, "test.xlsx");
        db.ImportBatches.Add(batch);
        await db.SaveChangesAsync();

        var handler = new MarkBatchReadyCommandHandler(db, user);
        var result = await handler.Handle(
            new MarkBatchReadyCommand(batch.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Import.BatchNotAwaiting");
    }
}