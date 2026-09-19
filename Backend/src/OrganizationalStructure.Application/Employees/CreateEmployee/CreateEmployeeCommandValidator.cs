using FluentValidation;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.CreateEmployee;

/// <summary>
/// اعتبارسنج دستور ثبت پرسنل.
/// </summary>
public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.PersonnelCode)
            .NotEmpty()
            .WithMessage("کد پرسنلی الزامی است.")
            .Matches("^[0-9]{8}$")
            .WithMessage("کد پرسنلی باید عدد ۸ رقمی باشد.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("نام الزامی است.")
            .MaximumLength(200)
            .WithMessage("نام حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("نام خانوادگی الزامی است.")
            .MaximumLength(200)
            .WithMessage("نام خانوادگی حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.NationalCode)
            .NotEmpty()
            .WithMessage("کد ملی الزامی است.")
            .Must(IranianNationalCodeValidator.IsValid)
            .WithMessage("کد ملی معتبر نیست.");

        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage("شماره همراه الزامی است.")
            .Matches("^09[0-9]{9}$")
            .WithMessage("شماره همراه باید فرمت ایرانی معتبر داشته باشد (مانند 09121234567).");

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