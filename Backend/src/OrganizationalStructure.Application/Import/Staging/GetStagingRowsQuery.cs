using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging.DTOs;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پرس‌وجوی ردیف‌های یک بارگذاری واسط (صفحه‌بندی‌شده) همراه خطاها.
/// </summary>
/// <param name="BatchId">شناسه بارگذاری</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record GetStagingRowsQuery(
    Guid BatchId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<StagingRowDto>>>;