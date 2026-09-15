using FluentValidation;

namespace OrganizationalStructure.Application.Posts.GetPostById;

/// <summary>
/// اعتبارسنج پرس‌وجوی دریافت پست.
/// </summary>
public sealed class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetPostByIdQueryValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}