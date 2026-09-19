using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Organizations.DTOs;

namespace OrganizationalStructure.Application.Organizations.GetOrganizationOptions;

/// <summary>
/// پرس‌وجوی فهرست سازمان‌های قابل انتخاب کاربر جاری (خود سازمان + زیرمجموعه‌ها).
/// </summary>
/// <param name="SearchTerm">عبارت جستجو روی نام/کد سازمان (اختیاری).</param>
public sealed record GetOrganizationOptionsQuery(string? SearchTerm = null)
    : IRequest<Result<IReadOnlyList<OrganizationOptionDto>>>;
