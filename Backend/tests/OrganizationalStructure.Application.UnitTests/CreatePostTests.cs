using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.CreatePost;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های Handler و Validator دستور ایجاد پست (با Scope صریح).
/// </summary>
public sealed class CreatePostTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext(IEnumerable<Guid> scope)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId, scope));
    }

    /// <summary>
    /// ایجاد پست معتبر در Scope باید موفق باشد و شناسه برگرداند.
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_ShouldSucceed()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new CreatePostCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر", null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// ایجاد پست خارج از Scope باید Forbidden دهد.
    /// </summary>
    [Fact]
    public async Task Handle_OutOfScope_ShouldReturnForbidden()
    {
        var (db, clock, user) = CreateContext(Array.Empty<Guid>());
        var handler = new CreatePostCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new CreatePostCommand(Guid.NewGuid(), "MGR-001", "مدیر", null, null),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    /// <summary>
    /// کد تکراری در سازمان باید خطای Conflict دهد.
    /// </summary>
    [Fact]
    public async Task Handle_DuplicateCode_ShouldReturnConflict()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new CreatePostCommandHandler(db, clock, user);

        await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر", null, null),
            CancellationToken.None);

        var result = await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر دوم", null, null),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    /// <summary>
    /// والد ناموجود باید خطای NotFound دهد.
    /// </summary>
    [Fact]
    public async Task Handle_MissingParent_ShouldReturnNotFound()
    {
        var organizationId = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { organizationId });
        var handler = new CreatePostCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر", null, Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// والد از سازمان دیگر باید خطای CrossOrganization دهد.
    /// </summary>
    [Fact]
    public async Task Handle_ParentFromOtherOrganization_ShouldReturnConflict()
    {
        var orgA = Guid.NewGuid();
        var orgB = Guid.NewGuid();
        var (db, clock, user) = CreateContext(new[] { orgA, orgB });
        var handler = new CreatePostCommandHandler(db, clock, user);

        var parentResult = await handler.Handle(
            new CreatePostCommand(orgA, "PAR-001", "والد", null, null),
            CancellationToken.None);
        parentResult.IsSuccess.Should().BeTrue();

        var result = await handler.Handle(
            new CreatePostCommand(orgB, "CHD-001", "فرزند", null, parentResult.Value),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Post.CrossOrganizationMove");
    }

    /// <summary>
    /// اعتبارسنج باید ورودی خالی را رد کند.
    /// </summary>
    [Fact]
    public void Validator_EmptyCodeAndTitle_ShouldBeInvalid()
    {
        var validator = new CreatePostCommandValidator();

        var result = validator.Validate(
            new CreatePostCommand(Guid.NewGuid(), string.Empty, string.Empty, null, null));

        result.IsValid.Should().BeFalse();
    }
}