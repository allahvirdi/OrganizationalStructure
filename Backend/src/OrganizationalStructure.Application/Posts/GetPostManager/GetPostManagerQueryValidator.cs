using FluentValidation;

namespace OrganizationalStructure.Application.Posts.GetPostManager;

/// <summary>
/// اعتبارسنجی پرس‌وجوی رئیس مستقیم پست.
/// </summary>
public sealed class GetPostManagerQueryValidator : AbstractValidator<GetPostManagerQuery>
{
    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostManagerQueryValidator()
    {
        RuleFor(x => x.PostId).NotEmpty().WithMessage("شناسه پست الزامی است.");
    }
}
