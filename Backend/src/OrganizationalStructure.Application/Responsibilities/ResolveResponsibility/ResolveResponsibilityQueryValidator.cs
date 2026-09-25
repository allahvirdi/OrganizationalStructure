using FluentValidation;

namespace OrganizationalStructure.Application.Responsibilities.ResolveResponsibility;

/// <summary>
/// اعتبارسنج پرس‌وجوی مسیریابی مسئولیت.
/// </summary>
public sealed class ResolveResponsibilityQueryValidator : AbstractValidator<ResolveResponsibilityQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public ResolveResponsibilityQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("شناسه سازمان الزامی است.");

        RuleFor(x => x.ResponsibilityCode)
            .NotEmpty()
            .WithMessage("کد مسئولیت الزامی است.")
            .MaximumLength(200)
            .WithMessage("کد مسئولیت حداکثر ۲۰۰ نویسه است.");
    }
}
