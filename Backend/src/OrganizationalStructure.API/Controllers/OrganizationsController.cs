using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Application.Organizations.DTOs;
using OrganizationalStructure.Application.Organizations.GetOrganizationOptions;
using OrganizationalStructure.API.Security;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// واحدهای سازمانی قابل انتخاب کاربر (Reference از IAM — ADR-002).
/// </summary>
/// <remarks>
/// پاسخ این Endpoint به «شناسه خام» بسنده نمی‌کند و نام سازمان را برمی‌گرداند تا فرم‌ها
/// بتوانند سازمان را با «نام» و به‌صورت جستجوپذیر نمایش دهند.
/// </remarks>
[Route("api/v1/organizations")]
public sealed class OrganizationsController : ApiControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="sender">فرستنده MediatR</param>
    public OrganizationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// فهرست سازمان‌های قابل انتخاب کاربر جاری (خود سازمان + زیرمجموعه‌ها).
    /// </summary>
    /// <remarks>
    /// خروجی هرگز از محدوده سازمانی کاربر (Scope نشست، محاسبه‌شده از درخت IAM) فراتر نمی‌رود؛
    /// بنابراین فقط «احراز هویت» لازم است و Permission دامنه‌ای جدا نمی‌خواهد
    /// (Deny by Default با Fallback Policy حفظ می‌شود).
    /// </remarks>
    /// <param name="searchTerm">عبارت جستجو روی نام/کد سازمان (اختیاری)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<OrganizationOptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<OrganizationOptionDto>>> GetOptions(
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOrganizationOptionsQuery(searchTerm), cancellationToken);
        return HandleResult(result);
    }
}
