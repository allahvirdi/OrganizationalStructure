using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Infrastructure.Integration;

/// <summary>
/// کلاینت سروربه‌سرور سامانه هویت با قراردادهای واقعی IAM.
/// </summary>
/// <remarks>
/// شکل endpointها و DTOها از کد واقعی IAM استخراج شده است:
/// <list type="bullet">
/// <item><c>POST /api/auth/login</c> و <c>POST /api/auth/login/verify-mfa</c> با پوشش <c>Result&lt;LoginResponse&gt;</c>.</item>
/// <item><c>POST /api/token/{validate,refresh,revoke}</c> با پوشش <c>Result&lt;T&gt;</c> و الزام هدرهای <c>X-Client-Id/X-Client-Secret</c>.</item>
/// <item>پوشش پاسخ: <c>{ IsSuccess, Value, Error }</c>.</item>
/// </list>
/// هر خطای انتقالی به نتیجه ناموفق تبدیل می‌شود (fail-closed)؛ هیچ استثنایی به بیرون درز نمی‌کند.
/// </remarks>
public sealed class IamClient : IIamClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly IamOptions _options;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="httpClientFactory">کارخانه HttpClient</param>
    /// <param name="options">تنظیمات IAM</param>
    public IamClient(IHttpClientFactory httpClientFactory, IOptions<IamOptions> options)
    {
        _options = options.Value;

        _http = string.IsNullOrWhiteSpace(_options.BaseAddress)
            ? httpClientFactory.CreateClient()
            : httpClientFactory.CreateClient("iam");

        if (!string.IsNullOrWhiteSpace(_options.BaseAddress))
        {
            _http.BaseAddress = new Uri(_options.BaseAddress.TrimEnd('/') + "/");
        }

        _http.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds <= 0 ? 10 : _options.TimeoutSeconds);
    }

    /// <inheritdoc />
    public async Task<IamLoginResult> LoginAsync(
        IamLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            request.UserName,
            Email = (string?)null,
            request.Password,
            request.RememberMe,
            CaptchaId = (string?)null,
            CaptchaInput = (string?)null,
            ClientId = string.IsNullOrWhiteSpace(_options.ClientId) ? null : _options.ClientId
        };

        var envelope = await PostEnvelopeAsync<IamLoginResponseBody>(
            _options.LoginPath, payload, includeClientHeaders: false, cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null)
        {
            return new IamLoginResult
            {
                IsSuccess = false,
                Error = envelope?.Error ?? "خطا در ارتباط با سامانه هویت."
            };
        }

        var value = envelope.Value;
        return new IamLoginResult
        {
            IsSuccess = true,
            AccessToken = value.AccessToken,
            RefreshToken = value.RefreshToken,
            RequiresMfa = value.RequiresMfa,
            MfaChallengeToken = value.MfaChallengeToken
        };
    }

    /// <inheritdoc />
    public async Task<IamLoginResult> VerifyMfaAsync(
        IamMfaRequest request,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            request.MfaChallengeToken,
            request.Code,
            request.RememberMe,
            ClientId = string.IsNullOrWhiteSpace(_options.ClientId) ? null : _options.ClientId
        };

        var envelope = await PostEnvelopeAsync<IamLoginResponseBody>(
            _options.VerifyMfaPath, payload, includeClientHeaders: false, cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null)
        {
            return new IamLoginResult
            {
                IsSuccess = false,
                Error = envelope?.Error ?? "خطا در ارتباط با سامانه هویت."
            };
        }

        var value = envelope.Value;
        return new IamLoginResult
        {
            IsSuccess = true,
            AccessToken = value.AccessToken,
            RefreshToken = value.RefreshToken,
            RequiresMfa = value.RequiresMfa,
            MfaChallengeToken = value.MfaChallengeToken
        };
    }

    /// <inheritdoc />
    public async Task<IamValidationResult> ValidateAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new IamValidationResult { IsValid = false, Error = "توکن نامعتبر است." };
        }

        var envelope = await PostEnvelopeAsync<IamValidateResponseBody>(
            _options.ValidatePath,
            new { Token = accessToken },
            includeClientHeaders: true,
            cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null || !envelope.Value.IsValid)
        {
            return new IamValidationResult
            {
                IsValid = false,
                Error = envelope?.Error ?? envelope?.Value?.Error ?? "توکن نامعتبر است."
            };
        }

        var value = envelope.Value;
        var extended = ReadExtendedClaims(accessToken);

        return new IamValidationResult
        {
            IsValid = true,
            UserId = value.UserId,
            TenantId = extended.TenantId,
            OrganizationId = extended.OrganizationId,
            Roles = value.Roles,
            Permissions = extended.Permissions
        };
    }

    /// <inheritdoc />
    public async Task<IamTokenResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new IamTokenResult { IsSuccess = false, Error = "توکن بازآوری نامعتبر است." };
        }

        var envelope = await PostEnvelopeAsync<IamTokenResponseBody>(
            _options.RefreshPath,
            new { RefreshToken = refreshToken },
            includeClientHeaders: true,
            cancellationToken);

        if (envelope is null || !envelope.IsSuccess || envelope.Value is null)
        {
            return new IamTokenResult
            {
                IsSuccess = false,
                Error = envelope?.Error ?? "خطا در تجدید توکن."
            };
        }

        return new IamTokenResult
        {
            IsSuccess = true,
            AccessToken = envelope.Value.AccessToken,
            RefreshToken = envelope.Value.RefreshToken
        };
    }

    /// <inheritdoc />
    public async Task<bool> RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var envelope = await PostEnvelopeAsync<bool?>(
            _options.RevokePath,
            new { RefreshToken = refreshToken },
            includeClientHeaders: true,
            cancellationToken);

        return envelope is not null && envelope.IsSuccess && envelope.Value == true;
    }

    private async Task<IamEnvelope<T>?> PostEnvelopeAsync<T>(
        string path,
        object payload,
        bool includeClientHeaders,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(payload, options: JsonOptions)
            };

            if (includeClientHeaders && !string.IsNullOrWhiteSpace(_options.ClientId))
            {
                request.Headers.Add("X-Client-Id", _options.ClientId);
                request.Headers.Add("X-Client-Secret", _options.ClientSecret);
            }

            using var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<IamEnvelope<T>>(
                JsonOptions, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    /// <summary>
    /// خواندن ادعاهای تکمیلی (مستأجر/سازمان/دسترسی‌ها) از بدنه JWT پس از اعتبارسنجی موفق سمت IAM.
    /// </summary>
    /// <remarks>
    /// نام Claimها از ثابت‌های IAM و اسناد توکن استخراج شده است
    /// (<c>user_id</c>، <c>tenant_id</c>، <c>organization_id</c>، <c>role</c>، <c>permission</c>).
    /// </remarks>
    private static (string? TenantId, string? OrganizationId, IReadOnlyList<string> Permissions)
        ReadExtendedClaims(string accessToken)
    {
        try
        {
            var parts = accessToken.Split('.');
            if (parts.Length < 2)
            {
                return (null, null, Array.Empty<string>());
            }

            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            using var document = JsonDocument.Parse(payloadJson);
            var root = document.RootElement;

            return (
                GetStringClaim(root, "tenant_id"),
                GetStringClaim(root, "organization_id"),
                GetArrayClaim(root, "permission"));
        }
        catch (Exception ex) when (ex is FormatException or JsonException or ArgumentException)
        {
            return (null, null, Array.Empty<string>());
        }
    }

    private static string? GetStringClaim(JsonElement root, string name)
    {
        if (root.TryGetProperty(name, out var element) && element.ValueKind == JsonValueKind.String)
        {
            return element.GetString();
        }

        return null;
    }

    private static IReadOnlyList<string> GetArrayClaim(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var element))
        {
            return Array.Empty<string>();
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            var single = element.GetString();
            return string.IsNullOrWhiteSpace(single)
                ? Array.Empty<string>()
                : new[] { single };
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            return element.EnumerateArray()
                .Where(e => e.ValueKind == JsonValueKind.String)
                .Select(e => e.GetString()!)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }

        return Array.Empty<string>();
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var output = input.Replace('-', '+').Replace('_', '/');
        switch (output.Length % 4)
        {
            case 2: output += "=="; break;
            case 3: output += "="; break;
        }

        return Convert.FromBase64String(output);
    }

    /// <summary>
    /// پوشش پاسخ IAM به شکل { IsSuccess, Value, Error }.
    /// </summary>
    /// <typeparam name="T">نوع مقدار</typeparam>
    private sealed class IamEnvelope<T>
    {
        /// <summary>
        /// موفقیت عملیات.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// مقدار پاسخ.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// پیام خطا.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// شکل پاسخ ورود IAM (زیرمجموعه موردنیاز BFF).
    /// </summary>
    private sealed class IamLoginResponseBody
    {
        /// <summary>
        /// توکن دسترسی.
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// توکن بازآوری.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// نیاز به دومرحله‌ای.
        /// </summary>
        public bool RequiresMfa { get; set; }

        /// <summary>
        /// توکن چالش دومرحله‌ای.
        /// </summary>
        public string? MfaChallengeToken { get; set; }
    }

    /// <summary>
    /// شکل پاسخ اعتبارسنجی توکن IAM.
    /// </summary>
    private sealed class IamValidateResponseBody
    {
        /// <summary>
        /// معتبر بودن.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// شناسه کاربر.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// نقش‌ها.
        /// </summary>
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// خطا.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// شکل پاسخ عملیات توکن IAM.
    /// </summary>
    private sealed class IamTokenResponseBody
    {
        /// <summary>
        /// توکن دسترسی.
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// توکن بازآوری.
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}