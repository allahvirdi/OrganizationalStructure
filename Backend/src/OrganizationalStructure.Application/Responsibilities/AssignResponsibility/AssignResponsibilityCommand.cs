using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities.AssignResponsibility;

/// <summary>
/// دستور انتساب مسئولیت به پست (Code-based).
/// </summary>
/// <param name="ResponsibilityCode">کد مسئولیت</param>
/// <param name="PostId">شناسه پست مقصد</param>
/// <param name="StartDate">تاریخ شروع (اختیاری)</param>
/// <param name="EndDate">تاریخ پایان (اختیاری)</param>
public sealed record AssignResponsibilityCommand(
    string ResponsibilityCode,
    Guid PostId,
    DateOnly? StartDate,
    DateOnly? EndDate) : IRequest<Result<Guid>>;