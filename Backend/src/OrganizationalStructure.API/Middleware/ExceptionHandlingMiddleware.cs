using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.API.Middleware;

/// <summary>
/// میان‌افزار مدیریت استثنا؛ تمام خطاها را به <c>Problem Details</c> یکپارچه (RFC 7807) تبدیل می‌کند.
/// </summary>
/// <remarks>
/// با پشتیبانی از <see cref="Error"/>/خطاهای اعتبارسنجی و خطاهای پیش‌بینی‌نشده (Internal).
/// خطای داخلی به‌گونه‌ای پوشانده می‌شود که جزئیات سرور یا PII منتشر نشود (OWASP - عدم افشا).
/// </remarks>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// مقداردهی اولیه میان‌افزار.
    /// </summary>
    /// <param name="next">درخواست بعدی در خط لوله</param>
    /// <param name="logger">ثبت‌کننده</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// اجرای میان‌افزار برای درخواست جاری.
    /// </summary>
    /// <param name="context">زمینه HTTP</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "خطا در پردازش درخواست",
            Status = (int)HttpStatusCode.InternalServerError
        };

        switch (exception)
        {
            case KeyNotFoundException notFound:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "رکورد یافت نشد";
                problemDetails.Detail = notFound.Message;
                break;

            case InvalidOperationException operation:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "عملیات نامعتبر";
                problemDetails.Detail = operation.Message;
                break;

            case UnauthorizedAccessException unauthorized:
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "دسترسی غیرمجاز";
                problemDetails.Detail = unauthorized.Message;
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred: {Message}",
                    Sanitize(exception.Message));
                problemDetails.Title = "خطای داخلی سرور";
                problemDetails.Detail = "خطایی پیش‌بینی‌نشده رخ داده است.";
                break;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    /// <summary>
    /// پاک‌سازی پیام خطا (حذف مقادیر حساس/توکن‌ها) برای لاگ.
    /// </summary>
    /// <param name="message">پیام خام</param>
    /// <returns>پیام پاک‌شده</returns>
    private static string Sanitize(string message)
    {
        return message.Length > 1000
            ? message[..1000]
            : message;
    }
}

/// <summary>
/// متد الحاقی برای ثبت میان‌افزار خطا به خط لوله.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// افزودن میان‌افزار مدیریت خطای یکپارچه به خط لوله.
    /// </summary>
    /// <param name="app">برنامه</param>
    /// <returns>برنامه</returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}