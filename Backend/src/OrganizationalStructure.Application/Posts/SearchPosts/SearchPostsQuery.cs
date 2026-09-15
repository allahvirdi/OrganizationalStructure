using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Posts.DTOs;

namespace OrganizationalStructure.Application.Posts.SearchPosts;

/// <summary>
/// پرس‌وجوی جستجوی صفحه‌بندی‌شده پست‌ها.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان (اختیاری)</param>
/// <param name="SearchTerm">عبارت جستجو در کد/عنوان (اختیاری)</param>
/// <param name="IsActive">فیلتر وضعیت (اختیاری)</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record SearchPostsQuery(
    Guid? OrganizationId,
    string? SearchTerm,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<PostSummaryDto>>>;