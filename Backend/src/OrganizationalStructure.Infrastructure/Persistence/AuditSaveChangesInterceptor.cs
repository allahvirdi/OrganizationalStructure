using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Infrastructure.Persistence;

/// <summary>
/// درون‌پرداز جهش‌های ذخیره‌سازی برای اعمال خودکار حسابرسی، Soft Delete و چندمستأجری.
/// </summary>
/// <remarks>
/// هنگام <c>SavingChanges</c> موارد زیر به‌طور خودکار اعمال می‌شود:
/// <list type="bullet">
/// <item>ثبت <see cref="IAuditable.CreatedAt"/>/<see cref="IAuditable.CreatedById"/> برای رکوردهای جدید.</item>
/// <item>ثبت <see cref="IAuditable.UpdatedAt"/>/<see cref="IAuditable.UpdatedById"/> برای رکوردهای ویرایش‌شده.</item>
/// <item>اعمال <see cref="ITenantScoped.TenantId"/> برای رکوردهای جدید در صورت خالی بودن.</item>
/// <item>تبدیل حذف فیزیکی به Soft Delete برای موجودیت‌های مشتق از <see cref="TenantEntity"/>.</item>
/// </list>
/// </remarks>
public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه درون‌پرداز.
    /// </summary>
    /// <param name="clock">ساعت</param>
    /// <param name="currentUser">کاربر جاری</param>
    public AuditSaveChangesInterceptor(IClock clock, ICurrentUser currentUser)
    {
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// اعمال حسابرسی/Soft Delete/تعیین مستأجر روی تغییرات در انتظار.
    /// </summary>
    /// <param name="context">DbContext جاری</param>
    private void ApplyAudit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = _clock.UtcNow;
        var userId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty;
        var tenantId = _currentUser.TenantId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyCreated(entry.Entity, now, userId);
                    ApplyTenant(entry.Entity, tenantId);
                    break;

                case EntityState.Modified:
                    ApplyModified(entry.Entity, now, userId);
                    break;

                case EntityState.Deleted:
                    ApplySoftDelete(entry.Entity, now, userId);
                    break;
            }
        }
    }

    private static void ApplyCreated(object entity, DateTimeOffset now, Guid userId)
    {
        if (entity is IAuditable auditable)
        {
            auditable.CreatedAt = now;
            auditable.CreatedById = userId;
        }
    }

    private static void ApplyModified(object entity, DateTimeOffset now, Guid userId)
    {
        if (entity is IAuditable auditable)
        {
            auditable.UpdatedAt = now;
            auditable.UpdatedById = userId;
        }
    }

    private static void ApplyTenant(object entity, Guid tenantId)
    {
        if (entity is ITenantScoped tenantScoped && tenantScoped.TenantId == Guid.Empty)
        {
            tenantScoped.TenantId = tenantId;
        }
    }

    private static void ApplySoftDelete(object entity, DateTimeOffset now, Guid userId)
    {
        if (entity is TenantEntity tenantEntity)
        {
            tenantEntity.IsDeleted = true;
            tenantEntity.DeletedAt = now;
            tenantEntity.DeletedById = userId == Guid.Empty ? null : userId;
        }
    }
}