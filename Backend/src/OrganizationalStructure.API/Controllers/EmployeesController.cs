using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Employees.AssignPost;
using OrganizationalStructure.Application.Employees.CreateEmployee;
using OrganizationalStructure.Application.Employees.DTOs;
using OrganizationalStructure.Application.Employees.EndAssignment;
using OrganizationalStructure.Application.Employees.GetEmployeeById;
using OrganizationalStructure.Application.Employees.GetEmployeePosts;
using OrganizationalStructure.Application.Employees.GetPostEmployees;
using OrganizationalStructure.Application.Employees.LinkEmployeeUser;
using OrganizationalStructure.Application.Employees.SearchEmployees;
using OrganizationalStructure.Application.Employees.SetEmployeeStatus;
using OrganizationalStructure.Application.Employees.UpdateEmployee;
using OrganizationalStructure.Application.Employees.UpdateSupplementary;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// مدیریت پرسنل و انتساب به پست‌ها.
/// </summary>
/// <remarks>
/// هر Endpoint با Policy متناظر از کاتالوگ (DEC-025) محافظت می‌شود.
/// </remarks>
[Route("api/v1/employees")]
public sealed class EmployeesController : ApiControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public EmployeesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// ثبت پرسنل جدید.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Employee.Create)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result, id => CreatedAtAction(nameof(GetById), new { id }, id));
    }

    /// <summary>
    /// ویرایش اطلاعات پرسنلی.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateEmployeeCommand(id, request.FirstName, request.LastName, request.NationalCode, request.Mobile),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// ویرایش اطلاعات تکمیلی پرسنل.
    /// </summary>
    [HttpPatch("{id:guid}/supplementary")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateSupplementary(
        Guid id,
        [FromBody] UpdateSupplementaryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateSupplementaryCommand(id, request.BirthDate, request.ServiceYears, request.ServiceMonths, request.PezhvakMobile),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// انتساب پرسنل به پست.
    /// </summary>
    [HttpPost("{id:guid}/posts")]
    [Authorize(Policy = AuthorizationPolicies.Employee.AssignPost)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> AssignPost(
        Guid id,
        [FromBody] AssignPostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new AssignPostCommand(id, request.PostId, request.FromDate, request.ToDate, request.IsPrimary),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// پایان دادن به انتساب پرسنل به پست.
    /// </summary>
    [HttpDelete("{id:guid}/posts/{postId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Employee.RemovePost)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> EndAssignment(
        Guid id,
        Guid postId,
        [FromBody] EndAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new EndAssignmentCommand(id, postId, request.EndDate),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تعیین وضعیت فعال/غیرفعال پرسنل.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Disable)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetStatus(
        Guid id,
        [FromBody] SetEmployeeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SetEmployeeStatusCommand(id, request.IsActive),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// اتصال پرسنل به حساب کاربری IAM.
    /// </summary>
    [HttpPost("{id:guid}/link-user")]
    [Authorize(Policy = AuthorizationPolicies.Employee.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> LinkUser(
        Guid id,
        [FromBody] LinkEmployeeUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new LinkEmployeeUserCommand(id, request.UserId),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت پرسنل با شناسه.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Employee.View)]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت پست‌های منتسب به پرسنل.
    /// </summary>
    [HttpGet("{id:guid}/posts")]
    [Authorize(Policy = AuthorizationPolicies.Employee.View)]
    [ProducesResponseType(typeof(IReadOnlyList<EmployeePostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<EmployeePostDto>>> GetPosts(
        Guid id,
        [FromQuery] bool onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetEmployeePostsQuery(id, onlyActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت پرسنل منتسب به پست.
    /// </summary>
    [HttpGet("/api/v1/posts/{postId:guid}/employees")]
    [Authorize(Policy = AuthorizationPolicies.Employee.View)]
    [ProducesResponseType(typeof(IReadOnlyList<PostEmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PostEmployeeDto>>> GetPostEmployees(
        Guid postId,
        [FromQuery] bool onlyActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetPostEmployeesQuery(postId, onlyActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// جستجوی صفحه‌بندی‌شده پرسنل.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.Employee.View)]
    [ProducesResponseType(typeof(Application.Common.PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Application.Common.PagedResult<EmployeeDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new SearchEmployeesQuery(searchTerm, isActive, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// بدنه درخواست ویرایش پرسنل.
/// </summary>
public sealed record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string NationalCode,
    string? Mobile);

/// <summary>
/// بدنه درخواست ویرایش اطلاعات تکمیلی.
/// </summary>
public sealed record UpdateSupplementaryRequest(
    DateOnly? BirthDate,
    int? ServiceYears,
    int? ServiceMonths,
    string? PezhvakMobile);

/// <summary>
/// بدنه درخواست انتساب به پست.
/// </summary>
public sealed record AssignPostRequest(
    Guid PostId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    bool IsPrimary);

/// <summary>
/// بدنه درخواست پایان انتساب.
/// </summary>
public sealed record EndAssignmentRequest(DateOnly EndDate);

/// <summary>
/// بدنه درخواست تعیین وضعیت پرسنل.
/// </summary>
public sealed record SetEmployeeStatusRequest(bool IsActive);

/// <summary>
/// بدنه درخواست اتصال به کاربر.
/// </summary>
public sealed record LinkEmployeeUserRequest(Guid UserId);