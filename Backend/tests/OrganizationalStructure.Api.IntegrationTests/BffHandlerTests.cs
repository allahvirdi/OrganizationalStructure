using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Integration;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های مستقیم هندلر احراز هویت BFF (بدون شبکه).
/// </summary>
public sealed class BffHandlerTests
{
    private static readonly Guid SessionOrganizationId =
        Guid.Parse("77777777-7777-7777-7777-777777777777");

    private sealed class MutableClock : IClock
    {
        /// <summary>
        /// زمان قابل تنظیم.
        /// </summary>
        public DateTimeOffset Current { get; set; } =
            new(2026, 9, 15, 12, 0, 0, TimeSpan.Zero);

        /// <inheritdoc />
        public DateTimeOffset UtcNow => Current;
    }

    private sealed class FakeIamClient : IIamClient
    {
        /// <summary>
        /// نتیجه اعتبارسنجی قابل تنظیم.
        /// </summary>
        public IamValidationResult Validation { get; set; } =
            new() { IsValid = false, Error = "نامعتبر" };

        /// <inheritdoc />
        public Task<IamLoginResult> LoginAsync(
            IamLoginRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new IamLoginResult { IsSuccess = false });

        /// <inheritdoc />
        public Task<IamLoginResult> VerifyMfaAsync(
            IamMfaRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new IamLoginResult { IsSuccess = false });

        /// <inheritdoc />
        public Task<IamValidationResult> ValidateAsync(
            string accessToken, CancellationToken cancellationToken = default) =>
            Task.FromResult(Validation);

        /// <inheritdoc />
        public Task<IamTokenResult> RefreshAsync(
            string refreshToken, CancellationToken cancellationToken = default) =>
            Task.FromResult(new IamTokenResult { IsSuccess = false });

        /// <inheritdoc />
        public Task<bool> RevokeAsync(
            string refreshToken, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        /// <inheritdoc />
        public Task<IReadOnlyList<IamOrganizationNode>> GetOrganizationTreeAsync(
            string accessToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<IamOrganizationNode>>(Array.Empty<IamOrganizationNode>());
    }

    private static BffSessionAuthenticationHandler CreateHandler(
        IBffSessionStore store,
        FakeIamClient iam,
        MutableClock clock,
        int cacheSeconds = 60)
    {
        var schemeOptions = new OptionsMonitor<AuthenticationSchemeOptions>(
            new OptionsFactory<AuthenticationSchemeOptions>(
                Array.Empty<IConfigureOptions<AuthenticationSchemeOptions>>(),
                Array.Empty<IPostConfigureOptions<AuthenticationSchemeOptions>>()),
            Array.Empty<IOptionsChangeTokenSource<AuthenticationSchemeOptions>>(),
            new OptionsCache<AuthenticationSchemeOptions>());

        var iamOptions = Options.Create(new IamOptions { ValidationCacheSeconds = cacheSeconds });

        return new BffSessionAuthenticationHandler(
            schemeOptions,
            LoggerFactory.Create(_ => { }),
            UrlEncoder.Default,
            store,
            iam,
            clock,
            iamOptions,
            new OrganizationScopeResolver(iam));
    }

    private static async Task<AuthenticateResult> AuthenticateWithCookieAsync(
        BffSessionAuthenticationHandler handler,
        string? sessionId)
    {
        var context = new DefaultHttpContext();
        if (sessionId is not null)
        {
            context.Request.Headers.Cookie =
                $"{BffSessionAuthenticationHandler.SessionCookieName}={sessionId}";
        }

        await handler.InitializeAsync(
            new AuthenticationScheme(
                BffSessionAuthenticationHandler.SchemeName,
                null,
                typeof(BffSessionAuthenticationHandler)),
            context);

        return await handler.AuthenticateAsync();
    }

    private static BffSession ValidSession(MutableClock clock, string sid) => new()
    {
        SessionId = sid,
        AccessToken = "token",
        RefreshToken = "refresh",
        ExpiresAt = clock.UtcNow.AddMinutes(30),
        ValidatedAt = clock.UtcNow,
        UserId = "u-1",
        TenantId = "t-1",
        OrganizationId = "o-1",
        Roles = new[] { "R" },
        Permissions = new[] { "P" },
        VisibleOrganizationIds = new[] { SessionOrganizationId },
        VisibleOrganizations = new[]
        {
            new OrganizationReference(SessionOrganizationId, "سازمان تست", "T-ROOT", null, 0)
        }
    };

    /// <summary>
    /// نشست دارای مراجع سازمان باید Claim نام/کد سازمان را برای کاربر جاری صادر کند.
    /// </summary>
    [Fact]
    public async Task FreshSession_WithVisibleOrganizations_ShouldEmitScopeNodeClaims()
    {
        var clock = new MutableClock();
        var store = new InMemoryBffSessionStore(clock);
        await store.SaveAsync(ValidSession(clock, "s4"));
        var handler = CreateHandler(store, new FakeIamClient(), clock);

        var result = await AuthenticateWithCookieAsync(handler, "s4");

        result.Succeeded.Should().BeTrue();

        var nodes = result.Principal!.FindAll(ClaimNames.OrganizationScopeNode)
            .Select(claim => OrganizationScopeClaim.Parse(claim.Value))
            .Where(reference => reference is not null)
            .ToArray();

        nodes.Should().ContainSingle();
        nodes[0]!.Id.Should().Be(SessionOrganizationId);
        nodes[0]!.Name.Should().Be("سازمان تست");
        nodes[0]!.Code.Should().Be("T-ROOT");
        nodes[0]!.Depth.Should().Be(0);
    }

    /// <summary>
    /// بدون کوکی باید NoResult برگردد (نه Fail).
    /// </summary>
    [Fact]
    public async Task NoCookie_ShouldReturnNoResult()
    {
        var clock = new MutableClock();
        var handler = CreateHandler(
            new InMemoryBffSessionStore(clock), new FakeIamClient(), clock);

        var result = await AuthenticateWithCookieAsync(handler, null);

        result.None.Should().BeTrue();
    }

    /// <summary>
    /// نشست ناموجود باید Fail دهد.
    /// </summary>
    [Fact]
    public async Task UnknownSession_ShouldFail()
    {
        var clock = new MutableClock();
        var handler = CreateHandler(
            new InMemoryBffSessionStore(clock), new FakeIamClient(), clock);

        var result = await AuthenticateWithCookieAsync(handler, "missing");

        result.Succeeded.Should().BeFalse();
    }

    /// <summary>
    /// نشست تازه باید موفق باشد و ادعاها را بسازد.
    /// </summary>
    [Fact]
    public async Task FreshSession_ShouldSucceedWithClaims()
    {
        var clock = new MutableClock();
        var store = new InMemoryBffSessionStore(clock);
        await store.SaveAsync(ValidSession(clock, "s1"));
        var handler = CreateHandler(store, new FakeIamClient(), clock);

        var result = await AuthenticateWithCookieAsync(handler, "s1");

        result.Succeeded.Should().BeTrue();
        result.Principal!.FindFirst(ClaimNames.UserId)!.Value.Should().Be("u-1");
        result.Principal!.FindAll("permission").Should().ContainSingle(c => c.Value == "P");
        result.Principal!.FindAll(ClaimNames.OrganizationScope).Should().ContainSingle();
    }

    /// <summary>
    /// نشست منقضی باید Fail دهد.
    /// </summary>
    [Fact]
    public async Task ExpiredSession_ShouldFail()
    {
        var clock = new MutableClock();
        var store = new InMemoryBffSessionStore(clock);
        var session = ValidSession(clock, "s2") with { ExpiresAt = clock.UtcNow.AddMinutes(-1) };
        await store.SaveAsync(session);
        var handler = CreateHandler(store, new FakeIamClient(), clock);

        var result = await AuthenticateWithCookieAsync(handler, "s2");

        result.Succeeded.Should().BeFalse();
    }

    /// <summary>
    /// نشست کهنه با اعتبارسنجی ناموفق باید Fail دهد و نشست حذف شود.
    /// </summary>
    [Fact]
    public async Task StaleSession_WithFailedRevalidation_ShouldFailAndRemove()
    {
        var clock = new MutableClock();
        var store = new InMemoryBffSessionStore(clock);
        var session = ValidSession(clock, "s3") with { ValidatedAt = clock.UtcNow.AddMinutes(-10) };
        await store.SaveAsync(session);
        var handler = CreateHandler(store, new FakeIamClient(), clock);

        var result = await AuthenticateWithCookieAsync(handler, "s3");

        result.Succeeded.Should().BeFalse();
        (await store.GetAsync("s3")).Should().BeNull();
    }
}