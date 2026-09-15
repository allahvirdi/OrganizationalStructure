using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Employees.DTOs;
using OrganizationalStructure.Application.Posts;

namespace OrganizationalStructure.Application.Employees.GetPostEmployees;

/// <summary>
/// پردازش‌گر پرس‌وجوی پرسنل منتسب به پست.
/// </summary>
public sealed class GetPostEmployeesQueryHandler
    : IRequestHandler<GetPostEmployeesQuery, Result<IReadOnlyList<PostEmployeeDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostEmployeesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PostEmployeeDto>>> Handle(
        GetPostEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var postExists = await _db.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!postExists)
        {
            return Result<IReadOnlyList<PostEmployeeDto>>.Failure(
                PostErrors.NotFound(request.PostId));
        }

        var query = from a in _db.Assignments.AsNoTracking()
                    join e in _db.Employees.AsNoTracking() on a.EmployeeId equals e.Id
                    where a.PostId == request.PostId
                    select new { Assignment = a, Employee = e };

        if (request.OnlyActive)
        {
            query = query.Where(x => x.Assignment.ToDate == null);
        }

        var items = await query
            .OrderBy(x => x.Employee.PersonnelCode)
            .Select(x => new PostEmployeeDto
            {
                EmployeeId = x.Employee.Id,
                PersonnelCode = x.Employee.PersonnelCode,
                FullName = x.Employee.FirstName + " " + x.Employee.LastName,
                FromDate = x.Assignment.FromDate,
                ToDate = x.Assignment.ToDate,
                IsPrimary = x.Assignment.IsPrimary
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PostEmployeeDto>>.Success(items);
    }
}