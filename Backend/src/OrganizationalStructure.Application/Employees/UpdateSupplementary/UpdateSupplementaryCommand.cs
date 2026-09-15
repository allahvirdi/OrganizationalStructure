using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.UpdateSupplementary;

/// <summary>
/// دستور ویرایش اطلاعات تکمیلی پرسنل.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="BirthDate">تاریخ تولد (اختیاری)</param>
/// <param name="ServiceYears">سال سابقه (اختیاری؛ همراه ماه)</param>
/// <param name="ServiceMonths">ماه سابقه ۰..۱۱ (اختیاری؛ همراه سال)</param>
/// <param name="PezhvakMobile">موبایل پژواک (اختیاری)</param>
public sealed record UpdateSupplementaryCommand(
    Guid EmployeeId,
    DateOnly? BirthDate,
    int? ServiceYears,
    int? ServiceMonths,
    string? PezhvakMobile) : IRequest<Result>;