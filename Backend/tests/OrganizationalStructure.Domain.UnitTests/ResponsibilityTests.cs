using FluentAssertions;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Events;

namespace OrganizationalStructure.Domain.UnitTests;

/// <summary>
/// تست‌های واحد Aggregate مسئولیت سازمانی و انتساب به پست.
/// </summary>
public sealed class ResponsibilityTests
{
    private static readonly DateTimeOffset OccurredOn =
        new(2026, 9, 15, 12, 0, 0, TimeSpan.Zero);

    private static Responsibility CreateValid(
        string code = "SECRETARIAT",
        string title = "مسئول دبیرخانه")
    {
        return Responsibility.Create(
            Guid.NewGuid(), Guid.NewGuid(), code, title, null, OccurredOn);
    }

    /// <summary>
    /// تعریف مسئولیت معتبر باید موفق باشد و رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldSucceedAndRaiseEvent()
    {
        var responsibility = CreateValid();

        responsibility.IsActive.Should().BeTrue();
        responsibility.DomainEvents.Should().ContainSingle(e => e is ResponsibilityCreated);
    }

    /// <summary>
    /// انتساب به پست باید موفق باشد و انتساب را برگرداند.
    /// </summary>
    [Fact]
    public void AssignToPost_ValidData_ShouldSucceedAndReturnAssignment()
    {
        var responsibility = CreateValid();
        var organizationId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        responsibility.ClearDomainEvents();

        var assignment = responsibility.AssignToPost(organizationId, postId, null, null, OccurredOn);

        assignment.PostId.Should().Be(postId);
        assignment.OrganizationId.Should().Be(organizationId);
        assignment.IsCurrent.Should().BeTrue();
        responsibility.DomainEvents.OfType<ResponsibilityAssigned>().Should().ContainSingle();
    }

    /// <summary>
    /// انتساب جاری تکراری باید خطا دهد.
    /// </summary>
    [Fact]
    public void AssignToPost_DuplicateCurrent_ShouldThrow()
    {
        var responsibility = CreateValid();
        var organizationId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        responsibility.AssignToPost(organizationId, postId, null, null, OccurredOn);

        var act = () => responsibility.AssignToPost(organizationId, postId, null, null, OccurredOn);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// پایان انتساب باید موفق باشد و رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void EndAssignment_ExistingCurrent_ShouldSucceedAndRaiseEvent()
    {
        var responsibility = CreateValid();
        var assignment = responsibility.AssignToPost(
            Guid.NewGuid(), Guid.NewGuid(), null, null, OccurredOn);
        responsibility.ClearDomainEvents();

        responsibility.EndAssignment(assignment.Id, new DateOnly(2026, 9, 30), OccurredOn);

        assignment.IsCurrent.Should().BeFalse();
        responsibility.DomainEvents.OfType<ResponsibilityAssignmentEnded>().Should().ContainSingle();
    }

    /// <summary>
    /// پایان انتساب ناموجود باید خطا دهد.
    /// </summary>
    [Fact]
    public void EndAssignment_Missing_ShouldThrow()
    {
        var responsibility = CreateValid();

        var act = () => responsibility.EndAssignment(
            Guid.NewGuid(), new DateOnly(2026, 9, 30), OccurredOn);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// غیرفعال‌سازی باید رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void Deactivate_ShouldRaiseEvent()
    {
        var responsibility = CreateValid();
        responsibility.ClearDomainEvents();

        responsibility.Deactivate(OccurredOn);

        responsibility.IsActive.Should().BeFalse();
        responsibility.DomainEvents.OfType<ResponsibilityDeactivated>().Should().ContainSingle();
    }
}