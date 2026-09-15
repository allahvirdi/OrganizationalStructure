using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.SetEmployeeStatus;

/// <summary>
/// دستور تعیین وضعیت فعال/غیرفعال پرسنل.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="IsActive">وضعیت جدید</param>
public sealed record SetEmployeeStatusCommand(
    Guid EmployeeId,
    bool IsActive) : IRequest<Result>;