namespace OrganizationalStructure.Application.Integration.Iam;

/// <summary>
/// درخواست ورود به سامانه هویت (IAM).
/// </summary>
public sealed record IamLoginRequest
{
    /// <summary>
    /// نام کاربری.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// رمز عبور.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// نشست ماندگار.
    /// </summary>
    public bool RememberMe { get; init; }
}

/// <summary>
/// نتیجه عملیات ورود به سامانه هویت.
/// </summary>
public sealed record IamLoginResult
{
    /// <summary>
    /// موفقیت عملیات.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// پیام خطا در صورت شکست.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// توکن دسترسی (فقط سمت سرور نگهداری می‌شود).
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// توکن بازآوری (فقط سمت سرور نگهداری می‌شود).
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// نیاز به تکمیل ورود با کد دومرحله‌ای.
    /// </summary>
    public bool RequiresMfa { get; init; }

    /// <summary>
    /// توکن کوتاه‌عمر مرحله دومرحله‌ای.
    /// </summary>
    public string? MfaChallengeToken { get; init; }
}

/// <summary>
/// درخواست تکمیل ورود با کد دومرحله‌ای.
/// </summary>
public sealed record IamMfaRequest
{
    /// <summary>
    /// توکن کوتاه‌عمر مرحله اول.
    /// </summary>
    public string MfaChallengeToken { get; init; } = string.Empty;

    /// <summary>
    /// کد تأیید.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// نشست ماندگار.
    /// </summary>
    public bool RememberMe { get; init; }
}

/// <summary>
/// نتیجه اعتبارسنجی توکن دسترسی در سامانه هویت.
/// </summary>
public sealed record IamValidationResult
{
    /// <summary>
    /// معتبر بودن توکن.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// شناسه کاربر.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// شناسه مستأجر.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// شناسه سازمان.
    /// </summary>
    public string? OrganizationId { get; init; }

    /// <summary>
    /// نقش‌های کاربر.
    /// </summary>
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>
    /// دسترسی‌های کاربر (Permissionها).
    /// </summary>
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// نام کاربر (Claim اختیاری first_name توکن IAM؛ برای نمایش در UI نگه‌داری می‌شود — داده مرجع، نه Master).
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// نام خانوادگی کاربر (Claim اختیاری last_name توکن IAM؛ برای نمایش در UI نگه‌داری می‌شود — داده مرجع، نه Master).
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// پیام خطا.
    /// </summary>
    public string? Error { get; init; }
}

/// <summary>
/// نتیجه تجدید توکن.
/// </summary>
public sealed record IamTokenResult
{
    /// <summary>
    /// موفقیت عملیات.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// پیام خطا.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// توکن دسترسی جدید.
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// توکن بازآوری جدید.
    /// </summary>
    public string? RefreshToken { get; init; }
}

/// <summary>
/// کلاینت سروربه‌سرور سامانه هویت (فقط سمت سرور؛ مرورگر هرگز توکن IAM را نمی‌بیند).
/// </summary>
public interface IIamClient
{
    /// <summary>
    /// ورود با نام کاربری/رمز عبور.
    /// </summary>
    /// <param name="request">درخواست ورود</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه ورود</returns>
    Task<IamLoginResult> LoginAsync(IamLoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// تکمیل ورود با کد دومرحله‌ای.
    /// </summary>
    /// <param name="request">درخواست تکمیل ورود</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه ورود</returns>
    Task<IamLoginResult> VerifyMfaAsync(IamMfaRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// اعتبارسنجی توکن دسترسی در سامانه هویت.
    /// </summary>
    /// <param name="accessToken">توکن دسترسی</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه اعتبارسنجی</returns>
    Task<IamValidationResult> ValidateAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// تجدید توکن دسترسی با توکن بازآوری.
    /// </summary>
    /// <param name="refreshToken">توکن بازآوری</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه تجدید</returns>
    Task<IamTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// ابطال توکن بازآوری.
    /// </summary>
    /// <param name="refreshToken">توکن بازآوری</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>درست در صورت ابطال موفق</returns>
    Task<bool> RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درخت سازمان‌ها از IAM (با توکن کاربر).
    /// </summary>
    /// <param name="accessToken">توکن دسترسی کاربر</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>ریشه‌های درخت یا خالی در صورت شکست (fail-closed در مصرف‌کننده)</returns>
    Task<IReadOnlyList<IamOrganizationNode>> GetOrganizationTreeAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// گره درخت سازمان IAM (زیرمجموعه موردنیاز Scope).
/// </summary>
public sealed record IamOrganizationNode
{
    /// <summary>
    /// شناسه واحد سازمانی.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// نام نمایشی واحد سازمانی (از DTO درخت IAM).
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// کد واحد سازمانی (از DTO درخت IAM).
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// واحدهای زیرمجموعه.
    /// </summary>
    public List<IamOrganizationNode> Children { get; init; } = new();
}