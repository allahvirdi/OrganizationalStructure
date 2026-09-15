using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Authorities.DTOs;
using OrganizationalStructure.Application.Posts;

namespace OrganizationalStructure.Application.Authorities.Queries;

/// <summary>
/// پرس‌وجوی دریافت اختیار با کد.
/// </summary>
/// <param name="Code">کد اختیار</param>
public sealed record GetAuthorityByCodeQuery(string Code)
    : IRequest<Result<AuthorityDto>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی اختیار با کد.
/// </summary>
public sealed class GetAuthorityByCodeQueryValidator
    : AbstractValidator<GetAuthorityByCodeQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetAuthorityByCodeQueryValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("کد اختیار الزامی است.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی دریافت اختیار با کد.
/// </summary>
public sealed class GetAuthorityByCodeQueryHandler
    : IRequestHandler<GetAuthorityByCodeQuery, Result<AuthorityDto>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetAuthorityByCodeQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<AuthorityDto>> Handle(
        GetAuthorityByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _db.Authorities
            .AsNoTracking()
            .Where(a => a.Code == request.Code.Trim())
            .Select(a => new AuthorityDto
            {
                Id = a.Id,
                Code = a.Code,
                Title = a.Title,
                Description = a.Description,
                IsActive = a.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            return Result<AuthorityDto>.Failure(
                AuthorityErrors.NotFoundByCode(request.Code.Trim()));
        }

        return Result<AuthorityDto>.Success(dto);
    }
}

/// <summary>
/// پرس‌وجوی جستجوی صفحه‌بندی‌شده اختیارها.
/// </summary>
/// <param name="SearchTerm">عبارت جستجو در کد/عنوان (اختیاری)</param>
/// <param name="IsActive">فیلتر وضعیت (اختیاری)</param>
/// <param name="Page">شماره صفحه (از ۱)</param>
/// <param name="PageSize">اندازه صفحه</param>
public sealed record SearchAuthoritiesQuery(
    string? SearchTerm,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<AuthorityDto>>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی جستجوی اختیارها.
/// </summary>
public sealed class SearchAuthoritiesQueryValidator
    : AbstractValidator<SearchAuthoritiesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SearchAuthoritiesQueryValidator()
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
/// پردازش‌گر پرس‌وجوی جستجوی اختیارها.
/// </summary>
public sealed class SearchAuthoritiesQueryHandler
    : IRequestHandler<SearchAuthoritiesQuery, Result<PagedResult<AuthorityDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public SearchAuthoritiesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<AuthorityDto>>> Handle(
        SearchAuthoritiesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.Authorities.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(a => a.Code.Contains(term) || a.Title.Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(a => a.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.Code)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AuthorityDto
            {
                Id = a.Id,
                Code = a.Code,
                Title = a.Title,
                Description = a.Description,
                IsActive = a.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<AuthorityDto>>.Success(
            new PagedResult<AuthorityDto>(items, totalCount, request.Page, request.PageSize));
    }
}

/// <summary>
/// پرس‌وجوی دریافت اختیارهای منتسب به پست.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OnlyActive">فقط انتساب‌های جاری (پیش‌فرض: بله)</param>
public sealed record GetPostAuthoritiesQuery(Guid PostId, bool OnlyActive = true)
    : IRequest<Result<IReadOnlyList<AuthorityAssignmentDto>>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی اختیارهای پست.
/// </summary>
public sealed class GetPostAuthoritiesQueryValidator
    : AbstractValidator<GetPostAuthoritiesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetPostAuthoritiesQueryValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی اختیارهای منتسب به پست.
/// </summary>
public sealed class GetPostAuthoritiesQueryHandler
    : IRequestHandler<GetPostAuthoritiesQuery, Result<IReadOnlyList<AuthorityAssignmentDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostAuthoritiesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AuthorityAssignmentDto>>> Handle(
        GetPostAuthoritiesQuery request,
        CancellationToken cancellationToken)
    {
        var postExists = await _db.Posts.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!postExists)
        {
            return Result<IReadOnlyList<AuthorityAssignmentDto>>.Failure(
                PostErrors.NotFound(request.PostId));
        }

        var query = from a in _db.AuthorityAssignments.AsNoTracking()
                    join u in _db.Authorities.AsNoTracking() on a.AuthorityId equals u.Id
                    join p in _db.Posts.AsNoTracking() on a.PostId equals p.Id
                    where a.PostId == request.PostId
                    select new { Assignment = a, Authority = u, Post = p };

        if (request.OnlyActive)
        {
            query = query.Where(x => x.Assignment.IsActive && x.Assignment.EndDate == null);
        }

        var items = await query
            .OrderBy(x => x.Authority.Code)
            .Select(x => new AuthorityAssignmentDto
            {
                Id = x.Assignment.Id,
                AuthorityId = x.Authority.Id,
                AuthorityCode = x.Authority.Code,
                AuthorityTitle = x.Authority.Title,
                OrganizationId = x.Assignment.OrganizationId,
                PostId = x.Post.Id,
                PostCode = x.Post.Code,
                PostTitle = x.Post.Title,
                StartDate = x.Assignment.StartDate,
                EndDate = x.Assignment.EndDate,
                IsActive = x.Assignment.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AuthorityAssignmentDto>>.Success(items);
    }
}