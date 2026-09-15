using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.GetPostSubtree;

/// <summary>
/// پرس‌وجوی دریافت زیرشاخه چندسطحی یک پست.
/// </summary>
/// <param name="PostId">شناسه پست ریشه زیرشاخه</param>
/// <param name="MaxDepth">حداکثر عمق پیمایش (اختیاری؛ خالی یعنی بدون محدودیت داخلی تا سقف امن)</param>
public sealed record GetPostSubtreeQuery(Guid PostId, int? MaxDepth) : IRequest<Result<PostTreeDto>>;