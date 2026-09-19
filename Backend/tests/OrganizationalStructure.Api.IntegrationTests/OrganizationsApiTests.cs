using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تستهای یکپارچگی Endpoint گزینه‌های سازمان (`GET /api/v1/organizations`).
/// </summary>
/// <remarks>
/// نیاز کارفرما: سازمان با «نام» (نه شناسه خام) و فقط در محدوده کاربر (خود سازمان + زیرمجموعه‌ها).
/// </remarks>
public sealed class OrganizationsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public OrganizationsApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private sealed record OrganizationOptionPayload(
        Guid Id,
        string Name,
        string Code,
        Guid? ParentId,
        int Depth,
        bool IsCurrent);

    private async Task<OrganizationOptionPayload[]> GetOptionsAsync(HttpClient client, string? searchTerm = null)
    {
        var path = searchTerm is null
            ? "/api/v1/organizations"
            : $"/api/v1/organizations?searchTerm={Uri.EscapeDataString(searchTerm)}";

        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = await response.Content.ReadFromJsonAsync<OrganizationOptionPayload[]>();
        options.Should().NotBeNull();
        return options!;
    }

    /// <summary>
    /// پاسخ باید نام/کد سازمان را بدهد و فقط شامل خود سازمان کاربر و زیرمجموعهها باشد.
    /// </summary>
    [Fact]
    public async Task GetOptions_ShouldReturnOwnSubtreeWithNamesOnly()
    {
        var client = _factory.CreateClient();

        var options = await GetOptionsAsync(client);

        options.Select(o => o.Id).Should().BeEquivalentTo(new[]
        {
            TestAuthHandler.TestOrganizationId,
            TestAuthHandler.TestChildOrganizationId,
            TestAuthHandler.TestGrandchildOrganizationId
        });
        options.Select(o => o.Name).Should().Contain(new[]
        {
            TestAuthHandler.TestOrganizationName,
            TestAuthHandler.TestChildOrganizationName,
            TestAuthHandler.TestGrandchildOrganizationName
        });
        options.Should().NotContain(o => o.Id == TestAuthHandler.TestForeignOrganizationId);
        options.Should().NotContain(o => o.Name == TestAuthHandler.TestForeignOrganizationName);
    }

    /// <summary>
    /// سازمان خود کاربر باید با IsCurrent علامت‌گذاری شود و عمق والد/فرزند درست باشد.
    /// </summary>
    [Fact]
    public async Task GetOptions_ShouldMarkCurrentOrganizationAndDepth()
    {
        var client = _factory.CreateClient();

        var options = await GetOptionsAsync(client);

        options.Single(o => o.IsCurrent).Id.Should().Be(TestAuthHandler.TestOrganizationId);
        options.Single(o => o.Id == TestAuthHandler.TestOrganizationId).Depth.Should().Be(0);
        options.Single(o => o.Id == TestAuthHandler.TestChildOrganizationId).Depth.Should().Be(1);
        options.Single(o => o.Id == TestAuthHandler.TestGrandchildOrganizationId).ParentId
            .Should().Be(TestAuthHandler.TestChildOrganizationId);
        options.Single(o => o.Id == TestAuthHandler.TestGrandchildOrganizationId).Code.Should().Be("TEST-GRAND");
    }

    /// <summary>
    /// جستجو روی نام و کد سازمان باید کار کند.
    /// </summary>
    [Fact]
    public async Task GetOptions_WithSearchTerm_ShouldFilterByNameAndCode()
    {
        var client = _factory.CreateClient();

        var byName = await GetOptionsAsync(client, "فناوری");
        byName.Should().ContainSingle();
        byName[0].Id.Should().Be(TestAuthHandler.TestChildOrganizationId);

        var byCode = await GetOptionsAsync(client, "test-grand");
        byCode.Should().ContainSingle();
        byCode[0].Id.Should().Be(TestAuthHandler.TestGrandchildOrganizationId);
    }

    /// <summary>
    /// عبارت جستجوی بیش از حد بلند باید ۴۰۰ بدهد.
    /// </summary>
    [Fact]
    public async Task GetOptions_WithTooLongSearchTerm_ShouldReturn400()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/v1/organizations?searchTerm={new string('a', 101)}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// فهرست سازمان‌ها برای همه نقش‌های احرازشده یکسان است (محدوده، نه نقش).
    /// </summary>
    [Theory]
    [InlineData("SystemAdmin")]
    [InlineData("OrganizationStructureAdmin")]
    public async Task GetOptions_ShouldBeAvailableForAdminRoles(string role)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", role);

        var options = await GetOptionsAsync(client);

        options.Should().HaveCount(3);
    }
}