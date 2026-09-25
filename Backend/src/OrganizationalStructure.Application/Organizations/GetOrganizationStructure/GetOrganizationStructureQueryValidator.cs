using FluentValidation;

namespace OrganizationalStructure.Application.Organizations.GetOrganizationStructure;

/// <summary>
/// اعتبارسنج پرس‌وجوی ساختار سازمان.
/// </summary>
public sealed class GetOrganizationStructureQueryValidator : AbstractValidator<GetOrganizationStructureQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetOrganizationStructureQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("شناسه سازمان الزامی است.");
    }
}
