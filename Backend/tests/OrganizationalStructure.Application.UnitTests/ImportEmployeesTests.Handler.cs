using System.Linq;
using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پردازش‌گر ورود دسته‌جمعی پرسنل.
/// </summary>
public sealed partial class ImportEmployeesTests
{
    /// <summary>
    /// ردیف‌های معتبر باید به‌صورت اتمیک ذخیره شوند.
    /// </summary>
    [Fact]
    public async Task Handle_ValidRows_ShouldPersistEmployees()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[]
            {
                ValidRow(1, "00000001"),
                ValidRow(2, "00000002")
            }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().Be(2);
        db.Employees.Count().Should().Be(2);
    }

    /// <summary>
    /// کد پرسنلی موجود در دیتابیس باید Conflict بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_DuplicatePersonnelCodeInDatabase_ShouldReturnConflict()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[] { ValidRow(1, "00000001") }),
            CancellationToken.None);

        var result = await handler.Handle(
            new ImportEmployeesCommand(organizationId, new[] { ValidRow(1, "00000001") }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Employee.DuplicatePersonnelCode");
    }

    /// <summary>
    /// سازمان خارج از محدوده باید Forbidden بدهد.
    /// </summary>
    [Fact]
    public async Task Handle_OutOfScope_ShouldReturnForbidden()
    {
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());
        var handler = new ImportEmployeesCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new ImportEmployeesCommand(Guid.NewGuid(), new[] { ValidRow() }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }
}