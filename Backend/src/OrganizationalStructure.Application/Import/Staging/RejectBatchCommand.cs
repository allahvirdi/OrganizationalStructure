using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// دستور رد بارگذاری واسط.
/// </summary>
/// <param name="BatchId">شناسه بارگذاری</param>
/// <param name="Notes">یادداشت بازبین (اختیاری)</param>
public sealed record RejectBatchCommand(
    Guid BatchId,
    string? Notes) : IRequest<Result>;