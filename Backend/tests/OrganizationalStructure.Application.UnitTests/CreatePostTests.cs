using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.CreatePost;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های Handler و Validator دستور ایجاد پست.
/// </summary>
public sealed class CreatePostTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext()
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId));
    }

    /// <summary>
    /// ایجاد پست معتبر باید موفق باشد و شناسه برگرداند.
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_ShouldSucceed()
    {
        var (db, clock, user) = CreateContext();
        var handler = new CreatePostCommandHandler(db, clock, user);
        var organizationId = Guid.NewGuid();

        var result = await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر", null, null, false),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// کد تکراری در سازمان باید خطای Conflict دهد.
    /// </summary>
    [Fact]
    public async Task Handle_DuplicateCode_ShouldReturnConflict()
    {
        var (db, clock, user) = CreateContext();
        var handler = new CreatePostCommandHandler(db, clock, user);
        var organizationId = Guid.NewGuid();

        await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر", null, null, false),
            CancellationToken.None);

        var result = await handler.Handle(
            new CreatePostCommand(organizationId, "MGR-001", "مدیر دوم", null, null, false),
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
        var (db, clock, user) = CreateContext();
        var handler = new CreatePostCommandHandler(db, clock, user);

        var result = await handler.Handle(
            new CreatePostCommand(Guid.NewGuid(), "MGR-001", "مدیر", null, Guid.NewGuid(), false),
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
        var (db, clock, user) = CreateContext();
        var handler = new CreatePostCommandHandler(db, clock, user);

        var parentResult = await handler.Handle(
            new CreatePostCommand(Guid.NewGuid(), "PAR-001", "والد", null, null, false),
            CancellationToken.None);
        parentResult.IsSuccess.Should().BeTrue();

        var result = await handler.Handle(
            new CreatePostCommand(Guid.NewGuid(), "CHD-001", "فرزند", null, parentResult.Value, false),
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
            new CreatePostCommand(Guid.NewGuid(), string.Empty, string.Empty, null, null, false));

        result.IsValid.Should().BeFalse();
    }
}