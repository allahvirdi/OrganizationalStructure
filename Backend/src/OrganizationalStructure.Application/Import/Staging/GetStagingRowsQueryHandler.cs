using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Import.Staging.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// پردازش‌گر ردیف‌های یک بارگذاری واسط همراه خطاهای اعتبارسنجی.
/// </summary>
public sealed class GetStagingRowsQueryHandler
    : IRequestHandler<GetStagingRowsQuery, Result<PagedResult<StagingRowDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="db">زمینه داده</param>
    /// <param name="currentUser">کاربر جاری</param>
    public GetStagingRowsQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<StagingRowDto>>> Handle(
        GetStagingRowsQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await _db.ImportBatches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BatchId, cancellationToken);

        if (batch is null)
        {
            return Result<PagedResult<StagingRowDto>>.Failure(
                new Error("Import.BatchNotFound", "بارگذاری یافت نشد.", ErrorType.NotFound));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(batch.OrganizationId))
        {
            return Result<PagedResult<StagingRowDto>>.Failure(AccessErrors.Forbidden());
        }

        var rowsQuery = _db.EmployeeStagingRows
            .AsNoTracking()
            .Where(r => r.BatchId == request.BatchId);

        var totalCount = await rowsQuery.CountAsync(cancellationToken);

        var rows = await rowsQuery
            .OrderBy(r => r.RowNumber)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // خطاهای ردیف‌های صفحه جاری در یک کوئری جدا (جلوگیری از N+1)
        var rowIds = rows.Select(r => r.Id).ToList();
        var errors = await _db.ImportErrors
            .AsNoTracking()
            .Where(e => e.BatchId == request.BatchId
                && e.StagingRowId != null
                && rowIds.Contains(e.StagingRowId.Value))
            .ToListAsync(cancellationToken);

        var errorsByRow = errors
            .GroupBy(e => e.StagingRowId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var items = rows.Select(r => new StagingRowDto
        {
            Id = r.Id,
            RowNumber = r.RowNumber,
            PersonnelCode = r.PersonnelCode,
            FirstName = r.FirstName,
            LastName = r.LastName,
            NationalCode = r.NationalCode,
            Mobile = r.Mobile,
            BirthDate = r.BirthDate,
            ServiceYears = r.ServiceYears,
            ServiceMonths = r.ServiceMonths,
            ValidationStatus = r.ValidationStatus,
            CommitStatus = r.CommitStatus,
            EmployeeId = r.EmployeeId,
            Errors = errorsByRow.TryGetValue(r.Id, out var rowErrors)
                ? rowErrors.Select(e => new StagingRowErrorDto
                {
                    RowNumber = e.RowNumber,
                    ColumnName = e.ColumnName,
                    ErrorCode = e.ErrorCode,
                    Message = e.Message
                }).ToList()
                : Array.Empty<StagingRowErrorDto>()
        }).ToList();

        return Result<PagedResult<StagingRowDto>>.Success(
            new PagedResult<StagingRowDto>(items, totalCount, request.Page, request.PageSize));
    }
}