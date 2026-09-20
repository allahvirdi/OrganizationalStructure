using MediatR;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.SearchEmployees;

/// <summary>
/// پرس‌وجوی جستجوی صفحه‌بندی‌شده پرسنل.
/// </summary>
/// <remarks>
/// فیلترهای سروری: کد پرسنلی (شامل)، کد ملی (تساوی دقیق)، سازمان.
/// جستجوی نام/نام‌خانوادگی در پایگاه داده ممکن نیست (رمزنگاری تصادفی) — محدودیت مستند ADR-006.
/// </remarks>
/// <param name="SearchTerm">عبارت جستجوی عمومی (کد پرسنلی یا کد ملی)</param>
/// <param name="PersonnelCode">فیلتر کد پرسنلی (شامل)</param>
/// <param name="NationalCode">فیلتر کد ملی (تساوی دقیق)</param>
/// <param name="IsActive">فیلتر وضعیت (اختیاری)</param>
/// <param name="OrganizationId">فیلتر سازمان (اختیاری)</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record SearchEmployeesQuery(
    string? SearchTerm,
    string? PersonnelCode,
    string? NationalCode,
    bool? IsActive,
    Guid? OrganizationId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<EmployeeDto>>>;