using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.AssignPost;
using OrganizationalStructure.Application.Employees.CreateEmployee;
using OrganizationalStructure.Application.Employees.EndAssignment;
using OrganizationalStructure.Application.Employees.UpdateEmployee;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های Handler و Validator دستورهای پرسنل.
/// </summary>
public sealed class EmployeeCommandTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext()
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId));
    }

    private static CreateEmployeeCommand ValidCreateCommand(string code = "00000001") =>
        new(code, "علی", "رضایی", "0012345678", "09120000000", null, null, null, null, null);

    /// <summary>
    /// ثبت پرسنل معتبر باید موفق باشد.
    /// </summary>
    [Fact]
    public async Task Create_ValidCommand_ShouldSucceed()
    {
        var (db, clock, user) = CreateContext();
        var handler = new CreateEmployeeCommandHandler(db, clock, user);

        var result = await handler.Handle(ValidCreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// کد تکراری باید Conflict دهد.
    /// </summary>
    [Fact]
    public async Task Create_DuplicateCode_ShouldReturnConflict()
    {
        var (db, clock, user) = CreateContext();
        var handler = new CreateEmployeeCommandHandler(db, clock, user);

        await handler.Handle(ValidCreateCommand(), CancellationToken.None);
        var result = await handler.Handle(ValidCreateCommand(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    /// <summary>
    /// کد غیر ۸ رقمی باید توسط اعتبارسنج رد شود.
    /// </summary>
    [Fact]
    public void CreateValidator_BadCode_ShouldBeInvalid()
    {
        var validator = new CreateEmployeeCommandValidator();

        validator.Validate(ValidCreateCommand("ABC")).IsValid.Should().BeFalse();
    }

    /// <summary>
    /// سال بدون ماه سابقه باید رد شود.
    /// </summary>
    [Fact]
    public void CreateValidator_YearsWithoutMonths_ShouldBeInvalid()
    {
        var validator = new CreateEmployeeCommandValidator();
        var command = ValidCreateCommand() with { ServiceYears = 5, ServiceMonths = null };

        validator.Validate(command).IsValid.Should().BeFalse();
    }

    /// <summary>
    /// ویرایش پرسنل ناموجود باید NotFound دهد.
    /// </summary>
    [Fact]
    public async Task Update_MissingEmployee_ShouldReturnNotFound()
    {
        var (db, clock, _) = CreateContext();
        var handler = new UpdateEmployeeCommandHandler(db, clock);

        var result = await handler.Handle(
            new UpdateEmployeeCommand(Guid.NewGuid(), "A", "B", "C", null),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// انتساب به پست ناموجود باید NotFound دهد.
    /// </summary>
    [Fact]
    public async Task Assign_MissingPost_ShouldReturnNotFound()
    {
        var (db, clock, user) = CreateContext();
        var createHandler = new CreateEmployeeCommandHandler(db, clock, user);
        var created = await createHandler.Handle(ValidCreateCommand(), CancellationToken.None);
        var handler = new AssignPostCommandHandler(db, clock);

        var result = await handler.Handle(
            new AssignPostCommand(created.Value, Guid.NewGuid(), null, null, false),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// پایان انتساب ناموجود باید Conflict دهد.
    /// </summary>
    [Fact]
    public async Task EndAssignment_NoActive_ShouldReturnConflict()
    {
        var (db, clock, user) = CreateContext();
        var createHandler = new CreateEmployeeCommandHandler(db, clock, user);
        var created = await createHandler.Handle(ValidCreateCommand(), CancellationToken.None);
        var handler = new EndAssignmentCommandHandler(db, clock);

        var result = await handler.Handle(
            new EndAssignmentCommand(created.Value, Guid.NewGuid(), new DateOnly(2026, 9, 30)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }
}