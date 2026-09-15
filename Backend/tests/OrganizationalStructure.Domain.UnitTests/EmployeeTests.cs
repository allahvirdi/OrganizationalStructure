using FluentAssertions;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.Events;

namespace OrganizationalStructure.Domain.UnitTests;

/// <summary>
/// تست‌های واحد Aggregate پرسنل (انتساب چندپستی و چرخه حیات).
/// </summary>
public sealed class EmployeeTests
{
    private static readonly DateTimeOffset OccurredOn =
        new(2026, 9, 14, 12, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// ساخت پرسنل معتبر برای تست.
    /// </summary>
    private static Employee CreateValidEmployee()
    {
        return Employee.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "EMP-001",
            "علی",
            "رضایی",
            "0012345678",
            "09120000000",
            null,
            OccurredOn);
    }

    /// <summary>
    /// ایجاد پرسنل معتبر باید موفق باشد و رویداد EmployeeCreated منتشر کند.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldSucceedAndRaiseEvent()
    {
        var employee = CreateValidEmployee();

        employee.IsActive.Should().BeTrue();
        employee.DomainEvents.Should().ContainSingle(e => e is EmployeeCreated);
    }

    /// <summary>
    /// انتساب به یک پست باید موفق باشد و رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void AssignToPost_ValidPost_ShouldSucceedAndRaiseEvent()
    {
        var employee = CreateValidEmployee();
        var postId = Guid.NewGuid();
        employee.ClearDomainEvents();

        employee.AssignToPost(postId, null, null, true, OccurredOn);

        employee.Assignments.Should().ContainSingle(a => a.PostId == postId && a.IsPrimary);
        employee.DomainEvents.OfType<EmployeeAssignedToPost>().Should().ContainSingle();
    }

    /// <summary>
    /// یک پرسنل می‌تواند هم‌زمان به چند پست منتسب باشد.
    /// </summary>
    [Fact]
    public void AssignToPost_MultiplePosts_ShouldAllowAll()
    {
        var employee = CreateValidEmployee();
        employee.ClearDomainEvents();

        employee.AssignToPost(Guid.NewGuid(), null, null, true, OccurredOn);
        employee.AssignToPost(Guid.NewGuid(), null, null, false, OccurredOn);

        employee.Assignments.Count(a => a.IsActiveAssignment).Should().Be(2);
    }

    /// <summary>
    /// انتساب فعال تکراری به همان پست باید خطا دهد.
    /// </summary>
    [Fact]
    public void AssignToPost_DuplicateActiveAssignment_ShouldThrow()
    {
        var employee = CreateValidEmployee();
        var postId = Guid.NewGuid();
        employee.AssignToPost(postId, null, null, false, OccurredOn);

        var act = () => employee.AssignToPost(postId, null, null, false, OccurredOn);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// انتساب اصلی دوم در صورت وجود اصلی فعال باید خطا دهد.
    /// </summary>
    [Fact]
    public void AssignToPost_SecondPrimaryWhileActiveExists_ShouldThrow()
    {
        var employee = CreateValidEmployee();
        employee.AssignToPost(Guid.NewGuid(), null, null, true, OccurredOn);

        var act = () => employee.AssignToPost(Guid.NewGuid(), null, null, true, OccurredOn);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// بازه تاریخی نامعتبر (شروع بعد از پایان) باید خطا دهد.
    /// </summary>
    [Fact]
    public void AssignToPost_InvalidDateRange_ShouldThrow()
    {
        var employee = CreateValidEmployee();

        var act = () => employee.AssignToPost(Guid.NewGuid(),
            new DateOnly(2026, 10, 1), new DateOnly(2026, 9, 1), false, OccurredOn);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// پایان دادن به انتساب موجود باید موفق باشد و رویداد منتشر کند.
    /// </summary>
    [Fact]
    public void EndAssignment_ExistingActive_ShouldSucceedAndRaiseEvent()
    {
        var employee = CreateValidEmployee();
        var postId = Guid.NewGuid();
        employee.AssignToPost(postId, null, null, true, OccurredOn);
        employee.ClearDomainEvents();

        employee.EndAssignment(postId, new DateOnly(2026, 9, 30), OccurredOn);

        employee.Assignments.Count(a => a.IsActiveAssignment).Should().Be(0);
        employee.DomainEvents.OfType<EmployeeAssignmentEnded>().Should().ContainSingle();
    }

    /// <summary>
    /// پایان دادن به انتساب ناموجود باید خطا دهد.
    /// </summary>
    [Fact]
    public void EndAssignment_NotAssigned_ShouldThrow()
    {
        var employee = CreateValidEmployee();

        var act = () => employee.EndAssignment(Guid.NewGuid(), new DateOnly(2026, 9, 30), OccurredOn);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// اتصال و قطع ارتباط حساب کاربری باید به‌درستی کار کند.
    /// </summary>
    [Fact]
    public void LinkAndUnlinkUser_ShouldWork()
    {
        var employee = CreateValidEmployee();
        var userId = Guid.NewGuid();

        employee.LinkToUser(userId, OccurredOn);
        employee.UserId.Should().Be(userId);

        employee.UnlinkFromUser(OccurredOn);
        employee.UserId.Should().BeNull();
    }
}