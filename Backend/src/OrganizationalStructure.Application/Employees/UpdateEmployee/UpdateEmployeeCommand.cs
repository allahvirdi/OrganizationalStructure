using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.UpdateEmployee;

/// <summary>
/// دستور ویرایش اطلاعات پرسنلی.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="FirstName">نام</param>
/// <param name="LastName">نام خانوادگی</param>
/// <param name="NationalCode">کد ملی</param>
/// <param name="Mobile">شماره همراه (اجباری، فرمت ایرانی)</param>
public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    string FirstName,
    string LastName,
    string NationalCode,
    string Mobile) : IRequest<Result>;