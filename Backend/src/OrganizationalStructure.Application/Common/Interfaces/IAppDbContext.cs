using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Common.Interfaces;

/// <summary>
/// قرارداد دسترسی به persistence برای لایه کاربرد (CQRS Handlers).
/// </summary>
public interface IAppDbContext
{
    /// <summary>
    /// مجموعه پست‌های سازمانی.
    /// </summary>
    DbSet<Post> Posts { get; }

    /// <summary>
    /// مجموعه پرسنل.
    /// </summary>
    DbSet<Employee> Employees { get; }

    /// <summary>
    /// مجموعه انتساب‌های پرسنل به پست.
    /// </summary>
    DbSet<EmployeePostAssignment> Assignments { get; }

    /// <summary>
    /// مجموعه مسئولیت‌های سازمانی.
    /// </summary>
    DbSet<Responsibility> Responsibilities { get; }

    /// <summary>
    /// مجموعه انتساب‌های مسئولیت به پست.
    /// </summary>
    DbSet<PostResponsibilityAssignment> ResponsibilityAssignments { get; }

    /// <summary>
    /// مجموعه اختیارهای سازمانی.
    /// </summary>
    DbSet<Authority> Authorities { get; }

    /// <summary>
    /// مجموعه انتساب‌های اختیار به پست.
    /// </summary>
    DbSet<PostAuthorityAssignment> AuthorityAssignments { get; }

    /// <summary>
    /// مجموعه بارگذاری‌های واسط.
    /// </summary>
    DbSet<ImportBatch> ImportBatches { get; }

    /// <summary>
    /// مجموعه ردیف‌های واسط پرسنل.
    /// </summary>
    DbSet<EmployeeStagingRow> EmployeeStagingRows { get; }

    /// <summary>
    /// مجموعه خطاهای بارگذاری واسط.
    /// </summary>
    DbSet<ImportError> ImportErrors { get; }

    /// <summary>
    /// ذخیره تغییرات در انتظار.
    /// </summary>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>تعداد رکوردهای affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}