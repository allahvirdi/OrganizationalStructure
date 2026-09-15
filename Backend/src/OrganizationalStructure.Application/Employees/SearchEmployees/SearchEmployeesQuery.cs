using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.SearchEmployees;

/// <summary>
/// پرس‌وجوی جستجوی صفحه‌بندی‌شده پرسنل.
/// </summary>
/// <remarks>
/// عبارت جستجو روی کد پرسنلی (شامل) و کد ملی (تساوی دقیق، با رمزنگاری قطعی) اعمال می‌شود؛
/// جستجوی نام/نام‌خانوادگی در پایگاه داده ممکن نیست (رمزنگاری تصادفی) — محدودیت مستند ADR-006.
/// </remarks>
/// <param name="SearchTerm">عبارت جستجو (اختیاری)</param>
/// <param name="IsActive">فیلتر وضعیت (اختیاری)</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record SearchEmployeesQuery(
    string? SearchTerm,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<EmployeeDto>>>;