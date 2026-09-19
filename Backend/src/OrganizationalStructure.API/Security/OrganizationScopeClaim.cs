using System.Text.Json;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// سریال‌سازی و بازخوانی مرجع سازمان در Claimهای نشست (کانال انتقال «نشست ← کاربر جاری»).
/// </summary>
/// <remarks>
/// هر Claim یک مرجع سازمان را به‌صورت JSON نگه می‌دارد تا نویسه‌های خاص در «نام» سازمان
/// (مانند «|») شکست ایجاد نکند.
/// </remarks>
public static class OrganizationScopeClaim
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// تبدیل مرجع سازمان به مقدار Claim.
    /// </summary>
    /// <param name="reference">مرجع سازمان</param>
    /// <returns>مقدار Claim</returns>
    public static string Serialize(OrganizationReference reference) =>
        JsonSerializer.Serialize(reference, Options);

    /// <summary>
    /// بازخوانی مرجع سازمان از مقدار Claim.
    /// </summary>
    /// <param name="value">مقدار Claim</param>
    /// <returns>مرجع سازمان یا <c>null</c> در صورت خالی/نامعتبر بودن</returns>
    public static OrganizationReference? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<OrganizationReference>(value, Options);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}