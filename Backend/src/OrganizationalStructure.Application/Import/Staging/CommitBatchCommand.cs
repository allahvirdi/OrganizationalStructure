using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.Staging.DTOs;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// دستور ثبت نهایی بارگذاری واسط.
/// </summary>
/// <remarks>
/// مطابق DEC-030: ردیف‌های معتبر به جدول پرسنل منتقل می‌شوند؛
/// ردیف‌های نامعتبر/تکراری در پاسخ گزارش می‌شوند ولی مانع ثبت بقیه نمی‌شوند.
/// </remarks>
/// <param name="BatchId">شناسه بارگذاری</param>
public sealed record CommitBatchCommand(
    Guid BatchId) : IRequest<Result<StagingCommitResultDto>>;