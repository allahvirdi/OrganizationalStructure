using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Application.Authorities.DTOs;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Application.Authorities.ManageAuthorities;

/// <summary>
/// دستور تعریف اختیار سازمانی جدید.
/// </summary>
/// <param name="Code">کد یکتا</param>
/// <param name="Title">عنوان</param>
/// <param name="Description">شرح اختیاری</param>
public sealed record CreateAuthorityCommand(
    string Code,
    string Title,
    string? Description) : IRequest<Result<Guid>>;

/// <summary>
/// اعتبارسنج دستور تعریف اختیار.
/// </summary>
public sealed class CreateAuthorityCommandValidator : AbstractValidator<CreateAuthorityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public CreateAuthorityCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("کد اختیار الزامی است.")
            .MaximumLength(100)
            .WithMessage("کد اختیار حداکثر ۱۰۰ کاراکتر است.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان اختیار الزامی است.")
            .MaximumLength(200)
            .WithMessage("عنوان اختیار حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("شرح حداکثر ۱۰۰۰ کاراکتر است.")
            .When(x => x.Description is not null);
    }
}

/// <summary>
/// پردازش‌گر دستور تعریف اختیار سازمانی.
/// </summary>
public sealed class CreateAuthorityCommandHandler
    : IRequestHandler<CreateAuthorityCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public CreateAuthorityCommandHandler(
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
        CreateAuthorityCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();

        if (await _db.Authorities.AnyAsync(a => a.Code == code, cancellationToken))
        {
            return Result<Guid>.Failure(AuthorityErrors.DuplicateCode(code));
        }

        var authority = Authority.Create(
            Guid.NewGuid(),
            _currentUser.TenantId,
            code,
            request.Title,
            request.Description,
            _clock.UtcNow);

        _db.Authorities.Add(authority);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(authority.Id);
    }
}

/// <summary>
/// دستور ویرایش اختیار سازمانی.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
/// <param name="Title">عنوان جدید</param>
/// <param name="Description">شرح جدید</param>
public sealed record UpdateAuthorityCommand(
    Guid AuthorityId,
    string Title,
    string? Description) : IRequest<Result>;

/// <summary>
/// اعتبارسنج دستور ویرایش اختیار.
/// </summary>
public sealed class UpdateAuthorityCommandValidator : AbstractValidator<UpdateAuthorityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public UpdateAuthorityCommandValidator()
    {
        RuleFor(x => x.AuthorityId)
            .NotEmpty()
            .WithMessage("شناسه اختیار معتبر نیست.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان اختیار الزامی است.")
            .MaximumLength(200)
            .WithMessage("عنوان اختیار حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("شرح حداکثر ۱۰۰۰ کاراکتر است.")
            .When(x => x.Description is not null);
    }
}

/// <summary>
/// پردازش‌گر دستور ویرایش اختیار سازمانی.
/// </summary>
public sealed class UpdateAuthorityCommandHandler
    : IRequestHandler<UpdateAuthorityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public UpdateAuthorityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        UpdateAuthorityCommand request,
        CancellationToken cancellationToken)
    {
        var authority = await _db.Authorities.FirstOrDefaultAsync(
            a => a.Id == request.AuthorityId,
            cancellationToken);

        if (authority is null)
        {
            return Result.Failure(AuthorityErrors.NotFound(request.AuthorityId));
        }

        authority.UpdateDetails(request.Title, request.Description, _clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

/// <summary>
/// دستور غیرفعال‌سازی اختیار سازمانی.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
public sealed record DisableAuthorityCommand(Guid AuthorityId) : IRequest<Result>;

/// <summary>
/// اعتبارسنج دستور غیرفعال‌سازی اختیار.
/// </summary>
public sealed class DisableAuthorityCommandValidator : AbstractValidator<DisableAuthorityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public DisableAuthorityCommandValidator()
    {
        RuleFor(x => x.AuthorityId)
            .NotEmpty()
            .WithMessage("شناسه اختیار معتبر نیست.");
    }
}

/// <summary>
/// پردازش‌گر دستور غیرفعال‌سازی اختیار سازمانی.
/// </summary>
public sealed class DisableAuthorityCommandHandler
    : IRequestHandler<DisableAuthorityCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public DisableAuthorityCommandHandler(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        DisableAuthorityCommand request,
        CancellationToken cancellationToken)
    {
        var authority = await _db.Authorities
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.Id == request.AuthorityId, cancellationToken);

        if (authority is null)
        {
            return Result.Failure(AuthorityErrors.NotFound(request.AuthorityId));
        }

        if (authority.Assignments.Any(a => a.IsCurrent))
        {
            return Result.Failure(AuthorityErrors.HasActiveAssignments());
        }

        authority.Deactivate(_clock.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}