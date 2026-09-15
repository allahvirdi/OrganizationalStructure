using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.CreatePost;
using OrganizationalStructure.Application.Posts.MovePost;
using OrganizationalStructure.Application.Posts.SetPostStatus;
using OrganizationalStructure.Application.Posts.UpdatePost;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های Handler دستورهای ویرایش، جابجایی و وضعیت پست.
/// </summary>
public sealed class UpdateMoveStatusTests
{
    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext()
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId));
    }

    private static async Task<Guid> SeedPostAsync(
        OrganizationalStructureDbContext db,
        TestClock clock,
        TestCurrentUser user,
        Guid organizationId,
        string code,
        Guid? parentId = null)
    {
        var handler = new CreatePostCommandHandler(db, clock, user);
        var result = await handler.Handle(
            new CreatePostCommand(organizationId, code, $"عنوان {code}", null, parentId),
            CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    /// <summary>
    /// ویرایش پست موجود باید موفق باشد.
    /// </summary>
    [Fact]
    public async Task Update_ExistingPost_ShouldSucceed()
    {
        var (db, clock, user) = CreateContext();
        var organizationId = Guid.NewGuid();
        var postId = await SeedPostAsync(db, clock, user, organizationId, "MGR-001");
        var handler = new UpdatePostCommandHandler(db, clock);

        var result = await handler.Handle(
            new UpdatePostCommand(postId, "MGR-001", "عنوان جدید", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// ویرایش پست ناموجود باید NotFound دهد.
    /// </summary>
    [Fact]
    public async Task Update_MissingPost_ShouldReturnNotFound()
    {
        var (db, clock, _) = CreateContext();
        var handler = new UpdatePostCommandHandler(db, clock);

        var result = await handler.Handle(
            new UpdatePostCommand(Guid.NewGuid(), "X", "Y", null),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    /// <summary>
    /// جابجایی زیر والد هم‌سازمان باید موفق باشد.
    /// </summary>
    [Fact]
    public async Task Move_ValidParent_ShouldSucceed()
    {
        var (db, clock, user) = CreateContext();
        var organizationId = Guid.NewGuid();
        var parentId = await SeedPostAsync(db, clock, user, organizationId, "PAR-001");
        var childId = await SeedPostAsync(db, clock, user, organizationId, "CHD-001");
        var handler = new MovePostCommandHandler(db, clock);

        var result = await handler.Handle(
            new MovePostCommand(childId, parentId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// جابجایی که چرخه ایجاد کند باید خطای Cycle دهد.
    /// </summary>
    [Fact]
    public async Task Move_CreatingCycle_ShouldReturnConflict()
    {
        var (db, clock, user) = CreateContext();
        var organizationId = Guid.NewGuid();
        var parentId = await SeedPostAsync(db, clock, user, organizationId, "PAR-001");
        var childId = await SeedPostAsync(db, clock, user, organizationId, "CHD-001", parentId);
        var handler = new MovePostCommandHandler(db, clock);

        var result = await handler.Handle(
            new MovePostCommand(parentId, childId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Post.CycleDetected");
    }

    /// <summary>
    /// غیرفعال و فعال‌سازی مجدد باید موفق باشد.
    /// </summary>
    [Fact]
    public async Task SetStatus_DeactivateThenActivate_ShouldSucceed()
    {
        var (db, clock, user) = CreateContext();
        var postId = await SeedPostAsync(db, clock, user, Guid.NewGuid(), "MGR-001");
        var handler = new SetPostStatusCommandHandler(db, clock);

        var deactivate = await handler.Handle(
            new SetPostStatusCommand(postId, false),
            CancellationToken.None);
        deactivate.IsSuccess.Should().BeTrue();

        var activate = await handler.Handle(
            new SetPostStatusCommand(postId, true),
            CancellationToken.None);
        activate.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// تعیین وضعیت پست ناموجود باید NotFound دهد.
    /// </summary>
    [Fact]
    public async Task SetStatus_MissingPost_ShouldReturnNotFound()
    {
        var (db, clock, _) = CreateContext();
        var handler = new SetPostStatusCommandHandler(db, clock);

        var result = await handler.Handle(
            new SetPostStatusCommand(Guid.NewGuid(), false),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }
}