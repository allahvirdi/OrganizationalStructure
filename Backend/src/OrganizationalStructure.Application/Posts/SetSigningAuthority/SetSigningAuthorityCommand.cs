using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts.SetSigningAuthority;

/// <summary>
/// دستور تعیین وضعیت صاحب‌امضا بودن پست.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="HasSigningAuthority">وضعیت جدید</param>
public sealed record SetSigningAuthorityCommand(
    Guid PostId,
    bool HasSigningAuthority) : IRequest<Result>;