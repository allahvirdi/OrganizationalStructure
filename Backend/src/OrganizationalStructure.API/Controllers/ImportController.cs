using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.API.Security;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// ورود ساختار سازمانی از فایل اکسل.
/// </summary>
/// <remarks>
/// مجوز: <c>OrganizationStructure.Post.Create</c> (ایجاد پست از فایل).
/// </remarks>
[Route("api/v1/import")]
public sealed class ImportController : ApiControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public ImportController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// بارگذاری ساختار پست‌های یک سازمان از فایل اکسل.
    /// </summary>
    /// <param name="organizationId">شناسه سازمان مقصد</param>
    /// <param name="file">فایل اکسل .xlsx</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpPost("posts")]
    [Authorize(Policy = AuthorizationPolicies.Post.Create)]
    [ProducesResponseType(typeof(ImportPostsResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ImportPostsResultDto>> ImportPosts(
        [FromForm] Guid organizationId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Import.InvalidFile",
                Detail = "فایل اکسل الزامی است.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Import.InvalidFile",
                Detail = "فقط فایل‌های با پسوند .xlsx مجاز هستند.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var parser = new PostsExcelParser();
        using var stream = file.OpenReadStream();
        var parseResult = parser.Parse(stream);

        if (parseResult.IsFailure)
        {
            return HandleResult(Result<ImportPostsResultDto>.Failure(parseResult.Error!));
        }

        var command = new ImportPostsCommand(organizationId, parseResult.Value!);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// دانلود قالب نمونه فایل اکسل با هدر فارسی.
    /// </summary>
    [HttpGet("posts/template")]
    [Authorize(Policy = AuthorizationPolicies.Post.Create)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public ActionResult GetTemplate()
    {
        var templateBytes = PostsExcelParser.BuildTemplate();
        return File(
            templateBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "قالب-ساختار-پست‌ها.xlsx");
    }
}