using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostManager;

/// <summary>
/// پرس‌وجوی دریافت رئیس مستقیم (پست والد) یک پست.
/// </summary>
/// <param name="PostId">شناسه پست مبدا</param>
public sealed record GetPostManagerQuery(Guid PostId) : IRequest<Result<PostSummaryDto>>;
