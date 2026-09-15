using FluentValidation;

namespace OrganizationalStructure.Application.Posts.ManageResponsibilities;

/// <summary>
/// اعتبارسنج دستورهای مدیریت مسئولیت.
/// </summary>
public sealed class AddResponsibilityCommandValidator : AbstractValidator<AddResponsibilityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public AddResponsibilityCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان مسئولیت الزامی است.")
            .MaximumLength(200)
            .WithMessage("عنوان مسئولیت حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("شرح مسئولیت حداکثر ۱۰۰۰ کاراکتر است.")
            .When(x => x.Description is not null);
    }
}

/// <summary>
/// اعتبارسنج دستور حذف مسئولیت.
/// </summary>
public sealed class RemoveResponsibilityCommandValidator : AbstractValidator<RemoveResponsibilityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public RemoveResponsibilityCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان مسئولیت الزامی است.");
    }
}