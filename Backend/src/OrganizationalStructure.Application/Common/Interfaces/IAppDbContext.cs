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
    /// ذخیره تغییرات در انتظار.
    /// </summary>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>تعداد رکوردهای affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}