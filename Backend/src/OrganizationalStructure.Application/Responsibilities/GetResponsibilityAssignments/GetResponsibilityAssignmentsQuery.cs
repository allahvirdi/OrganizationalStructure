using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Responsibilities.DTOs;

namespace OrganizationalStructure.Application.Responsibilities.GetResponsibilityAssignments;

/// <summary>
/// پرس‌وجوی دریافت انتساب‌های یک مسئولیت.
/// </summary>
/// <param name="ResponsibilityCode">کد مسئولیت</param>
/// <param name="OnlyActive">فقط انتساب‌های جاری (پیش‌فرض: بله)</param>
public sealed record GetResponsibilityAssignmentsQuery(string ResponsibilityCode, bool OnlyActive = true)
    : IRequest<Result<IReadOnlyList<ResponsibilityAssignmentDto>>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی انتساب‌های مسئولیت.
/// </summary>
public sealed class GetResponsibilityAssignmentsQueryValidator
    : AbstractValidator<GetResponsibilityAssignmentsQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetResponsibilityAssignmentsQueryValidator()
    {
        RuleFor(x => x.ResponsibilityCode)
            .NotEmpty()
            .WithMessage("کد مسئولیت الزامی است.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی انتساب‌های یک مسئولیت.
/// </summary>
public sealed class GetResponsibilityAssignmentsQueryHandler
    : IRequestHandler<GetResponsibilityAssignmentsQuery, Result<IReadOnlyList<ResponsibilityAssignmentDto>>>
{
    private readonly IAppDbContext _db;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetResponsibilityAssignmentsQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ResponsibilityAssignmentDto>>> Handle(
        GetResponsibilityAssignmentsQuery request,
        CancellationToken cancellationToken)
    {
        var code = request.ResponsibilityCode.Trim();
        var responsibility = await _db.Responsibilities
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);

        if (responsibility is null)
        {
            return Result<IReadOnlyList<ResponsibilityAssignmentDto>>.Failure(
                ResponsibilityErrors.NotFoundByCode(code));
        }

        var query = from a in _db.ResponsibilityAssignments.AsNoTracking()
                    join p in _db.Posts.AsNoTracking() on a.PostId equals p.Id
                    where a.ResponsibilityId == responsibility.Id
                    select new { Assignment = a, Post = p };

        if (request.OnlyActive)
        {
            query = query.Where(x => x.Assignment.IsActive && x.Assignment.EndDate == null);
        }

        var items = await query
            .OrderBy(x => x.Post.Code)
            .Select(x => new ResponsibilityAssignmentDto
            {
                Id = x.Assignment.Id,
                ResponsibilityId = responsibility.Id,
                ResponsibilityCode = responsibility.Code,
                ResponsibilityTitle = responsibility.Title,
                OrganizationId = x.Assignment.OrganizationId,
                PostId = x.Post.Id,
                PostCode = x.Post.Code,
                PostTitle = x.Post.Title,
                StartDate = x.Assignment.StartDate,
                EndDate = x.Assignment.EndDate,
                IsActive = x.Assignment.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<ResponsibilityAssignmentDto>>.Success(items);
    }
}