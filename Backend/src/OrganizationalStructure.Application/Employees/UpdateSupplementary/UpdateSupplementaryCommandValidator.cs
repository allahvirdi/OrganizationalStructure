using FluentValidation;

namespace OrganizationalStructure.Application.Employees.UpdateSupplementary;

/// <summary>
/// اعتبارسنج دستور ویرایش اطلاعات تکمیلی.
/// </summary>
public sealed class UpdateSupplementaryCommandValidator : AbstractValidator<UpdateSupplementaryCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public UpdateSupplementaryCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");

        RuleFor(x => x.ServiceYears)
            .GreaterThanOrEqualTo(0)
            .WithMessage("سال سابقه نمی‌تواند منفی باشد.")
            .When(x => x.ServiceYears.HasValue);

        RuleFor(x => x.ServiceMonths)
            .InclusiveBetween(0, 11)
            .WithMessage("ماه سابقه باید بین ۰ تا ۱۱ باشد.")
            .When(x => x.ServiceMonths.HasValue);

        RuleFor(x => x)
            .Must(x => x.ServiceYears.HasValue == x.ServiceMonths.HasValue)
            .WithMessage("سال و ماه سابقه باید با هم وارد شوند.")
            .WithName("ServiceRecord");
    }
}