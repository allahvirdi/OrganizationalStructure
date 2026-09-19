using FluentValidation;

namespace OrganizationalStructure.Application.Organizations.GetOrganizationOptions;

/// <summary>
/// اعتبارسنج پرس‌وجوی گزینه‌های سازمان.
/// </summary>
public sealed class GetOrganizationOptionsQueryValidator : AbstractValidator<GetOrganizationOptionsQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetOrganizationOptionsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .WithMessage("عبارت جستجوی سازمان حداکثر ۱۰۰ نویسه است.");
    }
}
