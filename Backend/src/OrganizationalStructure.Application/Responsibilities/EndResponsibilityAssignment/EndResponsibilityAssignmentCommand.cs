using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities.EndResponsibilityAssignment;

/// <summary>
/// دستور پایان دادن به انتساب مسئولیت به پست (بدون حذف فیزیکی).
/// </summary>
/// <param name="AssignmentId">شناسه انتساب</param>
/// <param name="EndDate">تاریخ پایان</param>
public sealed record EndResponsibilityAssignmentCommand(
    Guid AssignmentId,
    DateOnly EndDate) : IRequest<Result>;