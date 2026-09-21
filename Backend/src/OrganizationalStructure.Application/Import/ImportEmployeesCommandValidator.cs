using FluentValidation;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// اعتبارسنج دستور بارگذاری دسته‌جمعی پرسنل از فایل.
/// </summary>
public sealed class ImportEmployeesCommandValidator : AbstractValidator<ImportEmployeesCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی ساختاری.
    /// </summary>
    public ImportEmployeesCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("شناسه سازمان معتبر نیست.");

        RuleFor(x => x.Rows)
            .NotEmpty()
            .WithMessage("فایل حاوی ردیف داده نیست.");
    }
}