using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Application.Import.Staging;
using OrganizationalStructure.Application.Import.Staging.DTOs;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Domain.Enums;

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

    /// <summary>
    /// بارگذاری دسته‌جمعی پرسنل یک سازمان از فایل اکسل یا CSV.
    /// </summary>
    /// <param name="organizationId">شناسه سازمان مقصد</param>
    /// <param name="file">فایل .xlsx یا .csv</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpPost("employees")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(ImportEmployeesResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ImportEmployeesResultDto>> ImportEmployees(
        [FromForm] Guid organizationId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Import.InvalidFile",
                Detail = "فایل الزامی است.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".csv")
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Import.InvalidFile",
                Detail = "فقط فایل‌های با پسوند .xlsx یا .csv مجاز هستند.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        Result<IReadOnlyList<ImportEmployeeRowDto>> parseResult;
        using var stream = file.OpenReadStream();

        if (extension == ".xlsx")
        {
            parseResult = new EmployeesExcelParser().Parse(stream);
        }
        else
        {
            parseResult = new EmployeesCsvParser().Parse(stream);
        }

        if (parseResult.IsFailure)
        {
            return HandleResult(Result<ImportEmployeesResultDto>.Failure(parseResult.Error!));
        }

        var command = new ImportEmployeesCommand(organizationId, parseResult.Value!);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// دانلود قالب نمونه پرسنل (اکسل یا CSV).
    /// </summary>
    /// <param name="format">قالب مورد انتظار: xlsx یا csv</param>
    [HttpGet("employees/template")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public ActionResult GetEmployeesTemplate([FromQuery] string format = "xlsx")
    {
        if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            return File(
                EmployeesCsvParser.BuildTemplate(),
                "text/csv; charset=utf-8",
                "قالب-پرسنل.csv");
        }

        return File(
            EmployeesExcelParser.BuildTemplate(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "قالب-پرسنل.xlsx");
    }

    /// <summary>
    /// بارگذاری فایل پرسنل در جدول واسط (بازبینی قبل از ثبت نهایی).
    /// </summary>
    /// <param name="organizationId">شناسه سازمان مقصد</param>
    /// <param name="file">فایل .xlsx یا .csv</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpPost("employees/staging/upload")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(StagingUploadResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<StagingUploadResultDto>> UploadToStaging(
        [FromForm] Guid organizationId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Import.InvalidFile",
                Detail = "فایل الزامی است.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".csv")
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Import.InvalidFile",
                Detail = "فقط فایل‌های با پسوند .xlsx یا .csv مجاز هستند.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        Result<IReadOnlyList<ImportEmployeeRowDto>> parseResult;
        using var stream = file.OpenReadStream();

        if (extension == ".xlsx")
        {
            parseResult = new EmployeesExcelParser().Parse(stream);
        }
        else
        {
            parseResult = new EmployeesCsvParser().Parse(stream);
        }

        if (parseResult.IsFailure)
        {
            return HandleResult(Result<StagingUploadResultDto>.Failure(parseResult.Error!));
        }

        var command = new UploadEmployeesToStagingCommand(organizationId, file.FileName, parseResult.Value!);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// ایجاد بارگذاری بیرونی (مسیر درج مستقیم سامانه بیرونی).
    /// </summary>
    /// <param name="organizationId">شناسه سازمان مقصد</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpPost("employees/staging/batches")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateExternalBatch(
        [FromForm] Guid organizationId,
        CancellationToken cancellationToken)
    {
        var command = new CreateExternalBatchCommand(organizationId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status201Created, new { batchId = result.Value });
        }

        return HandleResult((Result)result);
    }

    /// <summary>
    /// اعلام آماده بودن بارگذاری بیرونی پس از درج ردیف‌ها.
    /// </summary>
    /// <param name="batchId">شناسه بارگذاری</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpPost("employees/staging/{batchId:guid}/ready")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(StagingUploadResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<StagingUploadResultDto>> MarkReady(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var command = new MarkBatchReadyCommand(batchId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// فهرست بارگذاری‌های واسط (صفحه‌بندی‌شده با فیلتر سازمان/وضعیت).
    /// </summary>
    /// <param name="organizationId">فیلتر سازمان (اختیاری)</param>
    /// <param name="status">فیلتر وضعیت: ۱=در انتظار درج، ۲=آماده، ۳=ثبت‌شده، ۴=ردشده (اختیاری)</param>
    /// <param name="page">شماره صفحه (از ۱)</param>
    /// <param name="pageSize">اندازه صفحه (حداکثر ۱۰۰)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpGet("employees/staging/batches")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(PagedResult<ImportBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<ImportBatchDto>>> ListBatches(
        [FromQuery] Guid? organizationId,
        [FromQuery] int? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new ListImportBatchesQuery(organizationId, (ImportBatchStatus?)status, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// ردیف‌های یک بارگذاری واسط (صفحه‌بندی‌شده) همراه خطاهای اعتبارسنجی.
    /// </summary>
    /// <param name="batchId">شناسه بارگذاری</param>
    /// <param name="page">شماره صفحه (از ۱)</param>
    /// <param name="pageSize">اندازه صفحه (حداکثر ۱۰۰)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [HttpGet("employees/staging/{batchId:guid}/rows")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Import)]
    [ProducesResponseType(typeof(PagedResult<StagingRowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<StagingRowDto>>> GetBatchRows(
        Guid batchId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetStagingRowsQuery(batchId, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }
}