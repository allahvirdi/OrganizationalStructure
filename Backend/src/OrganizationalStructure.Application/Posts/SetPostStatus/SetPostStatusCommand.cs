using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.SetPostStatus;

/// <summary>
/// دستور تعیین وضعیت فعال/غیرفعال پست سازمانی.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="IsActive">وضعیت جدید (فعال یا غیرفعال)</param>
public sealed record SetPostStatusCommand(
    Guid PostId,
    bool IsActive) : IRequest<Result>;