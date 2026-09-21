using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// پردازش‌گر دستور بارگذاری دسته‌جمعی پرسنل از فایل.
/// </summary>
/// <remarks>
/// عملیات اتمیک است؛ در صورت خطای هر ردیف هیچ پرسنلی ثبت نمی‌شود.
/// مقادیر حساس در پیام خطا افشا نمی‌شوند.
/// </remarks>
public sealed partial class ImportEmployeesCommandHandler : IRequestHandler<ImportEmployeesCommand, Result<ImportEmployeesResultDto>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="db">زمینه دسترسی به داده</param>
    /// <param name="clock">ساعت قابل تست</param>
    /// <param name="currentUser">کاربر جاری</param>
    public ImportEmployeesCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<ImportEmployeesResultDto>> Handle(
        ImportEmployeesCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Rows.Count == 0)
        {
            return Failure(ImportErrors.InvalidFile("فایل حاوی ردیف داده نیست."));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(request.OrganizationId))
        {
            return Failure(AccessErrors.Forbidden());
        }

        var tenantId = _currentUser.TenantId;
        var rowCodes = request.Rows.Select(r => r.PersonnelCode).ToArray();
        var existingCodes = new HashSet<string>(
            await _db.Employees
                .Where(e => rowCodes.Contains(e.PersonnelCode))
                .Select(e => e.PersonnelCode)
                .ToListAsync(cancellationToken),
            StringComparer.OrdinalIgnoreCase);

        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var employees = new List<Employee>(request.Rows.Count);

        foreach (var row in request.Rows)
        {
            if (!seenCodes.Add(row.PersonnelCode))
            {
                return Failure(ImportErrors.DuplicatePersonnelCodeInFile(row.RowNumber, row.PersonnelCode));
            }

            var built = BuildEmployee(row, tenantId, request.OrganizationId, existingCodes);
            if (built.IsFailure)
            {
                return Failure(built.Error!);
            }

            employees.Add(built.Value!);
        }

        foreach (var employee in employees)
        {
            _db.Employees.Add(employee);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<ImportEmployeesResultDto>.Success(
            new ImportEmployeesResultDto(request.OrganizationId, employees.Count));
    }

    /// <summary>
    /// ساخت نتیجه ناموفق.
    /// </summary>
    /// <param name="error">خطا</param>
    /// <returns>نتیجه ناموفق</returns>
    private static Result<ImportEmployeesResultDto> Failure(Error error)
    {
        return Result<ImportEmployeesResultDto>.Failure(error);
    }
}