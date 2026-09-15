using FluentValidation;

namespace OrganizationalStructure.Application.Posts.GetPostChildren;

/// <summary>
/// اعتبارسنج پرس‌وجوی فرزندان پست.
/// </summary>
public sealed class GetPostChildrenQueryValidator : AbstractValidator<GetPostChildrenQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetPostChildrenQueryValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}