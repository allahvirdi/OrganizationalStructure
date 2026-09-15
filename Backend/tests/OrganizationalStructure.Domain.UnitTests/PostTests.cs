using FluentAssertions;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Events;

namespace OrganizationalStructure.Domain.UnitTests;

/// <summary>
/// تست‌های واحد Aggregate پست سازمانی (قواعد درخت و چرخه حیات).
/// </summary>
public sealed class PostTests
{
    private static readonly DateTimeOffset OccurredOn =
        new(2026, 9, 14, 12, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// ساخت پست معتبر برای تست.
    /// </summary>
    private static Post CreateValidPost(Guid? id = null, Guid? parentId = null)
    {
        return Post.Create(
            id ?? Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "P-001",
            "مدیر اداره",
            null,
            parentId,
            false,
            OccurredOn);
    }

    /// <summary>
    /// ایجاد پست معتبر باید موفق باشد و رویداد PostCreated منتشر کند.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldSucceedAndRaiseEvent()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        var post = Post.Create(id, tenantId, organizationId, "P-001", "مدیر", null, null, false, OccurredOn);

        post.Id.Should().Be(id);
        post.TenantId.Should().Be(tenantId);
        post.OrganizationId.Should().Be(organizationId);
        post.IsActive.Should().BeTrue();
        post.DomainEvents.Should().ContainSingle(e => e is PostCreated);
    }

    /// <summary>
    /// ایجاد پست با والد برابر خودش باید خطا دهد (جلوگیری از خودارجاعی).
    /// </summary>
    [Fact]
    public void Create_WithSelfAsParent_ShouldThrow()
    {
        var id = Guid.NewGuid();

        var act = () => Post.Create(id, Guid.NewGuid(), Guid.NewGuid(),
            "P-001", "مدیر", null, id, false, OccurredOn);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// تغییر والد به خودش باید خطا دهد.
    /// </summary>
    [Fact]
    public void ChangeParent_ToSelf_ShouldThrow()
    {
        var post = CreateValidPost();

        var act = () => post.ChangeParent(post.Id, OccurredOn);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// تغییر والد به مقدار جدید باید رویداد PostMoved منتشر کند.
    /// </summary>
    [Fact]
    public void ChangeParent_ToNewParent_ShouldRaiseMovedEvent()
    {
        var post = CreateValidPost();
        var newParentId = Guid.NewGuid();
        post.ClearDomainEvents();

        post.ChangeParent(newParentId, OccurredOn);

        post.ParentId.Should().Be(newParentId);
        post.DomainEvents.Should().ContainSingle(e => e is PostMoved);
    }

    /// <summary>
    /// تغییر والد به همان مقدار فعلی نباید رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void ChangeParent_ToSameValue_ShouldNotRaiseEvent()
    {
        var post = CreateValidPost();
        post.ClearDomainEvents();

        post.ChangeParent(post.ParentId, OccurredOn);

        post.DomainEvents.Should().BeEmpty();
    }

    /// <summary>
    /// غیرفعال‌سازی و فعال‌سازی مجدد باید رویدادهای متناظر منتشر کنند.
    /// </summary>
    [Fact]
    public void Deactivate_ThenActivate_ShouldRaiseEvents()
    {
        var post = CreateValidPost();
        post.ClearDomainEvents();

        post.Deactivate(OccurredOn);
        post.IsActive.Should().BeFalse();
        post.DomainEvents.Should().ContainSingle(e => e is PostDeactivated);

        post.ClearDomainEvents();
        post.Activate(OccurredOn);
        post.IsActive.Should().BeTrue();
        post.DomainEvents.Should().ContainSingle(e => e is PostActivated);
    }

    /// <summary>
    /// عملیات تکراری فعال/غیرفعال نباید رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void Activate_WhenAlreadyActive_ShouldNotRaiseEvent()
    {
        var post = CreateValidPost();
        post.ClearDomainEvents();

        post.Activate(OccurredOn);

        post.DomainEvents.Should().BeEmpty();
    }

    /// <summary>
    /// تغییر وضعیت صاحب‌امضا باید رویداد SigningAuthorityChanged منتشر کند.
    /// </summary>
    [Fact]
    public void SetSigningAuthority_Changed_ShouldRaiseEvent()
    {
        var post = CreateValidPost();
        post.ClearDomainEvents();

        post.SetSigningAuthority(true, OccurredOn);

        post.HasSigningAuthority.Should().BeTrue();
        post.DomainEvents.OfType<SigningAuthorityChanged>().Should().ContainSingle();
    }

    /// <summary>
    /// افزودن مسئولیت تکراری نباید دوباره اضافه شود.
    /// </summary>
    [Fact]
    public void AddResponsibility_DuplicateTitle_ShouldNotDuplicate()
    {
        var post = CreateValidPost();
        var responsibility = new ValueObjects.Responsibility("تأیید مرخصی");
        post.ClearDomainEvents();

        post.AddResponsibility(responsibility, OccurredOn);
        post.AddResponsibility(new ValueObjects.Responsibility("تأیید مرخصی"), OccurredOn);

        post.Responsibilities.Should().ContainSingle();
    }
}