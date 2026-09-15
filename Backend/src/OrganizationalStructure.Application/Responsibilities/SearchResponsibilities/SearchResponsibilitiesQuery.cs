using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Responsibilities.DTOs;

namespace OrganizationalStructure.Application.Responsibilities.SearchResponsibilities;

/// <summary>
/// پرس‌وجوی جستجوی صفحه‌بندی‌شده مسئولیت‌ها.
/// </summary>
/// <param name="SearchTerm">عبارت جستجو در کد/عنوان (اختیاری)</param>
/// <param name="IsActive">فیلتر وضعیت (اختیاری)</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record SearchResponsibilitiesQuery(
    string? SearchTerm,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ResponsibilityDto>>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی جستجوی مسئولیت‌ها.
/// </summary>
public sealed class SearchResponsibilitiesQueryValidator
    : AbstractValidator<SearchResponsibilitiesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SearchResponsibilitiesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("شماره صفحه باید حداقل ۱ باشد.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی جستجوی مسئولیت‌ها.
/// </summary>
public sealed class SearchResponsibilitiesQueryHandler
    : IRequestHandler<SearchResponsibilitiesQuery, Result<PagedResult<ResponsibilityDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchResponsibilitiesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<ResponsibilityDto>>> Handle(
        SearchResponsibilitiesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Responsibilities.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(r => r.Code.Contains(term) || r.Title.Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.Code)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ResponsibilityDto
            {
                Id = r.Id,
                Code = r.Code,
                Title = r.Title,
                Description = r.Description,
                IsActive = r.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ResponsibilityDto>>.Success(
            new PagedResult<ResponsibilityDto>(items, totalCount, request.Page, request.PageSize));
    }
}