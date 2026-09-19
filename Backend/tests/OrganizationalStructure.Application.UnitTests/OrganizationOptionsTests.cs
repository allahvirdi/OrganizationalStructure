using FluentAssertions;
using OrganizationalStructure.Application.Organizations.DTOs;
using OrganizationalStructure.Application.Organizations.GetOrganizationOptions;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پرس‌وجوی گزینه‌های سازمان (محدود به سازمان کاربر و زیرمجموعه‌ها).
/// </summary>
public sealed class OrganizationOptionsTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OwnId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid ChildId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid GrandchildId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static TestCurrentUser CreateUser() => new(
        TenantId,
        new[] { OwnId, ChildId, GrandchildId },
        new[]
        {
            new OrganizationReference(OwnId, "شرکت آزمون", "TEST-ROOT", null, 0),
            new OrganizationReference(ChildId, "واحد فناوری", "TEST-CHILD", OwnId, 1),
            new OrganizationReference(GrandchildId, "گروه توسعه", "TEST-GRAND", ChildId, 2)
        },
        OwnId);

    private static async Task<IReadOnlyList<OrganizationOptionDto>> QueryAsync(string? searchTerm)
    {
        var handler = new GetOrganizationOptionsQueryHandler(CreateUser());
        var result = await handler.Handle(
            new GetOrganizationOptionsQuery(searchTerm), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        return result.Value!;
    }

    /// <summary>
    /// خروجی باید خود سازمان (IsCurrent) و زیرمجموعه‌ها را به ترتیب عمق برگرداند.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnOwnOrganizationAndDescendantsOrderedByDepth()
    {
        var options = await QueryAsync(null);

        options.Select(o => o.Id).Should().ContainInOrder(OwnId, ChildId, GrandchildId);
        options.Single(o => o.IsCurrent).Id.Should().Be(OwnId);
        options.Should().OnlyContain(o => o.Name.Length > 0 && o.Code.Length > 0);
        options.Single(o => o.Id == ChildId).ParentId.Should().Be(OwnId);
        options.Single(o => o.Id == GrandchildId).Depth.Should().Be(2);
    }

    /// <summary>
    /// جستجو باید روی «نام» سازمان بدون حساسیت به بزرگی/کوچکی حروف کار کند.
    /// </summary>
    [Fact]
    public async Task Handle_WithSearchTermOnName_ShouldFilter()
    {
        var options = await QueryAsync("فناوری");

        options.Should().ContainSingle();
        options[0].Id.Should().Be(ChildId);
    }

    /// <summary>
    /// جستجو باید روی «کد» سازمان نیز کار کند.
    /// </summary>
    [Fact]
    public async Task Handle_WithSearchTermOnCode_ShouldFilterCaseInsensitively()
    {
        var options = await QueryAsync("test-grand");

        options.Should().ContainSingle();
        options[0].Id.Should().Be(GrandchildId);
    }

    /// <summary>
    /// کاربر بدون محدوده سازمانی نباید هیچ گزینه‌ای ببیند (fail-closed).
    /// </summary>
    [Fact]
    public async Task Handle_WithoutVisibleOrganizations_ShouldReturnEmpty()
    {
        var handler = new GetOrganizationOptionsQueryHandler(new TestCurrentUser(TenantId));
        var result = await handler.Handle(
            new GetOrganizationOptionsQuery(null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    /// <summary>
    /// عبارت جستجوی بیش از حد بلند باید رد شود.
    /// </summary>
    [Fact]
    public void Validator_TooLongSearchTerm_ShouldBeInvalid()
    {
        var validator = new GetOrganizationOptionsQueryValidator();

        validator.Validate(new GetOrganizationOptionsQuery(new string('a', 101)))
            .IsValid.Should().BeFalse();
        validator.Validate(new GetOrganizationOptionsQuery(new string('a', 100)))
            .IsValid.Should().BeTrue();
    }
}
