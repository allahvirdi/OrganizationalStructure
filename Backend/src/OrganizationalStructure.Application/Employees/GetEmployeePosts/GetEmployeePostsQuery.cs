using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.GetEmployeePosts;

/// <summary>
/// پرس‌وجوی دریافت پست‌های منتسب به پرسنل.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="OnlyActive">فقط انتساب‌های جاری (پیش‌فرض: بله)</param>
public sealed record GetEmployeePostsQuery(Guid EmployeeId, bool OnlyActive = true)
    : IRequest<Result<IReadOnlyList<EmployeePostDto>>>;