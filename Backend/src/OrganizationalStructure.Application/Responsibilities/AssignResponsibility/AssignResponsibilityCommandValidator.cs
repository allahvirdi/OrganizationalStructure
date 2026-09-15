using FluentValidation;

namespace OrganizationalStructure.Application.Responsibilities.AssignResponsibility;

/// <summary>
/// اعتبارسنج دستور انتساب مسئولیت به پست.
/// </summary>
public sealed class AssignResponsibilityCommandValidator
    : AbstractValidator<AssignResponsibilityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public AssignResponsibilityCommandValidator()
    {
        RuleFor(x => x.ResponsibilityCode)
            .NotEmpty()
            .WithMessage("کد مسئولیت الزامی است.");

        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.")
            .WithName("DateRange");
    }
}