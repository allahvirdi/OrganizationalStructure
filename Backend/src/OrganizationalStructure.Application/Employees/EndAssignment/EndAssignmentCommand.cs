using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.EndAssignment;

/// <summary>
/// دستور پایان دادن به انتساب پرسنل به پست.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="EndDate">تاریخ پایان</param>
public sealed record EndAssignmentCommand(
    Guid EmployeeId,
    Guid PostId,
    DateOnly EndDate) : IRequest<Result>;