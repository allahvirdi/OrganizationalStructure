using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common.Interfaces;

namespace OrganizationalStructure.Application.Authorization;

/// <summary>
/// خوانش Scope مشاهده پرسنل از persistence.
/// </summary>
public static class EmployeeScope
{
    /// <summary>
    /// بررسی مشاهده‌پذیری یک پرسنل.
    /// </summary>
    /// <param name="db">زمینه دسترسی داده</param>
    /// <param name="employeeId">شناسه پرسنل</param>
    /// <param name="scope">محدوده سازمانی کاربر</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>درست در صورت مشاهده‌پذیر بودن</returns>
    public static async Task<bool> IsEmployeeVisibleAsync(
        IAppDbContext db,
        Guid employeeId,
        IReadOnlySet<Guid> scope,
        CancellationToken cancellationToken)
    {
        var assignments = await db.Assignments
            .AsNoTracking()
            .Where(a => a.EmployeeId == employeeId)
            .Select(a => new { a.PostId, a.ToDate })
            .ToListAsync(cancellationToken);

        var currentPostIds = assignments
            .Where(x => x.ToDate is null)
            .Select(x => x.PostId)
            .ToList();

        var orgIds = currentPostIds.Count == 0
            ? new List<Guid>()
            : await db.Posts
                .AsNoTracking()
                .Where(p => currentPostIds.Contains(p.Id))
                .Select(p => p.OrganizationId)
                .Distinct()
                .ToListAsync(cancellationToken);

        return EmployeeVisibility.IsVisible(orgIds, assignments.Count > 0, scope);
    }
}