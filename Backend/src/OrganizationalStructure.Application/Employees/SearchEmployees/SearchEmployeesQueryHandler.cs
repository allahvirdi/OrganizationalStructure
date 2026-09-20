using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Employees.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.SearchEmployees;

/// <summary>
/// پردازش‌گر پرس‌وجوی جستجوی صفحه‌بندی‌شده پرسنل.
/// </summary>
/// <remarks>
/// بهینه‌سازی: فیلتر محدوده سازمانی مستقیماً روی <c>Employee.OrganizationId</c> اعمال می‌شود
/// (بدون زیرکوئری‌های همبسته روی Assignments/Posts) — کاهش چشمگیر زمان اجرا.
/// </remarks>
public sealed class SearchEmployeesQueryHandler
    : IRequestHandler<SearchEmployeesQuery, Result<PagedResult<EmployeeDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchEmployeesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<EmployeeDto>>> Handle(
        SearchEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var scope = _currentUser.VisibleOrganizationIds.ToHashSet();
        var scopeList = scope.ToList();

        // فیلتر اصلی محدوده سازمانی — مستقیم روی فیلد جدید (بدون زیرکوئری)
        var query = _db.Employees
            .AsNoTracking()
            .Where(e => scopeList.Contains(e.OrganizationId))
            .AsQueryable();

        // فیلتر کد پرسنلی
        if (!string.IsNullOrWhiteSpace(request.PersonnelCode))
        {
            var pc = request.PersonnelCode.Trim();
            query = query.Where(e => e.PersonnelCode.Contains(pc));
        }

        // فیلتر کد ملی (تساوی دقیق — رمزنگاری قطعی)
        if (!string.IsNullOrWhiteSpace(request.NationalCode))
        {
            var nc = request.NationalCode.Trim();
            query = query.Where(e => e.NationalCode == nc);
        }

        // جستجوی عمومی (سازگار با نسخه قبلی)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(e =>
                e.PersonnelCode.Contains(term) ||
                e.NationalCode == term);
        }

        // فیلتر سازمان
        if (request.OrganizationId.HasValue)
        {
            query = query.Where(e => e.OrganizationId == request.OrganizationId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.PersonnelCode)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                OrganizationId = e.OrganizationId,
                OrganizationName = null,
                PersonnelCode = e.PersonnelCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                NationalCode = e.NationalCode,
                Mobile = e.Mobile,
                BirthDate = e.BirthDate,
                ServiceYears = e.ServiceRecord != null ? e.ServiceRecord.Years : null,
                ServiceMonths = e.ServiceRecord != null ? e.ServiceRecord.Months : null,
                PezhvakMobile = e.PezhvakMobile,
                UserId = e.UserId,
                IsActive = e.IsActive
            })
            .ToListAsync(cancellationToken);

        // پر کردن نام سازمان از مراجع در دسترس کاربر
        var orgRefs = _currentUser.VisibleOrganizations;
        for (var i = 0; i < items.Count; i++)
        {
            var orgRef = orgRefs.FirstOrDefault(o => o.Id == items[i].OrganizationId);
            if (orgRef is not null)
            {
                items[i] = items[i] with { OrganizationName = orgRef.Name };
            }
        }

        var result = new PagedResult<EmployeeDto>(items, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<EmployeeDto>>.Success(result);
    }
}