using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// دستور بارگذاری ساختار پست‌های یک سازمان از فایل اکسل.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مقصد (مرجع IAM)</param>
/// <param name="Rows">ردیف‌های پست پارس‌شده از فایل اکسل</param>
public sealed record ImportPostsCommand(
    Guid OrganizationId,
    IReadOnlyList<ImportPostRowDto> Rows) : IRequest<Result<ImportPostsResultDto>>;