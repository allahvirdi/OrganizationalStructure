using FluentAssertions;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Events;

namespace OrganizationalStructure.Domain.UnitTests;

/// <summary>
/// تست‌های واحد Aggregate اختیار سازمانی و انتساب به پست.
/// </summary>
public sealed class AuthorityTests
{
    private static readonly DateTimeOffset OccurredOn =
        new(2026, 9, 15, 12, 0, 0, TimeSpan.Zero);

    private static Authority CreateValid(
        string code = "SIGNING_AUTHORITY",
        string title = "اختیار امضا")
    {
        return Authority.Create(
            Guid.NewGuid(), Guid.NewGuid(), code, title, null, OccurredOn);
    }

    /// <summary>
    /// تعریف اختیار معتبر باید موفق باشد و رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldSucceedAndRaiseEvent()
    {
        var authority = CreateValid();

        authority.IsActive.Should().BeTrue();
        authority.DomainEvents.Should().ContainSingle(e => e is AuthorityCreated);
    }

    /// <summary>
    /// انتساب به پست باید موفق باشد و انتساب را برگرداند.
    /// </summary>
    [Fact]
    public void AssignToPost_ValidData_ShouldSucceedAndReturnAssignment()
    {
        var authority = CreateValid();
        var organizationId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        authority.ClearDomainEvents();

        var assignment = authority.AssignToPost(organizationId, postId, null, null, OccurredOn);

        assignment.PostId.Should().Be(postId);
        assignment.IsCurrent.Should().BeTrue();
        authority.DomainEvents.OfType<AuthorityAssigned>().Should().ContainSingle();
    }

    /// <summary>
    /// انتساب جاری تکراری باید خطا دهد.
    /// </summary>
    [Fact]
    public void AssignToPost_DuplicateCurrent_ShouldThrow()
    {
        var authority = CreateValid();
        var organizationId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        authority.AssignToPost(organizationId, postId, null, null, OccurredOn);

        var act = () => authority.AssignToPost(organizationId, postId, null, null, OccurredOn);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// پایان انتساب باید موفق باشد و رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void EndAssignment_ExistingCurrent_ShouldSucceedAndRaiseEvent()
    {
        var authority = CreateValid();
        var assignment = authority.AssignToPost(
            Guid.NewGuid(), Guid.NewGuid(), null, null, OccurredOn);
        authority.ClearDomainEvents();

        authority.EndAssignment(assignment.Id, new DateOnly(2026, 9, 30), OccurredOn);

        assignment.IsCurrent.Should().BeFalse();
        authority.DomainEvents.OfType<AuthorityAssignmentEnded>().Should().ContainSingle();
    }

    /// <summary>
    /// غیرفعال‌سازی باید رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void Deactivate_ShouldRaiseEvent()
    {
        var authority = CreateValid();
        authority.ClearDomainEvents();

        authority.Deactivate(OccurredOn);

        authority.IsActive.Should().BeFalse();
        authority.DomainEvents.OfType<AuthorityDeactivated>().Should().ContainSingle();
    }
}