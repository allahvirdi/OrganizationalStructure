using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// دستور ایجاد بارگذاری بیرونی (مسیر درج مستقیم سامانه بیرونی — DEC-030).
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مقصد</param>
public sealed record CreateExternalBatchCommand(
    Guid OrganizationId) : IRequest<Result<Guid>>;