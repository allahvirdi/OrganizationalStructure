using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Authorities.AssignAuthority;
using OrganizationalStructure.Application.Authorities.DTOs;
using OrganizationalStructure.Application.Authorities.Queries;
using OrganizationalStructure.Application.Authorities.ManageAuthorities;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// مدیریت اختیارهای سازمانی و انتساب به پست‌ها.
/// </summary>
/// <remarks>
/// هر Endpoint با Policy متناظر از کاتالوگ (DEC-025) محافظت می‌شود.
/// </remarks>
[Route("api/v1/authorities")]
public sealed class AuthoritiesController : ApiControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AuthoritiesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// تعریف اختیار جدید.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Authority.Create)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateAuthorityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result, id => CreatedAtAction(nameof(GetByCode), new { code = command.Code.Trim() }, id));
    }

    /// <summary>
    /// ویرایش اختیار.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Authority.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateAuthorityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateAuthorityCommand(id, request.Title, request.Description),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// غیرفعال‌سازی اختیار.
    /// </summary>
    [HttpPatch("{id:guid}/disable")]
    [Authorize(Policy = AuthorizationPolicies.Authority.Disable)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Disable(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DisableAuthorityCommand(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت اختیار با کد.
    /// </summary>
    [HttpGet("{code}")]
    [Authorize(Policy = AuthorizationPolicies.Authority.View)]
    [ProducesResponseType(typeof(AuthorityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorityDto>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAuthorityByCodeQuery(code), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// جستجوی صفحه‌بندی‌شده اختیارها.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Authority.View)]
    [ProducesResponseType(typeof(PagedResult<AuthorityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuthorityDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new SearchAuthoritiesQuery(searchTerm, isActive, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// انتساب اختیار به پست (Code-based).
    /// </summary>
    [HttpPost("assignments")]
    [Authorize(Policy = AuthorizationPolicies.Authority.Assign)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Assign(
        [FromBody] AssignAuthorityCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result, id => CreatedAtAction(nameof(GetByCode), new { code = command.AuthorityCode.Trim() }, id));
    }

    /// <summary>
    /// پایان دادن به انتساب اختیار (بدون حذف فیزیکی).
    /// </summary>
    [HttpPost("assignments/{assignmentId:guid}/end")]
    [Authorize(Policy = AuthorizationPolicies.Authority.EndAssignment)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> EndAssignment(
        Guid assignmentId,
        [FromBody] EndAuthorityAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new EndAuthorityAssignmentCommand(assignmentId, request.EndDate),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت انتساب‌های یک اختیار.
    /// </summary>
    [HttpGet("{code}/assignments")]
    [Authorize(Policy = AuthorizationPolicies.Authority.View)]
    [ProducesResponseType(typeof(IReadOnlyList<AuthorityAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AuthorityAssignmentDto>>> GetAssignments(
        string code,
        [FromQuery] bool onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetAuthorityAssignmentsQuery(code, onlyActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت اختیارهای منتسب به پست.
    /// </summary>
    [HttpGet("/api/v1/posts/{postId:guid}/authorities")]
    [Authorize(Policy = AuthorizationPolicies.Authority.View)]
    [ProducesResponseType(typeof(IReadOnlyList<AuthorityAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AuthorityAssignmentDto>>> GetPostAuthorities(
        Guid postId,
        [FromQuery] bool onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetPostAuthoritiesQuery(postId, onlyActive), cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// بدنه درخواست ویرایش اختیار.
/// </summary>
public sealed record UpdateAuthorityRequest(string Title, string? Description);

/// <summary>
/// بدنه درخواست پایان انتساب اختیار.
/// </summary>
/// <param name="EndDate">تاریخ پایان</param>
public sealed record EndAuthorityAssignmentRequest(DateOnly EndDate);
