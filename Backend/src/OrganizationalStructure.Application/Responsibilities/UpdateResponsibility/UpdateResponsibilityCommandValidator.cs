using FluentValidation;

namespace OrganizationalStructure.Application.Responsibilities.UpdateResponsibility;

/// <summary>
/// اعتبارسنج دستور ویرایش مسئولیت.
/// </summary>
public sealed class UpdateResponsibilityCommandValidator : AbstractValidator<UpdateResponsibilityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public UpdateResponsibilityCommandValidator()
    {
        RuleFor(x => x.ResponsibilityId)
            .NotEmpty()
            .WithMessage("شناسه مسئولیت معتبر نیست.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان مسئولیت الزامی است.")
            .MaximumLength(200)
            .WithMessage("عنوان مسئولیت حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("شرح حداکثر ۱۰۰۰ کاراکتر است.")
            .When(x => x.Description is not null);
    }
}