using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.CreatePost;

/// <summary>
/// دستور ایجاد پست سازمانی جدید.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان (مرجع IAM)</param>
/// <param name="Code">کد یکتای پست درون سازمان</param>
/// <param name="Title">عنوان پست</param>
/// <param name="Description">شرح اختیاری</param>
/// <param name="ParentId">شناسه والد مستقیم (خالی یعنی ریشه)</param>
public sealed record CreatePostCommand(
    Guid OrganizationId,
    string Code,
    string Title,
    string? Description,
    Guid? ParentId) : IRequest<Result<Guid>>;