using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی صاحب‌امضا و مسئولیت پست.
/// </summary>
public sealed class AuthorityApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public AuthorityApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<Guid> CreatePostAsync(HttpClient client, string code)
    {
        var response = await client.PostAsJsonAsync("/api/v1/posts", new
        {
            organizationId = Guid.NewGuid(),
            code,
            title = $"عنوان {code}",
            description = (string?)null,
            parentId = (Guid?)null,
            hasSigningAuthority = false
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<Guid>())!;
    }

    /// <summary>
    /// تعیین صاحب‌امضا باید در دریافت بعدی دیده شود.
    /// </summary>
    [Fact]
    public async Task SetSigningAuthority_ShouldBeVisibleInGet()
    {
        var client = _factory.CreateClient();
        var postId = await CreatePostAsync(client, "SA-001");

        var setResponse = await client.PatchAsJsonAsync(
            $"/api/v1/posts/{postId}/signing-authority",
            new { hasSigningAuthority = true });
        setResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/posts/{postId}");
        var body = await getResponse.Content.ReadAsStringAsync();
        body.Should().Contain("\"hasSigningAuthority\":true");
    }

    /// <summary>
    /// افزودن و حذف مسئولیت باید کار کند.
    /// </summary>
    [Fact]
    public async Task Add_ThenRemove_Responsibility_ShouldWork()
    {
        var client = _factory.CreateClient();
        var postId = await CreatePostAsync(client, "RS-001");

        var addResponse = await client.PostAsJsonAsync(
            $"/api/v1/posts/{postId}/responsibilities",
            new { title = "تأیید مرخصی", description = (string?)null });
        addResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterAdd = await client.GetAsync($"/api/v1/posts/{postId}");
        (await getAfterAdd.Content.ReadAsStringAsync()).Should().Contain("تأیید مرخصی");

        var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/v1/posts/{postId}/responsibilities")
        {
            Content = JsonContent.Create(new { title = "تأیید مرخصی" })
        };
        var removeResponse = await client.SendAsync(deleteRequest);
        removeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterRemove = await client.GetAsync($"/api/v1/posts/{postId}");
        (await getAfterRemove.Content.ReadAsStringAsync()).Should().NotContain("تأیید مرخصی");
    }

    /// <summary>
    /// عملیات روی پست ناموجود باید ۴۰۴ دهد.
    /// </summary>
    [Fact]
    public async Task Authority_MissingPost_ShouldReturn404()
    {
        var client = _factory.CreateClient();
        var missingId = Guid.NewGuid();

        var setResponse = await client.PatchAsJsonAsync(
            $"/api/v1/posts/{missingId}/signing-authority",
            new { hasSigningAuthority = true });
        setResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var addResponse = await client.PostAsJsonAsync(
            $"/api/v1/posts/{missingId}/responsibilities",
            new { title = "X", description = (string?)null });
        addResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}