using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostPeers;

/// <summary>
/// پرس‌وجوی دریافت هم‌سطح‌های مستقیم یک پست (پست‌هایی با والد مشترک).
/// </summary>
/// <param name="PostId">شناسه پست مبدا</param>
/// <param name="IncludeSelf">آیا پست مبدا هم در نتیجه باشد؟</param>
public sealed record GetPostPeersQuery(Guid PostId, bool IncludeSelf = false)
    : IRequest<Result<IReadOnlyList<PostSummaryDto>>>;
