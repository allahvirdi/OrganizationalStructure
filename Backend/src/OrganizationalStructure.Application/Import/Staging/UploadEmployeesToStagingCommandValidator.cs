using FluentValidation;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// اعتبارسنج ساختاری دستور بارگذاری واسط.
/// </summary>
public sealed class UploadEmployeesToStagingCommandValidator : AbstractValidator<UploadEmployeesToStagingCommand>
{
    /// <summary>
    /// تعریف قواعد.
    /// </summary>
    public UploadEmployeesToStagingCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("شناسه سازمان معتبر نیست.");

        RuleFor(x => x.Rows)
            .NotEmpty()
            .WithMessage("فایل حاوی ردیف داده نیست.");
    }
}