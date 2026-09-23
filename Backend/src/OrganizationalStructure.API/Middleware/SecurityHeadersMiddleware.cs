using Microsoft.AspNetCore.Mvc;

namespace OrganizationalStructure.API.Middleware;

/// <summary>
/// میان‌افزار افزودن هدرهای امنیتی به تمام پاسخ‌های HTTP.
/// </summary>
/// <remarks>
/// شامل هدرهای استاندارد امنیتی مطابق توصیه‌های OWASP Secure Headers Project.
/// </remarks>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// مقداردهی اولیه میان‌افزار.
    /// </summary>
    /// <param name="next">درخواست بعدی در خط لوله</param>
    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// اجرای میان‌افزار: افزودن هدرهای امنیتی به پاسخ.
    /// </summary>
    /// <param name="context">زمینه HTTP</param>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["X-XSS-Protection"] = "0";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
            headers["Content-Security-Policy"] =
                "default-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";

            // حذف هدرهای افشاکننده اطلاعات سرور
            headers.Remove("Server");
            headers.Remove("X-Powered-By");

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

/// <summary>
/// متد الحاقی برای ثبت میان‌افزار هدرهای امنیتی در خط لوله.
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>
    /// افزودن میان‌افزار هدرهای امنیتی به خط لوله.
    /// </summary>
    /// <param name="app">برنامه</param>
    /// <returns>برنامه</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}