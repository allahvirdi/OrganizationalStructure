using Microsoft.AspNetCore.Authentication;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// نام Schemeها و انتخاب‌گر هوشمند احراز هویت.
/// </summary>
public static class AuthenticationSchemes
{
    /// <summary>
    /// Scheme کوکی نشست BFF (مرورگر).
    /// </summary>
    public const string Bff = "Bff";

    /// <summary>
    /// Scheme توکن Bearer مصرف‌کننده‌های ماشینی.
    /// </summary>
    public const string IamBearer = "IamBearer";

    /// <summary>
    /// Scheme هوشمند پیش‌فرض: با هدر Bearer به توکن، وگرنه به کوکی BFF.
    /// </summary>
    public const string Smart = "Smart";

    /// <summary>
    /// انتخاب Scheme بر اساس درخواست.
    /// </summary>
    /// <param name="context">زمینه HTTP</param>
    /// <returns>نام Scheme مناسب</returns>
    public static string SelectScheme(Microsoft.AspNetCore.Http.HttpContext context)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return IamBearer;
        }

        return Bff;
    }
}