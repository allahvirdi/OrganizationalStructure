using FluentValidation;

namespace OrganizationalStructure.Application.Posts.GetPostPeers;

/// <summary>
/// اعتبارسنجی پرس‌وجوی هم‌سطح‌های پست.
/// </summary>
public sealed class GetPostPeersQueryValidator : AbstractValidator<GetPostPeersQuery>
{
    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public GetPostPeersQueryValidator()
    {
        RuleFor(x => x.PostId).NotEmpty().WithMessage("شناسه پست الزامی است.");
    }
}
