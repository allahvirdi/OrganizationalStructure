using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.UpdatePost;

/// <summary>
/// دستور ویرایش پست سازمانی (عنوان، شرح و کد).
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="Code">کد جدید (یکتا درون سازمان)</param>
/// <param name="Title">عنوان جدید</param>
/// <param name="Description">شرح جدید</param>
public sealed record UpdatePostCommand(
    Guid PostId,
    string Code,
    string Title,
    string? Description) : IRequest<Result>;