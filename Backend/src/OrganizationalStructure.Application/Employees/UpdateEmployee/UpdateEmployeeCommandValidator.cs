using FluentValidation;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees.UpdateEmployee;

/// <summary>
/// اعتبارسنج دستور ویرایش پرسنل.
/// </summary>
public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");

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
    }
}