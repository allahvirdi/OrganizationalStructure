using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Application.Import.Staging.DTOs;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// دستور بارگذاری فایل پرسنل در جدول واسط (مسیر فایل — DEC-030).
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مقصد</param>
/// <param name="FileName">نام فایل اصلی</param>
/// <param name="Rows">ردیف‌های پارس‌شده از فایل</param>
public sealed record UploadEmployeesToStagingCommand(
    Guid OrganizationId,
    string? FileName,
    IReadOnlyList<ImportEmployeeRowDto> Rows) : IRequest<Result<StagingUploadResultDto>>;