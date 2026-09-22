using FluentValidation;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// اعتبارسنج پرس‌وجوی ردیف‌های بارگذاری واسط.
/// </summary>
public sealed class GetStagingRowsQueryValidator : AbstractValidator<GetStagingRowsQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetStagingRowsQueryValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("شناسه بارگذاری الزامی است.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("شماره صفحه باید حداقل ۱ باشد.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
    }
}