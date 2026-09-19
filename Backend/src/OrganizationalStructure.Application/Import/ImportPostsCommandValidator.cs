using FluentValidation;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// اعتبارسنج دستور بارگذاری ساختار پست‌ها از فایل اکسل.
/// </summary>
public sealed class ImportPostsCommandValidator : AbstractValidator<ImportPostsCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public ImportPostsCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("شناسه سازمان معتبر نیست.");

        RuleFor(x => x.Rows)
            .NotEmpty()
            .WithMessage("فایل حاوی ردیف داده نیست.");
    }
}