using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain.Enums;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.API.BackgroundJobs;

/// <summary>
/// سرویس پس‌زمینه پاک‌سازی ردیف‌های واسط منقضی‌شده.
/// </summary>
/// <remarks>
/// مطابق DEC-030: بارگذاری‌های Committed/Rejected پس از ۱۰ روز حذف فیزیکی می‌شوند.
/// داده اصلی پرسنل در جدول Employees ثبت شده و نیازی به نگهداری واسط نیست.
/// </remarks>
public sealed class StagingCleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StagingCleanupBackgroundService> _logger;

    /// <summary>بازه اجرای دوره‌ای (۱ ساعت).</summary>
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    /// <summary>تعداد روز نگهداری پس از تأیید/رد (مصوب کارفرما: ۱۰ روز).</summary>
    private const int RetentionDays = 10;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="scopeFactory">کارخانه اسکوپ خدمات</param>
    /// <param name="logger">لاگر</param>
    public StagingCleanupBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<StagingCleanupBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("سرویس پاک‌سازی جدول واسط شروع به کار کرد.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredBatchesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پاک‌سازی جدول واسط.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    /// <summary>
    /// شناسایی و حذف فیزیکی بارگذاری‌های منقضی‌شده.
    /// </summary>
    private async Task CleanupExpiredBatchesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrganizationalStructureDbContext>();

        var cutoff = DateTimeOffset.UtcNow.AddDays(-RetentionDays);

        var expiredBatchIds = await db.ImportBatches
            .Where(b => (b.Status == ImportBatchStatus.Committed || b.Status == ImportBatchStatus.Rejected)
                        && b.ReviewedAt != null
                        && b.ReviewedAt < cutoff)
            .Select(b => b.Id)
            .ToListAsync(cancellationToken);

        if (expiredBatchIds.Count == 0)
        {
            return;
        }

        // حذف ردیف‌های واسط مرتبط
        var deletedRows = await db.EmployeeStagingRows
            .Where(r => expiredBatchIds.Contains(r.BatchId))
            .ExecuteDeleteAsync(cancellationToken);

        // حذف خطاهای مرتبط
        var deletedErrors = await db.ImportErrors
            .Where(e => expiredBatchIds.Contains(e.BatchId))
            .ExecuteDeleteAsync(cancellationToken);

        // حذف خود بارگذاری‌ها
        var deletedBatches = await db.ImportBatches
            .Where(b => expiredBatchIds.Contains(b.Id))
            .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation(
            "پاک‌سازی جدول واسط: {Batches} بارگذاری، {Rows} ردیف و {Errors} خطا حذف شد.",
            deletedBatches, deletedRows, deletedErrors);
    }
}