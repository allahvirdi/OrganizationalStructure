using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Authorities.AssignAuthority;

/// <summary>
/// دستور انتساب اختیار به پست (Code-based).
/// </summary>
/// <param name="AuthorityCode">کد اختیار</param>
/// <param name="PostId">شناسه پست مقصد</param>
/// <param name="StartDate">تاریخ شروع (اختیاری)</param>
/// <param name="EndDate">تاریخ پایان (اختیاری)</param>
public sealed record AssignAuthorityCommand(
    string AuthorityCode,
    Guid PostId,
    DateOnly? StartDate,
    DateOnly? EndDate) : IRequest<Result<Guid>>;

/// <summary>
/// اعتبارسنج دستور انتساب اختیار به پست.
/// </summary>
public sealed class AssignAuthorityCommandValidator : AbstractValidator<AssignAuthorityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public AssignAuthorityCommandValidator()
    {
        RuleFor(x => x.AuthorityCode)
            .NotEmpty()
            .WithMessage("کد اختیار الزامی است.");

        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.")
            .WithName("DateRange");
    }
}

/// <summary>
/// پردازش‌گر دستور انتساب اختیار به پست.
/// </summary>
public sealed class AssignAuthorityCommandHandler
    : IRequestHandler<AssignAuthorityCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AssignAuthorityCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(
        AssignAuthorityCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.AuthorityCode.Trim();

        var authority = await _db.Authorities
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.Code == code, cancellationToken);

        if (authority is null)
        {
            return Result<Guid>.Failure(AuthorityErrors.NotFoundByCode(code));
        }

        if (!authority.IsActive)
        {
            return Result<Guid>.Failure(AuthorityErrors.Inactive(code));
        }

        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is null)
        {
            return Result<Guid>.Failure(AuthorityErrors.PostNotFound(request.PostId));
        }

        if (!_currentUser.VisibleOrganizationIds.Contains(post.OrganizationId))
        {
            return Result<Guid>.Failure(AccessErrors.Forbidden());
        }

        Guid assignmentId;
        try
        {
            var assignment = authority.AssignToPost(
                post.OrganizationId,
                post.Id,
                request.StartDate,
                request.EndDate,
                _clock.UtcNow);

            _db.AuthorityAssignments.Add(assignment);
            assignmentId = assignment.Id;
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Failure(AuthorityErrors.AssignmentConflict(ex.Message));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(assignmentId);
    }
}

/// <summary>
/// دستور پایان دادن به انتساب اختیار به پست (بدون حذف فیزیکی).
/// </summary>
/// <param name="AssignmentId">شناسه انتساب</param>
/// <param name="EndDate">تاریخ پایان</param>
public sealed record EndAuthorityAssignmentCommand(
    Guid AssignmentId,
    DateOnly EndDate) : IRequest<Result>;

/// <summary>
/// اعتبارسنج دستور پایان انتساب اختیار.
/// </summary>
public sealed class EndAuthorityAssignmentCommandValidator
    : AbstractValidator<EndAuthorityAssignmentCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public EndAuthorityAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("شناسه انتساب معتبر نیست.");
    }
}

/// <summary>
/// پردازش‌گر دستور پایان انتساب اختیار به پست.
/// </summary>
public sealed class EndAuthorityAssignmentCommandHandler
    : IRequestHandler<EndAuthorityAssignmentCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public EndAuthorityAssignmentCommandHandler(
        IAppDbContext db,
        IClock clock,
        ICurrentUser currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        EndAuthorityAssignmentCommand request,
        CancellationToken cancellationToken)
    {
        var assignment = await _db.AuthorityAssignments.FirstOrDefaultAsync(
            a => a.Id == request.AssignmentId,
            cancellationToken);

        if (assignment is null)
        {
            return Result.Failure(AuthorityErrors.AssignmentNotFound(request.AssignmentId));
        }

        var assignmentPost = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == assignment.PostId, cancellationToken);

        if (assignmentPost is not null
            && !_currentUser.VisibleOrganizationIds.Contains(assignmentPost.OrganizationId))
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        var authority = await _db.Authorities
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.Id == assignment.AuthorityId, cancellationToken);

        if (authority is null)
        {
            return Result.Failure(AuthorityErrors.NotFound(assignment.AuthorityId));
        }

        try
        {
            authority.EndAssignment(request.AssignmentId, request.EndDate, _clock.UtcNow);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(AuthorityErrors.AssignmentNotFound(request.AssignmentId));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}