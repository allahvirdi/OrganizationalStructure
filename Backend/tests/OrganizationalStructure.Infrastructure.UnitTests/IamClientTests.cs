using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Infrastructure.Integration;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Infrastructure.UnitTests;

/// <summary>
/// تست‌های کلاینت سروربه‌سرور سامانه هویت (IAM).
/// </summary>
public sealed class IamClientTests
{
    private const string BaseAddress = "http://iam.local";

    /// <summary>
    /// هندلر ساختگی برای کنترل پاسخ‌های IAM بدون فراخوانی شبکه واقعی.
    /// </summary>
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) =>
            _responder = responder;

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_responder(request));
        }
    }

    /// <summary>
    /// کارخانه HttpClient ساختگی که هندلر کنترل‌شده را برمی‌گرداند.
    /// </summary>
    private sealed class StubClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public StubClientFactory(HttpMessageHandler handler) => _handler = handler;

        public HttpClient CreateClient(string name) => new(_handler, disposeHandler: false);
    }

    private static IamClient CreateClient(
        StubHandler handler,
        string clientId = "personnel-bff",
        string clientSecret = "secret") =>
        new(
            new StubClientFactory(handler),
            Options.Create(new IamOptions
            {
                BaseAddress = BaseAddress,
                ClientId = clientId,
                ClientSecret = clientSecret
            }));

    private static HttpResponseMessage Json(HttpStatusCode status, string body) =>
        new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private static string Token(string payloadJson)
    {
        static string Encode(string value) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

        return $"{Encode("{\"alg\":\"HS256\",\"typ\":\"JWT\"}")}.{Encode(payloadJson)}.{Encode("signature")}";
    }

    /// <summary>
    /// ورود موفق باید توکن‌ها و وضعیت دومرحله‌ای را نگاشت کند.
    /// </summary>
    [Fact]
    public async Task LoginAsync_SuccessEnvelope_ShouldMapTokens()
    {
        var handler = new StubHandler(_ => Json(
            HttpStatusCode.OK,
            "{\"isSuccess\":true,\"value\":{\"accessToken\":\"access-1\",\"refreshToken\":\"refresh-1\",\"requiresMfa\":true,\"mfaChallengeToken\":\"mfa-1\"}}"));

        var result = await CreateClient(handler).LoginAsync(
            new IamLoginRequest { UserName = "admin@iam.com", Password = "Test@123" });

        result.IsSuccess.Should().BeTrue();
        result.AccessToken.Should().Be("access-1");
        result.RefreshToken.Should().Be("refresh-1");
        result.RequiresMfa.Should().BeTrue();
        result.MfaChallengeToken.Should().Be("mfa-1");
    }

    /// <summary>
    /// خطای دامنه IAM با کد وضعیت ۴۰۱ باید پیام دقیق IAM را به مصرف‌کننده برساند.
    /// </summary>
    [Fact]
    public async Task LoginAsync_InvalidCredentialsEnvelope_ShouldSurfaceIamMessage()
    {
        var handler = new StubHandler(_ => Json(
            HttpStatusCode.Unauthorized,
            "{\"isSuccess\":false,\"value\":null,\"error\":\"نام کاربری یا رمز عبور نامعتبر است.\"}"));

        var result = await CreateClient(handler).LoginAsync(
            new IamLoginRequest { UserName = "admin@iam.com", Password = "wrong" });

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("نام کاربری یا رمز عبور نامعتبر است.");
    }

    /// <summary>
    /// بدنه پاسخ غیرقابل تجزیه باید به شکست امن تبدیل شود (fail-closed).
    /// </summary>
    [Fact]
    public async Task LoginAsync_UnparsableBody_ShouldFailClosed()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("<html>error</html>", Encoding.UTF8, "text/html")
        });

        var result = await CreateClient(handler).LoginAsync(
            new IamLoginRequest { UserName = "admin@iam.com", Password = "Test@123" });

        result.IsSuccess.Should().BeFalse();
    }

    /// <summary>
    /// قطع ارتباط با IAM باید به شکست امن تبدیل شود.
    /// </summary>
    [Fact]
    public async Task LoginAsync_TransportFailure_ShouldFailClosed()
    {
        var handler = new StubHandler(_ => throw new HttpRequestException("IAM unreachable"));

        var result = await CreateClient(handler).LoginAsync(
            new IamLoginRequest { UserName = "admin@iam.com", Password = "Test@123" });

        result.IsSuccess.Should().BeFalse();
    }

    /// <summary>
    /// اعتبارسنجی توکن باید هدرهای کلاینت سروربه‌سرور را ارسال کند.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_ShouldSendClientCredentialHeaders()
    {
        var handler = new StubHandler(_ => Json(
            HttpStatusCode.OK,
            "{\"isSuccess\":true,\"value\":{\"isValid\":false}}"));

        await CreateClient(handler, "personnel-bff", "s3cret").ValidateAsync("token");

        handler.LastRequest!.Headers.GetValues("X-Client-Id").Should().ContainSingle("personnel-bff");
        handler.LastRequest.Headers.GetValues("X-Client-Secret").Should().ContainSingle("s3cret");
        handler.LastRequest.RequestUri!.ToString().Should().Be($"{BaseAddress}/api/token/validate");
    }

    /// <summary>
    /// ادعاهای تکمیلی (مستأجر/سازمان/دسترسی) باید از بدنه JWT خوانده شوند.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_ValidToken_ShouldReadExtendedClaims()
    {
        const string organizationId = "11111111-1111-1111-1111-111111111111";
        var token = Token(
            "{\"tenant_id\":\"t-1\",\"organization_id\":\"" + organizationId + "\",\"permission\":[\"OrganizationStructure.Employee.View\"]}");

        var handler = new StubHandler(_ => Json(
            HttpStatusCode.OK,
            "{\"isSuccess\":true,\"value\":{\"isValid\":true,\"userId\":\"u-1\",\"roles\":[\"SystemAdmin\"]}}"));

        var result = await CreateClient(handler).ValidateAsync(token);

        result.IsValid.Should().BeTrue();
        result.UserId.Should().Be("u-1");
        result.TenantId.Should().Be("t-1");
        result.OrganizationId.Should().Be(organizationId);
        result.Roles.Should().Equal("SystemAdmin");
        result.Permissions.Should().Equal("OrganizationStructure.Employee.View");
    }

    /// <summary>
    /// رد کلاینت سمت IAM باید به شکست امن تبدیل شود.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_ClientRejected_ShouldFailClosed()
    {
        var handler = new StubHandler(_ => Json(
            HttpStatusCode.Unauthorized,
            "{\"error\":\"invalid_client\",\"message\":\"کلاینت نامعتبر است.\"}"));

        var result = await CreateClient(handler).ValidateAsync("token");

        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// توکن خالی باید بدون فراخوانی شبکه بی‌اعتبار اعلام شود.
    /// </summary>
    [Fact]
    public async Task ValidateAsync_EmptyToken_ShouldNotCallIam()
    {
        var handler = new StubHandler(_ => Json(HttpStatusCode.OK, "{\"isSuccess\":true}"));

        var result = await CreateClient(handler).ValidateAsync(" ");

        result.IsValid.Should().BeFalse();
        handler.LastRequest.Should().BeNull();
    }
}