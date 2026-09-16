using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Integration.Iam;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های هندلر Bearer و انتخاب‌گر Scheme هوشمند.
/// </summary>
public sealed class BearerHandlerTests
{
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
            Task.FromResult<IReadOnlyList<IamOrganizationNode>>(Tree);

        /// <summary>
        /// درخت قابل تنظیم برای تست Scope.
        /// </summary>
        public List<IamOrganizationNode> Tree { get; } = new();
    }

    private static IamBearerAuthenticationHandler CreateHandler(FakeIamClient iam)
    {
        var schemeOptions = new OptionsMonitor<AuthenticationSchemeOptions>(
            new OptionsFactory<AuthenticationSchemeOptions>(
                Array.Empty<IConfigureOptions<AuthenticationSchemeOptions>>(),
                Array.Empty<IPostConfigureOptions<AuthenticationSchemeOptions>>()),
            Array.Empty<IOptionsChangeTokenSource<AuthenticationSchemeOptions>>(),
            new OptionsCache<AuthenticationSchemeOptions>());

        return new IamBearerAuthenticationHandler(
            schemeOptions,
            LoggerFactory.Create(_ => { }),
            UrlEncoder.Default,
            iam,
            new OrganizationScopeResolver(iam));
    }

    private static async Task<AuthenticateResult> AuthenticateWithHeaderAsync(
        IamBearerAuthenticationHandler handler,
        string? authorizationHeader)
    {
        var context = new DefaultHttpContext();
        if (authorizationHeader is not null)
        {
            context.Request.Headers.Authorization = authorizationHeader;
        }

        await handler.InitializeAsync(
            new AuthenticationScheme(
                AuthenticationSchemes.IamBearer,
                null,
                typeof(IamBearerAuthenticationHandler)),
            context);

        return await handler.AuthenticateAsync();
    }

    /// <summary>
    /// بدون هدر باید NoResult برگردد.
    /// </summary>
    [Fact]
    public async Task NoHeader_ShouldReturnNoResult()
    {
        var handler = CreateHandler(new FakeIamClient());

        var result = await AuthenticateWithHeaderAsync(handler, null);

        result.None.Should().BeTrue();
    }

    /// <summary>
    /// توکن نامعتبر باید Fail دهد.
    /// </summary>
    [Fact]
    public async Task InvalidToken_ShouldFail()
    {
        var handler = CreateHandler(new FakeIamClient());

        var result = await AuthenticateWithHeaderAsync(handler, "Bearer bad-token");

        result.Succeeded.Should().BeFalse();
    }

    /// <summary>
    /// توکن معتبر با Scope قابل حل باید موفق باشد و ادعاها را بسازد.
    /// </summary>
    [Fact]
    public async Task ValidToken_WithResolvableScope_ShouldSucceed()
    {
        var orgId = Guid.NewGuid();
        var fake = new FakeIamClient
        {
            Validation = new IamValidationResult
            {
                IsValid = true,
                UserId = "u-9",
                TenantId = "t-9",
                OrganizationId = orgId.ToString(),
                Roles = new[] { "R" },
                Permissions = new[] { "OrganizationStructure.Post.View" }
            }
        };
        fake.Tree.Add(new IamOrganizationNode { Id = orgId });
        var handler = CreateHandler(fake);

        var result = await AuthenticateWithHeaderAsync(handler, "Bearer good-token");

        result.Succeeded.Should().BeTrue();
        result.Principal!.FindFirst(ClaimNames.OrganizationScope)!.Value.Should().Be(orgId.ToString());
        result.Principal!.FindAll("permission").Should().ContainSingle(c =>
            c.Value == "OrganizationStructure.Post.View");
    }

    /// <summary>
    /// انتخاب‌گر باید Bearer را به Scheme توکن و بقیه را به BFF بفرستد.
    /// </summary>
    [Fact]
    public void SelectScheme_ShouldForwardCorrectly()
    {
        var bearerContext = new DefaultHttpContext();
        bearerContext.Request.Headers.Authorization = "Bearer abc";

        var plainContext = new DefaultHttpContext();

        AuthenticationSchemes.SelectScheme(bearerContext).Should().Be(AuthenticationSchemes.IamBearer);
        AuthenticationSchemes.SelectScheme(plainContext).Should().Be(AuthenticationSchemes.Bff);
    }
}