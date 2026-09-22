using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پرس‌وجوهای فهرست بارگذاری‌ها و ردیف‌های واسط.
/// </summary>
public sealed partial class ImportStagingTests
{
    /// <summary>
    /// فهرست بارگذاری‌ها فقط سازمان‌های داخل محدوده را برگرداند.
    /// </summary>
    [Fact]
    public async Task ListBatches_ShouldReturnOnlyInScope()
    {
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { org1 });

        db.ImportBatches.Add(ImportBatch.CreateFromFile(Guid.NewGuid(), org1, "a.xlsx"));
        db.ImportBatches.Add(ImportBatch.CreateFromFile(Guid.NewGuid(), org2, "b.xlsx"));
        await db.SaveChangesAsync();

        var handler = new ListImportBatchesQueryHandler(db, user);
        var result = await handler.Handle(
            new ListImportBatchesQuery(null, null, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].FileName.Should().Be("a.xlsx");
    }

    /// <summary>
    /// فیلتر وضعیت باید درست عمل کند.
    /// </summary>
    [Fact]
    public async Task ListBatches_StatusFilter_ShouldFilter()
    {
        var org = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { org });

        db.ImportBatches.Add(ImportBatch.CreateFromFile(Guid.NewGuid(), org, "a.xlsx"));
        db.ImportBatches.Add(ImportBatch.CreateExternal(Guid.NewGuid(), org));
        await db.SaveChangesAsync();

        var handler = new ListImportBatchesQueryHandler(db, user);
        var result = await handler.Handle(
            new ListImportBatchesQuery(null, ImportBatchStatus.Ready, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
    }

    /// <summary>
    /// ردیف‌های بارگذاری باید با خطاها برگردانده شوند.
    /// </summary>
    [Fact]
    public async Task GetStagingRows_ShouldReturnRowsWithErrors()
    {
        var org = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { org });

        var batchId = Guid.NewGuid();
        var batch = ImportBatch.CreateFromFile(batchId, org, "test.xlsx");
        db.ImportBatches.Add(batch);

        var rowId = Guid.NewGuid();
        var row = EmployeeStagingRow.Create(
            rowId, batchId, 1, "00000001", "علی", "رضایی",
            "0000000000", null, null, null, null, clock.UtcNow);
        row.SetValidationStatus(StagingRowValidationStatus.Invalid);
        db.EmployeeStagingRows.Add(row);

        db.ImportErrors.Add(ImportError.Create(
            Guid.NewGuid(), batchId, rowId, 1, null,
            "Import.EmployeeRowInvalid", "کد ملی معتبر نیست.", clock.UtcNow));
        await db.SaveChangesAsync();

        var handler = new GetStagingRowsQueryHandler(db, user);
        var result = await handler.Handle(
            new GetStagingRowsQuery(batchId, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalCount.Should().Be(1);
        result.Value.Items[0].Errors.Should().HaveCount(1);
        result.Value.Items[0].Errors[0].Message.Should().Be("کد ملی معتبر نیست.");
    }

    /// <summary>
    /// بارگذاری یافت‌نشده در ردیف‌ها باید NotFound بدهد.
    /// </summary>
    [Fact]
    public async Task GetStagingRows_NotFound_ShouldFail()
    {
        var org = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { org });
        var handler = new GetStagingRowsQueryHandler(db, user);

        var result = await handler.Handle(
            new GetStagingRowsQuery(Guid.NewGuid(), 1, 20), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// ردیف‌های بارگذاری خارج از محدوده باید Forbidden بدهد.
    /// </summary>
    [Fact]
    public async Task GetStagingRows_OutOfScope_ShouldReturnForbidden()
    {
        var org = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { org });

        var otherOrg = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        db.ImportBatches.Add(ImportBatch.CreateFromFile(batchId, otherOrg, "test.xlsx"));
        await db.SaveChangesAsync();

        var handler = new GetStagingRowsQueryHandler(db, user);
        var result = await handler.Handle(
            new GetStagingRowsQuery(batchId, 1, 20), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }
}