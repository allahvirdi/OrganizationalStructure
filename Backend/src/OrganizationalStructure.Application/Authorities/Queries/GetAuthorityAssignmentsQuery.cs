using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorities.DTOs;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;

namespace OrganizationalStructure.Application.Authorities.Queries;

/// <summary>
/// پرس‌وجوی دریافت انتساب‌های یک اختیار.
/// </summary>
/// <param name="AuthorityCode">کد اختیار</param>
/// <param name="OnlyActive">فقط انتساب‌های جاری (پیش‌فرض: بله)</param>
public sealed record GetAuthorityAssignmentsQuery(string AuthorityCode, bool OnlyActive = true)
    : IRequest<Result<IReadOnlyList<AuthorityAssignmentDto>>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی انتساب‌های اختیار.
/// </summary>
public sealed class GetAuthorityAssignmentsQueryValidator
    : AbstractValidator<GetAuthorityAssignmentsQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetAuthorityAssignmentsQueryValidator()
    {
        RuleFor(x => x.AuthorityCode)
            .NotEmpty()
            .WithMessage("کد اختیار الزامی است.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی انتساب‌های یک اختیار.
/// </summary>
public sealed class GetAuthorityAssignmentsQueryHandler
    : IRequestHandler<GetAuthorityAssignmentsQuery, Result<IReadOnlyList<AuthorityAssignmentDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetAuthorityAssignmentsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AuthorityAssignmentDto>>> Handle(
        GetAuthorityAssignmentsQuery request,
        CancellationToken cancellationToken)
    {
        var code = request.AuthorityCode.Trim();
        var authority = await _db.Authorities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == code, cancellationToken);

        if (authority is null)
        {
            return Result<IReadOnlyList<AuthorityAssignmentDto>>.Failure(
                AuthorityErrors.NotFoundByCode(code));
        }

        var query = from a in _db.AuthorityAssignments.AsNoTracking()
                    join p in _db.Posts.AsNoTracking() on a.PostId equals p.Id
                    where a.AuthorityId == authority.Id
                    select new { Assignment = a, Post = p };

        if (request.OnlyActive)
        {
            query = query.Where(x => x.Assignment.IsActive && x.Assignment.EndDate == null);
        }

        var items = await query
            .OrderBy(x => x.Post.Code)
            .Select(x => new AuthorityAssignmentDto
            {
                Id = x.Assignment.Id,
                AuthorityId = authority.Id,
                AuthorityCode = authority.Code,
                AuthorityTitle = authority.Title,
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