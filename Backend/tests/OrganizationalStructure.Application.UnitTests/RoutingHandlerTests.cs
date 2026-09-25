using FluentAssertions;
using Mapster;
using MapsterMapper;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Organizations.GetOrganizationStructure;
using OrganizationalStructure.Application.Posts;
using OrganizationalStructure.Application.Posts.CreatePost;
using OrganizationalStructure.Application.Posts.GetPostManager;
using OrganizationalStructure.Application.Posts.GetPostPeers;
using OrganizationalStructure.Application.Responsibilities.AssignResponsibility;
using OrganizationalStructure.Application.Responsibilities.CreateResponsibility;
using OrganizationalStructure.Application.Responsibilities.ResolveResponsibility;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های واحد هندلرهای مسیریابی (GetPostManager، GetPostPeers، ResolveResponsibility، GetOrganizationStructure).
/// </summary>
public sealed class RoutingHandlerTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext(IEnumerable<Guid>? scope = null, IEnumerable<OrganizationReference>? orgs = null)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId, scope, orgs));
    }

    private static IMapper CreateMapper() => new Mapper(new TypeAdapterConfig());

    private static async Task<Guid> SeedPostAsync(
        OrganizationalStructureDbContext db, TestClock clock, TestCurrentUser user,
        Guid organizationId, string code, string title = "پست تست", Guid? parentId = null)
    {
        var handler = new CreatePostCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CreatePostCommand(organizationId, code, title, null, parentId),
            CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    private static async Task<Guid> SeedEmployeeAsync(
        OrganizationalStructureDbContext db, TestClock clock, TestCurrentUser user, Guid orgId)
    {
        var handler = new Employees.CreateEmployee.CreateEmployeeCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new Employees.CreateEmployee.CreateEmployeeCommand(
                orgId, "00000099", "علی", "محمدی", "0012345678",
                "09120000001", null, null, null, null, null),
            CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    // ────────────────────────────────────────────────
    // GetPostManagerQueryHandler
    // ────────────────────────────────────────────────

    /// <summary>
    /// پست معتبر با والد → والد برگردانده شود.
    /// </summary>
    [Fact]
    public async Task GetPostManager_ValidPost_ShouldReturnParent()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var parentId = await SeedPostAsync(db, clock, user, orgId, "ROOT-01", "مدیر کل");
        var childId = await SeedPostAsync(db, clock, user, orgId, "MGR-01", "مدیر اداره", parentId);
        var handler = new GetPostManagerQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostManagerQuery(childId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(parentId);
        result.Value.Code.Should().Be("ROOT-01");
    }

    /// <summary>
    /// پست ریشه (بدون والد) → خطای NoParent.
    /// </summary>
    [Fact]
    public async Task GetPostManager_RootPost_ShouldReturnNoParent()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var rootId = await SeedPostAsync(db, clock, user, orgId, "ROOT-01", "مدیر کل");
        var handler = new GetPostManagerQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostManagerQuery(rootId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Post.NoParent");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// پست یافت نشد → خطای NotFound.
    /// </summary>
    [Fact]
    public async Task GetPostManager_MissingPost_ShouldReturnNotFound()
    {
        var orgId = Guid.NewGuid();
        var (db, _, user) = CreateContext(new[] { orgId });
        var handler = new GetPostManagerQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostManagerQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Post.NotFound");
    }

    /// <summary>
    /// پست خارج از محدوده کاربر → خطای Forbidden.
    /// </summary>
    [Fact]
    public async Task GetPostManager_OutOfScope_ShouldReturnForbidden()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, userInScope) = CreateContext(new[] { orgId });
        var postId = await SeedPostAsync(db, clock, userInScope, orgId, "ROOT-01", "مدیر کل");

        var (_, _, userNoScope) = CreateContext(Array.Empty<Guid>());
        var handler = new GetPostManagerQueryHandler(db, CreateMapper(), userNoScope);

        var result = await handler.Handle(new GetPostManagerQuery(postId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    // ────────────────────────────────────────────────
    // GetPostPeersQueryHandler
    // ────────────────────────────────────────────────

    /// <summary>
    /// پست معتبر با والد → هم‌سطح‌ها برگردانده شوند (خود پست نباشد).
    /// </summary>
    [Fact]
    public async Task GetPostPeers_ValidPost_ShouldReturnSiblings()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var rootId = await SeedPostAsync(db, clock, user, orgId, "ROOT-01", "مدیر کل");
        var child1 = await SeedPostAsync(db, clock, user, orgId, "MGR-01", "اداره الف", rootId);
        var child2 = await SeedPostAsync(db, clock, user, orgId, "MGR-02", "اداره ب", rootId);
        var child3 = await SeedPostAsync(db, clock, user, orgId, "MGR-03", "اداره ج", rootId);
        var handler = new GetPostPeersQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostPeersQuery(child2, IncludeSelf: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(p => p.Id).Should().BeEquivalentTo(new[] { child1, child3 });
    }

    /// <summary>
    /// IncludeSelf=true → خود پست هم در نتیجه باشد.
    /// </summary>
    [Fact]
    public async Task GetPostPeers_IncludeSelf_ShouldContainSelf()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var rootId = await SeedPostAsync(db, clock, user, orgId, "ROOT-01", "مدیر کل");
        var child1 = await SeedPostAsync(db, clock, user, orgId, "MGR-01", "اداره الف", rootId);
        await SeedPostAsync(db, clock, user, orgId, "MGR-02", "اداره ب", rootId);
        var handler = new GetPostPeersQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostPeersQuery(child1, IncludeSelf: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(p => p.Id).Should().Contain(child1);
    }

    /// <summary>
    /// پست ریشه → لیست خالی (موفق).
    /// </summary>
    [Fact]
    public async Task GetPostPeers_RootPost_ShouldReturnEmptyList()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });
        var rootId = await SeedPostAsync(db, clock, user, orgId, "ROOT-01", "مدیر کل");
        var handler = new GetPostPeersQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostPeersQuery(rootId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    /// <summary>
    /// پست یافت نشد → خطای NotFound.
    /// </summary>
    [Fact]
    public async Task GetPostPeers_MissingPost_ShouldReturnNotFound()
    {
        var orgId = Guid.NewGuid();
        var (db, _, user) = CreateContext(new[] { orgId });
        var handler = new GetPostPeersQueryHandler(db, CreateMapper(), user);

        var result = await handler.Handle(new GetPostPeersQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Post.NotFound");
    }

    /// <summary>
    /// پست خارج از محدوده کاربر → خطای Forbidden.
    /// </summary>
    [Fact]
    public async Task GetPostPeers_OutOfScope_ShouldReturnForbidden()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, userInScope) = CreateContext(new[] { orgId });
        var rootId = await SeedPostAsync(db, clock, userInScope, orgId, "ROOT-01", "مدیر کل");
        var childId = await SeedPostAsync(db, clock, userInScope, orgId, "MGR-01", "اداره الف", rootId);

        var (_, _, userNoScope) = CreateContext(Array.Empty<Guid>());
        var handler = new GetPostPeersQueryHandler(db, CreateMapper(), userNoScope);

        var result = await handler.Handle(new GetPostPeersQuery(childId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    // ────────────────────────────────────────────────
    // ResolveResponsibilityQueryHandler
    // ────────────────────────────────────────────────

    /// <summary>
    /// مسئولیت با انتساب فعال → پست و پرسنل برگردانده شوند.
    /// </summary>
    [Fact]
    public async Task ResolveResponsibility_ActiveAssignment_ShouldReturnPostAndEmployees()
    {
        var orgId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgId });

        var postId = await SeedPostAsync(db, clock, user, orgId, "SEC-01", "مسئول دبیرخانه");
        var employeeId = await SeedEmployeeAsync(db, clock, user, orgId);

        var assignPostHandler = new Employees.AssignPost.AssignPostCommandHandler(db, clock, user);
        var assignResult = await assignPostHandler.Handle(
            new Employees.AssignPost.AssignPostCommand(employeeId, postId, null, null, true),
            CancellationToken.None);
        assignResult.IsSuccess.Should().BeTrue();

        var createRespHandler = new CreateResponsibilityCommandHandler(db, clock, user);
        var created = await createRespHandler.Handle(
            new CreateResponsibilityCommand("مسئول دبیرخانه", null),
            CancellationToken.None);
        created.IsSuccess.Should().BeTrue();
        var respCode = created.Value.ToString();

        var assignRespHandler = new AssignResponsibilityCommandHandler(db, clock, user);
        var assigned = await assignRespHandler.Handle(
            new AssignResponsibilityCommand(respCode, postId, null, null),
            CancellationToken.None);
        assigned.IsSuccess.Should().BeTrue();

        var handler = new ResolveResponsibilityQueryHandler(db, user);
        var result = await handler.Handle(
            new ResolveResponsibilityQuery(orgId, respCode), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var dto = result.Value[0];
        dto.PostId.Should().Be(postId);
        dto.OrganizationId.Should().Be(orgId);
        dto.Employees.Should().HaveCount(1);
        dto.Employees[0].EmployeeId.Should().Be(employeeId);
        dto.Employees[0].IsPrimary.Should().BeTrue();
    }

    /// <summary>
    /// مسئولیت یافت نشد → خطای NotFound.
    /// </summary>
    [Fact]
    public async Task ResolveResponsibility_MissingCode_ShouldReturnNotFound()
    {
        var orgId = Guid.NewGuid();
        var (db, _, user) = CreateContext(new[] { orgId });
        var handler = new ResolveResponsibilityQueryHandler(db, user);

        var result = await handler.Handle(
            new ResolveResponsibilityQuery(orgId, "NOPE"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Responsibility.NotFound");
    }

    /// <summary>
    /// خارج از محدوده کاربر → خطای Forbidden.
    /// </summary>
    [Fact]
    public async Task ResolveResponsibility_OutOfScope_ShouldReturnForbidden()
    {
        var orgId = Guid.NewGuid();
        var (db, _, userNoScope) = CreateContext(Array.Empty<Guid>());
        var handler = new ResolveResponsibilityQueryHandler(db, userNoScope);

        var result = await handler.Handle(
            new ResolveResponsibilityQuery(orgId, "ANY"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    // ────────────────────────────────────────────────
    // GetOrganizationStructureQueryHandler
    // ────────────────────────────────────────────────

    /// <summary>
    /// ساختار سازمان با پست‌ها → لیست تخت برگردانده شود.
    /// </summary>
    [Fact]
    public async Task GetOrganizationStructure_ValidOrg_ShouldReturnPosts()
    {
        var orgId = Guid.NewGuid();
        var orgRef = new OrganizationReference(orgId, "اداره کل", "ADM-001", null, 0);
        var (db, clock, user) = CreateContext(new[] { orgId }, new[] { orgRef });

        var rootId = await SeedPostAsync(db, clock, user, orgId, "ROOT-01", "مدیر کل");
        var childId = await SeedPostAsync(db, clock, user, orgId, "MGR-01", "اداره الف", rootId);
        var handler = new GetOrganizationStructureQueryHandler(db, user);

        var result = await handler.Handle(
            new GetOrganizationStructureQuery(orgId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.OrganizationId.Should().Be(orgId);
        result.Value.OrganizationName.Should().Be("اداره کل");
        result.Value.Posts.Should().HaveCount(2);

        var root = result.Value.Posts.First(p => p.Id == rootId);
        root.ParentId.Should().BeNull();
        root.Code.Should().Be("ROOT-01");

        var child = result.Value.Posts.First(p => p.Id == childId);
        child.ParentId.Should().Be(rootId);
    }

    /// <summary>
    /// خارج از محدوده کاربر → خطای Forbidden.
    /// </summary>
    [Fact]
    public async Task GetOrganizationStructure_OutOfScope_ShouldReturnForbidden()
    {
        var orgId = Guid.NewGuid();
        var (db, _, userNoScope) = CreateContext(Array.Empty<Guid>());
        var handler = new GetOrganizationStructureQueryHandler(db, userNoScope);

        var result = await handler.Handle(
            new GetOrganizationStructureQuery(orgId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    /// <summary>
    /// سازمان بدون پست → لیست خالی (موفق).
    /// </summary>
    [Fact]
    public async Task GetOrganizationStructure_NoPosts_ShouldReturnEmptyPosts()
    {
        var orgId = Guid.NewGuid();
        var orgRef = new OrganizationReference(orgId, "اداره خالی", "ADM-002", null, 0);
        var (db, _, user) = CreateContext(new[] { orgId }, new[] { orgRef });
        var handler = new GetOrganizationStructureQueryHandler(db, user);

        var result = await handler.Handle(
            new GetOrganizationStructureQuery(orgId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Posts.Should().BeEmpty();
    }
}
