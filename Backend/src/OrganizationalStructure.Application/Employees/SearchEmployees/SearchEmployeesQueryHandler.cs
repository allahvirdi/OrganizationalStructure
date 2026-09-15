using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Employees.DTOs;

namespace OrganizationalStructure.Application.Employees.SearchEmployees;

/// <summary>
/// پردازش‌گر پرس‌وجوی جستجوی صفحه‌بندی‌شده پرسنل.
/// </summary>
public sealed class SearchEmployeesQueryHandler
    : IRequestHandler<SearchEmployeesQuery, Result<PagedResult<EmployeeDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchEmployeesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<EmployeeDto>>> Handle(
        SearchEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Employees.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(e =>
                e.PersonnelCode.Contains(term) ||
                e.NationalCode == term);
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

        var result = new PagedResult<EmployeeDto>(items, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<EmployeeDto>>.Success(result);
    }
}