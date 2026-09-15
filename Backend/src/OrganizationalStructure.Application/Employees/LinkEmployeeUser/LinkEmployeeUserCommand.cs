using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.LinkEmployeeUser;

/// <summary>
/// دستور اتصال پرسنل به حساب کاربری IAM.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="UserId">شناسه کاربر IAM</param>
public sealed record LinkEmployeeUserCommand(
    Guid EmployeeId,
    Guid UserId) : IRequest<Result>;