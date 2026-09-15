using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostById;

/// <summary>
/// پرس‌وجوی دریافت پست با شناسه.
/// </summary>
/// <param name="PostId">شناسه پست</param>
public sealed record GetPostByIdQuery(Guid PostId) : IRequest<Result<PostDto>>;