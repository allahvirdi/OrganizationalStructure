using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.Application.Posts.CreatePost;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Application.Posts.GetPostById;
using OrganizationalStructure.Application.Posts.GetPostChildren;
using OrganizationalStructure.Application.Posts.GetPostSubtree;
using OrganizationalStructure.Application.Posts.MovePost;
using OrganizationalStructure.Application.Posts.SearchPosts;
using OrganizationalStructure.Application.Posts.SetPostStatus;
using OrganizationalStructure.Application.Posts.UpdatePost;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// مدیریت پست‌های سازمانی.
/// </summary>
/// <remarks>
/// تصریح‌دهی (Policy) در Phase 4 اعمال می‌شود؛ تا آن زمان Endpointها بدون احراز هویت‌اند.
/// </remarks>
[Route("api/v1/posts")]
public sealed class PostsController : ApiControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public PostsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// ایجاد پست سازمانی جدید.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreatePostCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result, id => CreatedAtAction(nameof(GetById), new { id }, id));
    }

    /// <summary>
    /// ویرایش پست سازمانی.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdatePostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdatePostCommand(id, request.Code, request.Title, request.Description),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// جابجایی پست در درخت سازمان.
    /// </summary>
    [HttpPost("{id:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Move(
        Guid id,
        [FromBody] MovePostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new MovePostCommand(id, request.NewParentId),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// تعیین وضعیت فعال/غیرفعال پست.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetStatus(
        Guid id,
        [FromBody] SetPostStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SetPostStatusCommand(id, request.IsActive),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت پست با شناسه.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPostByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت فرزندان مستقیم پست.
    /// </summary>
    [HttpGet("{id:guid}/children")]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PostDto>>> GetChildren(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPostChildrenQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// دریافت زیرشاخه چندسطحی پست.
    /// </summary>
    [HttpGet("{id:guid}/subtree")]
    [ProducesResponseType(typeof(PostTreeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostTreeDto>> GetSubtree(
        Guid id,
        [FromQuery] int? maxDepth,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPostSubtreeQuery(id, maxDepth), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// جستجوی صفحه‌بندی‌شده پست‌ها.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Application.Common.PagedResult<PostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Application.Common.PagedResult<PostDto>>> Search(
        [FromQuery] Guid? organizationId,
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new SearchPostsQuery(organizationId, searchTerm, isActive, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }
}

/// <summary>
/// بدنه درخواست ویرایش پست.
/// </summary>
/// <param name="Code">کد جدید</param>
/// <param name="Title">عنوان جدید</param>
/// <param name="Description">شرح جدید</param>
public sealed record UpdatePostRequest(
    string Code,
    string Title,
    string? Description);

/// <summary>
/// بدنه درخواست جابجایی پست.
/// </summary>
/// <param name="NewParentId">شناسه والد جدید (خالی یعنی ریشه)</param>
public sealed record MovePostRequest(Guid? NewParentId);

/// <summary>
/// بدنه درخواست تعیین وضعیت پست.
/// </summary>
/// <param name="IsActive">وضعیت جدید</param>
public sealed record SetPostStatusRequest(bool IsActive);