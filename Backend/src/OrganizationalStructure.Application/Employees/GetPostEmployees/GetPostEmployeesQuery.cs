using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.GetPostEmployees;

/// <summary>
/// پرس‌وجوی دریافت پرسنل منتسب به پست.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OnlyActive">فقط انتساب‌های جاری (پیش‌فرض: بله)</param>
public sealed record GetPostEmployeesQuery(Guid PostId, bool OnlyActive = true)
    : IRequest<Result<IReadOnlyList<PostEmployeeDto>>>;