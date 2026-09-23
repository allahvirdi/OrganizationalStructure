using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.CreateEmployee;
using OrganizationalStructure.Application.Employees.LinkEmployeeUser;
using OrganizationalStructure.Application.Employees.SetEmployeeStatus;
using OrganizationalStructure.Application.Responsibilities.AssignResponsibility;
using OrganizationalStructure.Application.Responsibilities.CreateResponsibility;
using OrganizationalStructure.Application.Responsibilities.EndResponsibilityAssignment;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های تکمیلی Handlerهای Slice پرسنل و Responsibility (مثبت و منفی).
/// </summary>
public sealed class RemainingHandlerTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext(IEnumerable<Guid>? scope = null)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId, scope));
    }

    private static async Task<Guid> SeedEmployeeAsync(
        OrganizationalStructureDbContext db, TestClock clock, TestCurrentUser user, Guid orgId)
    {
        var handler = new CreateEmployeeCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CreateEmployeeCommand(orgId, "00000021", "علی", "رضایی", "0012345678",
                "09120000000", null, null, null, null, null),
            CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    private static async Task<Guid> SeedPostAsync(
        OrganizationalStructureDbContext db,
        TestClock clock,
        TestCurrentUser user,
        Guid? organizationId = null,
        string code = "T-001")
    {
        var handler = new Posts.CreatePost.CreatePostCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new Posts.CreatePost.CreatePostCommand(
                organizationId ?? Guid.NewGuid(), code, "t", null, null),
            CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    /// <summary>
    /// تعیین وضعیت پرسنل (مثبت و منفی).
    /// </summary>
    [Fact]
    public async Task SetEmployeeStatus_Existing_ShouldSucceed_Missing_ShouldFail()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var employeeId = await SeedEmployeeAsync(db, clock, user, orgId);
        var handler = new SetEmployeeStatusCommandHandler(db, clock);

        (await handler.Handle(new SetEmployeeStatusCommand(employeeId, false), CancellationToken.None))
            .IsSuccess.Should().BeTrue();

        (await handler.Handle(new SetEmployeeStatusCommand(Guid.NewGuid(), false), CancellationToken.None))
            .IsFailure.Should().BeTrue();
    }

    /// <summary>
    /// اتصال کاربر (مثبت و منفی).
    /// </summary>
    [Fact]
    public async Task LinkUser_Existing_ShouldSucceed_Missing_ShouldFail()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var employeeId = await SeedEmployeeAsync(db, clock, user, orgId);
        var handler = new LinkEmployeeUserCommandHandler(db, clock);

        (await handler.Handle(new LinkEmployeeUserCommand(employeeId, Guid.NewGuid()), CancellationToken.None))
            .IsSuccess.Should().BeTrue();

        (await handler.Handle(new LinkEmployeeUserCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None))
            .IsFailure.Should().BeTrue();
    }

    /// <summary>
    /// انتساب موفق و پایان موفق (مثبت).
    /// </summary>
    [Fact]
    public async Task AssignThenEnd_ShouldSucceed()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var employeeId = await SeedEmployeeAsync(db, clock, user, organizationId);
        var postId = await SeedPostAsync(db, clock, user, organizationId);
        var assignHandler = new Employees.AssignPost.AssignPostCommandHandler(db, clock, user);
        var endHandler = new Employees.EndAssignment.EndAssignmentCommandHandler(db, clock, user);

        var assign = await assignHandler.Handle(
            new Employees.AssignPost.AssignPostCommand(employeeId, postId, null, null, false),
            CancellationToken.None);
        assign.IsSuccess.Should().BeTrue();

        var end = await endHandler.Handle(
            new Employees.EndAssignment.EndAssignmentCommand(employeeId, postId, new DateOnly(2026, 9, 30)),
            CancellationToken.None);
        end.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// تعریف (با کد خودکار) و انتساب مسئولیت (مثبت و منفی).
    /// </summary>
    [Fact]
    public async Task CreateAndAssignResponsibility_ShouldWork_Missing_ShouldFail()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var postId = await SeedPostAsync(db, clock, user, organizationId);
        var createHandler = new CreateResponsibilityCommandHandler(db, clock, user);
        var assignHandler = new AssignResponsibilityCommandHandler(db, clock, user);
        var endHandler = new EndResponsibilityAssignmentCommandHandler(db, clock, user);

        var created = await createHandler.Handle(
            new CreateResponsibilityCommand("مسئول دبیرخانه", null),
            CancellationToken.None);
        created.IsSuccess.Should().BeTrue();

        // کد به‌صورت خودکار معادل شناسه تولید می‌شود (GUID)
        var code = created.Value.ToString();

        var assigned = await assignHandler.Handle(
            new AssignResponsibilityCommand(code, postId, null, null),
            CancellationToken.None);
        assigned.IsSuccess.Should().BeTrue();

        var missingCode = await assignHandler.Handle(
            new AssignResponsibilityCommand("NOPE", postId, null, null),
            CancellationToken.None);
        missingCode.IsFailure.Should().BeTrue();
        missingCode.Error!.Type.Should().Be(ErrorType.NotFound);

        var ended = await endHandler.Handle(
            new EndResponsibilityAssignmentCommand(assigned.Value, new DateOnly(2026, 9, 30)),
            CancellationToken.None);
        ended.IsSuccess.Should().BeTrue();
    }
}