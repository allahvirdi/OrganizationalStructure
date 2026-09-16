using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Posts;
using OrganizationalStructure.Application.Responsibilities.DTOs;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Responsibilities.GetPostResponsibilities;

/// <summary>
/// پرس‌وجوی دریافت مسئولیت‌های منتسب به پست.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OnlyActive">فقط انتساب‌های جاری (پیش‌فرض: بله)</param>
public sealed record GetPostResponsibilitiesQuery(Guid PostId, bool OnlyActive = true)
    : IRequest<Result<IReadOnlyList<ResponsibilityAssignmentDto>>>;

/// <summary>
/// اعتبارسنج پرس‌وجوی مسئولیت‌های پست.
/// </summary>
public sealed class GetPostResponsibilitiesQueryValidator
    : AbstractValidator<GetPostResponsibilitiesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetPostResponsibilitiesQueryValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}

/// <summary>
/// پردازش‌گر پرس‌وجوی مسئولیت‌های منتسب به پست.
/// </summary>
public sealed class GetPostResponsibilitiesQueryHandler
    : IRequestHandler<GetPostResponsibilitiesQuery, Result<IReadOnlyList<ResponsibilityAssignmentDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostResponsibilitiesQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ResponsibilityAssignmentDto>>> Handle(
        GetPostResponsibilitiesQuery request,
        CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<IReadOnlyList<ResponsibilityAssignmentDto>>.Failure(
                PostErrors.NotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(post.OrganizationId))
        {
            return Result<IReadOnlyList<ResponsibilityAssignmentDto>>.Failure(
                AccessErrors.Forbidden());
        }

        var query = from a in _db.ResponsibilityAssignments.AsNoTracking()
                    join r in _db.Responsibilities.AsNoTracking() on a.ResponsibilityId equals r.Id
                    join p in _db.Posts.AsNoTracking() on a.PostId equals p.Id
                    where a.PostId == request.PostId
                    select new { Assignment = a, Responsibility = r, Post = p };

        if (request.OnlyActive)
        {
            query = query.Where(x => x.Assignment.IsActive && x.Assignment.EndDate == null);
        }

        var items = await query
            .OrderBy(x => x.Responsibility.Code)
            .Select(x => new ResponsibilityAssignmentDto
            {
                Id = x.Assignment.Id,
                ResponsibilityId = x.Responsibility.Id,
                ResponsibilityCode = x.Responsibility.Code,
                ResponsibilityTitle = x.Responsibility.Title,
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