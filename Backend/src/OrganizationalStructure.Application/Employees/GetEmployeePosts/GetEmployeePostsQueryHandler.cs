using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Employees.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Employees.GetEmployeePosts;

/// <summary>
/// پردازش‌گر پرس‌وجوی پست‌های منتسب به پرسنل.
/// </summary>
public sealed class GetEmployeePostsQueryHandler
    : IRequestHandler<GetEmployeePostsQuery, Result<IReadOnlyList<EmployeePostDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetEmployeePostsQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<EmployeePostDto>>> Handle(
        GetEmployeePostsQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await _db.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!exists)
        {
            return Result<IReadOnlyList<EmployeePostDto>>.Failure(
                EmployeeErrors.NotFound(request.EmployeeId));
        }

        var visible = await EmployeeScope.IsEmployeeVisibleAsync(
            _db,
            request.EmployeeId,
            _currentUser.VisibleOrganizationIds.ToHashSet(),
            cancellationToken);

        if (!visible)
        {
            return Result<IReadOnlyList<EmployeePostDto>>.Failure(AccessErrors.Forbidden());
        }

        var query = from a in _db.Assignments.AsNoTracking()
                    join p in _db.Posts.AsNoTracking() on a.PostId equals p.Id
                    where a.EmployeeId == request.EmployeeId
                    select new { Assignment = a, Post = p };

        if (request.OnlyActive)
        {
            query = query.Where(x => x.Assignment.ToDate == null);
        }

        var items = await query
            .OrderByDescending(x => x.Assignment.IsPrimary)
            .ThenBy(x => x.Post.Code)
            .Select(x => new EmployeePostDto
            {
                PostId = x.Post.Id,
                PostCode = x.Post.Code,
                PostTitle = x.Post.Title,
                FromDate = x.Assignment.FromDate,
                ToDate = x.Assignment.ToDate,
                IsPrimary = x.Assignment.IsPrimary
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<EmployeePostDto>>.Success(items);
    }
}