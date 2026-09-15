using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// کنترلر پایه API با نگاشت یکپارچه <see cref="Result"/> به پاسخ HTTP.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// نگاشت نتیجه بدون مقدار به پاسخ HTTP.
    /// </summary>
    /// <param name="result">نتیجه عملیات</param>
    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return ToProblem(result.Error!);
    }

    /// <summary>
    /// نگاشت نتیجه دارای مقدار به پاسخ HTTP.
    /// </summary>
    /// <typeparam name="T">نوع مقدار</typeparam>
    /// <param name="result">نتیجه عملیات</param>
    /// <param name="createdAction">تابع ساخت پاسخ ۲۰۱ در حالت ایجاد (اختیاری)</param>
    protected ActionResult<T> HandleResult<T>(Result<T> result, Func<T, ActionResult<T>>? createdAction = null)
    {
        if (result.IsSuccess)
        {
            return createdAction is null
                ? Ok(result.Value)
                : createdAction(result.Value!);
        }

        return ToProblem(result.Error!);
    }

    private ObjectResult ToProblem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return StatusCode(statusCode, new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = statusCode
        });
    }
}