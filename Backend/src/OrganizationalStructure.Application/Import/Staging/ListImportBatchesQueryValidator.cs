using FluentValidation;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// اعتبارسنج پرس‌وجوی فهرست بارگذاری‌های واسط.
/// </summary>
public sealed class ListImportBatchesQueryValidator : AbstractValidator<ListImportBatchesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public ListImportBatchesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("شماره صفحه باید حداقل ۱ باشد.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
    }
}