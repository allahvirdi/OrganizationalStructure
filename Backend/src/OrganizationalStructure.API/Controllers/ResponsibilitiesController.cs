using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Responsibilities.AssignResponsibility;
using OrganizationalStructure.Application.Responsibilities.CreateResponsibility;
using OrganizationalStructure.Application.Responsibilities.DisableResponsibility;
using OrganizationalStructure.Application.Responsibilities.DTOs;
using OrganizationalStructure.Application.Responsibilities.EndResponsibilityAssignment;
using OrganizationalStructure.Application.Responsibilities.GetPostResponsibilities;
using OrganizationalStructure.Application.Responsibilities.GetResponsibilityAssignments;
using OrganizationalStructure.Application.Responsibilities.GetResponsibilityByCode;
using OrganizationalStructure.Application.Responsibilities.SearchResponsibilities;
using OrganizationalStructure.Application.Responsibilities.UpdateResponsibility;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// مدیریت مسئولیت‌های سازمانی و انتساب به پست‌ها.
/// </summary>
/// <remarks>
/// هر Endpoint با Policy متناظر از کاتالوگ (DEC-025) محافظت می‌شود.
/// </remarks>
[Route("api/v1/responsibilities")]
public sealed class ResponsibilitiesController : ApiControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public ResponsibilitiesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// تعریف مسئولیت جدید.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.Create)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateResponsibilityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result, id => CreatedAtAction(nameof(GetByCode), new { code = command.Code.Trim() }, id));
    }

    /// <summary>
    /// ویرایش مسئولیت.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateResponsibilityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateResponsibilityCommand(id, request.Title, request.Description),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// غیرفعال‌سازی مسئولیت.
    /// </summary>
    [HttpPatch("{id:guid}/disable")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.Disable)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Disable(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DisableResponsibilityCommand(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت مسئولیت با کد.
    /// </summary>
    [HttpGet("{code}")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.View)]
    [ProducesResponseType(typeof(ResponsibilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponsibilityDto>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetResponsibilityByCodeQuery(code), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// جستجوی صفحه‌بندی‌شده مسئولیت‌ها.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.View)]
    [ProducesResponseType(typeof(PagedResult<ResponsibilityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ResponsibilityDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new SearchResponsibilitiesQuery(searchTerm, isActive, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// انتساب مسئولیت به پست (Code-based).
    /// </summary>
    [HttpPost("assignments")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.Assign)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Assign(
        [FromBody] AssignResponsibilityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result, id => CreatedAtAction(nameof(GetByCode), new { code = command.ResponsibilityCode.Trim() }, id));
    }

    /// <summary>
    /// پایان دادن به انتساب مسئولیت (بدون حذف فیزیکی).
    /// </summary>
    [HttpPost("assignments/{assignmentId:guid}/end")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.EndAssignment)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> EndAssignment(
        Guid assignmentId,
        [FromBody] EndResponsibilityAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new EndResponsibilityAssignmentCommand(assignmentId, request.EndDate),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت انتساب‌های یک مسئولیت.
    /// </summary>
    [HttpGet("{code}/assignments")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.View)]
    [ProducesResponseType(typeof(IReadOnlyList<ResponsibilityAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ResponsibilityAssignmentDto>>> GetAssignments(
        string code,
        [FromQuery] bool onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetResponsibilityAssignmentsQuery(code, onlyActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت مسئولیت‌های منتسب به پست.
    /// </summary>
    [HttpGet("/api/v1/posts/{postId:guid}/responsibilities")]
    [Authorize(Policy = AuthorizationPolicies.Responsibility.View)]
    [ProducesResponseType(typeof(IReadOnlyList<ResponsibilityAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ResponsibilityAssignmentDto>>> GetPostResponsibilities(
        Guid postId,
        [FromQuery] bool onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetPostResponsibilitiesQuery(postId, onlyActive), cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// بدنه درخواست ویرایش مسئولیت.
/// </summary>
public sealed record UpdateResponsibilityRequest(string Title, string? Description);

/// <summary>
/// بدنه درخواست پایان انتساب مسئولیت.
/// </summary>
/// <param name="EndDate">تاریخ پایان</param>
public sealed record EndResponsibilityAssignmentRequest(DateOnly EndDate);
