using FluentValidation;

namespace OrganizationalStructure.Application.Responsibilities.CreateResponsibility;

/// <summary>
/// اعتبارسنج دستور تعریف مسئولیت.
/// </summary>
public sealed class CreateResponsibilityCommandValidator : AbstractValidator<CreateResponsibilityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public CreateResponsibilityCommandValidator()
    {
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