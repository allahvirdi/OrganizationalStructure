using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.CreateEmployee;

/// <summary>
/// دستور ثبت پرسنل جدید.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مالک پرسنل (اجباری)</param>
/// <param name="PersonnelCode">کد پرسنلی (عدد ۸ رقمی، یکتا در مستأجر)</param>
/// <param name="FirstName">نام</param>
/// <param name="LastName">نام خانوادگی</param>
/// <param name="NationalCode">کد ملی</param>
/// <param name="Mobile">شماره همراه (اجباری، فرمت ایرانی)</param>
/// <param name="BirthDate">تاریخ تولد (اختیاری)</param>
/// <param name="ServiceYears">سال سابقه حراست (اختیاری؛ همراه ماه)</param>
/// <param name="ServiceMonths">ماه سابقه حراست ۰..۱۱ (اختیاری؛ همراه سال)</param>
/// <param name="PezhvakMobile">موبایل پژواک (اختیاری)</param>
/// <param name="UserId">شناسه کاربر IAM (اختیاری)</param>
public sealed record CreateEmployeeCommand(
    Guid OrganizationId,
    string PersonnelCode,
    string FirstName,
    string LastName,
    string NationalCode,
    string Mobile,
    DateOnly? BirthDate,
    int? ServiceYears,
    int? ServiceMonths,
    string? PezhvakMobile,
    Guid? UserId) : IRequest<Result<Guid>>;