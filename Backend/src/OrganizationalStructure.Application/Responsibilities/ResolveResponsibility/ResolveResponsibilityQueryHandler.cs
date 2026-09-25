using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Responsibilities.ResolveResponsibility;

/// <summary>
/// پردازش‌گر مسیریابی مسئولیت (FindResponsible).
/// </summary>
/// <remarks>
/// زنجیره: Responsibility → PostResponsibilityAssignment جاری → Post → EmployeePostAssignment فعال → Employee فعال.
/// </remarks>
public sealed class ResolveResponsibilityQueryHandler
    : IRequestHandler<ResolveResponsibilityQuery, Result<IReadOnlyList<ResolvedResponsibleDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public ResolveResponsibilityQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ResolvedResponsibleDto>>> Handle(
        ResolveResponsibilityQuery request,
        CancellationToken cancellationToken)
    {
        // کنترل دسترسی سازمانی
        if (!_currentUser.VisibleOrganizationIds.Contains(request.OrganizationId))
        {
            return Result<IReadOnlyList<ResolvedResponsibleDto>>.Failure(AccessErrors.Forbidden());
        }

        var code = request.ResponsibilityCode.Trim();

        // ۱. یافتن مسئولیت با کد
        var responsibility = await _db.Responsibilities
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code == code && r.IsActive, cancellationToken);

        if (responsibility is null)
        {
            return Result<IReadOnlyList<ResolvedResponsibleDto>>.Failure(
                ResponsibilityErrors.NotFoundByCode(code));
        }

        // ۲. یافتن انتساب‌های جاری در سازمان مشخص
        var assignments = await (
            from a in _db.ResponsibilityAssignments.AsNoTracking()
            join p in _db.Posts.AsNoTracking() on a.PostId equals p.Id
            where a.ResponsibilityId == responsibility.Id
                && a.OrganizationId == request.OrganizationId
                && a.IsActive
                && a.EndDate == null
                && p.IsActive
            select new { Assignment = a, Post = p }
        ).ToListAsync(cancellationToken);

        if (assignments.Count == 0)
        {
            // هیچ انتساب جاری‌ای در این سازمان وجود ندارد — نتیجه خالی معتبر است.
            return Result<IReadOnlyList<ResolvedResponsibleDto>>.Success(
                Array.Empty<ResolvedResponsibleDto>());
        }

        // ۳. یافتن پرسنل فعال منتسب به هر پست
        var postIds = assignments.Select(x => x.Post.Id).ToList();

        var employeeAssignments = await (
            from ea in _db.Assignments.AsNoTracking()
            join e in _db.Employees.AsNoTracking() on ea.EmployeeId equals e.Id
            where postIds.Contains(ea.PostId)
                && ea.ToDate == null
                && !ea.IsDeleted
                && e.IsActive
            select new
            {
                ea.PostId,
                ea.IsPrimary,
                Employee = e
            }
        ).ToListAsync(cancellationToken);

        var employeesByPost = employeeAssignments
            .GroupBy(x => x.PostId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // ۴. ساخت نتیجه
        var results = assignments.Select(x =>
        {
            var employees = employeesByPost.TryGetValue(x.Post.Id, out var list)
                ? list.Select(ea => new ResolvedEmployeeDto
                {
                    EmployeeId = ea.Employee.Id,
                    UserId = ea.Employee.UserId,
                    PersonnelCode = ea.Employee.PersonnelCode,
                    FirstName = ea.Employee.FirstName,
                    LastName = ea.Employee.LastName,
                    IsPrimary = ea.IsPrimary
                }).ToList()
                : new List<ResolvedEmployeeDto>();

            return new ResolvedResponsibleDto
            {
                ResponsibilityId = responsibility.Id,
                ResponsibilityCode = responsibility.Code,
                ResponsibilityTitle = responsibility.Title,
                OrganizationId = request.OrganizationId,
                PostId = x.Post.Id,
                PostCode = x.Post.Code,
                PostTitle = x.Post.Title,
                Employees = employees
            };
        }).ToList();

        return Result<IReadOnlyList<ResolvedResponsibleDto>>.Success(results);
    }
}
