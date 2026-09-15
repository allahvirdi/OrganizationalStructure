using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostChildren;

/// <summary>
/// پرس‌وجوی دریافت فرزندان مستقیم یک پست.
/// </summary>
/// <param name="PostId">شناسه پست والد</param>
public sealed record GetPostChildrenQuery(Guid PostId) : IRequest<Result<IReadOnlyList<PostDto>>>;