using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging.DTOs;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پرس‌وجوی فهرست بارگذاری‌های واسط (صفحه‌بندی‌شده).
/// </summary>
/// <param name="OrganizationId">فیلتر سازمان (اختیاری)</param>
/// <param name="Status">فیلتر وضعیت چرخه حیات (اختیاری)</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record ListImportBatchesQuery(
    Guid? OrganizationId,
    ImportBatchStatus? Status,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ImportBatchDto>>>;