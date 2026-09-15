using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.MovePost;

/// <summary>
/// دستور جابجایی پست در درخت سازمان (تغییر والد مستقیم).
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="NewParentId">شناسه والد جدید (خالی یعنی انتقال به ریشه)</param>
public sealed record MovePostCommand(
    Guid PostId,
    Guid? NewParentId) : IRequest<Result>;