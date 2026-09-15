using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.GetEmployeeById;

/// <summary>
/// پرس‌وجوی دریافت پرسنل با شناسه.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
public sealed record GetEmployeeByIdQuery(Guid EmployeeId) : IRequest<Result<EmployeeDto>>;