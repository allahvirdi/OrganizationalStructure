using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Import.Staging.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پردازش‌گر فهرست بارگذاری‌های واسط.
/// </summary>
/// <remarks>
/// فقط بارگذاری‌های سازمان‌های داخل محدوده کاربر برگردانده می‌شود (Deny by Default).
/// </remarks>
public sealed class ListImportBatchesQueryHandler
    : IRequestHandler<ListImportBatchesQuery, Result<PagedResult<ImportBatchDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="db">زمینه داده</param>
    /// <param name="currentUser">کاربر جاری</param>
    public ListImportBatchesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<ImportBatchDto>>> Handle(
        ListImportBatchesQuery request,
        CancellationToken cancellationToken)
    {
        var scopeList = _currentUser.VisibleOrganizationIds.ToList();

        var query = _db.ImportBatches
            .AsNoTracking()
            .Where(b => scopeList.Contains(b.OrganizationId))
            .AsQueryable();

        if (request.OrganizationId.HasValue)
        {
            query = query.Where(b => b.OrganizationId == request.OrganizationId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new ImportBatchDto
            {
                Id = b.Id,
                OrganizationId = b.OrganizationId,
                OrganizationName = null,
                Source = b.Source,
                FileName = b.FileName,
                Status = b.Status,
                TotalRows = b.TotalRows,
                ValidRows = b.ValidRows,
                InvalidRows = b.InvalidRows,
                CommittedCount = b.CommittedCount,
                Notes = b.Notes,
                CreatedAt = b.CreatedAt,
                ReviewedAt = b.ReviewedAt,
                CommittedAt = b.CommittedAt
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

        return Result<PagedResult<ImportBatchDto>>.Success(
            new PagedResult<ImportBatchDto>(items, totalCount, request.Page, request.PageSize));
    }
}