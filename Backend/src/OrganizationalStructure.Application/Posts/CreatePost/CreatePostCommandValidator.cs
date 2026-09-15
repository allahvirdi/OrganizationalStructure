using FluentValidation;

namespace OrganizationalStructure.Application.Posts.CreatePost;

/// <summary>
/// اعتبارسنج دستور ایجاد پست سازمانی.
/// </summary>
public sealed class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("شناسه سازمان معتبر نیست.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("کد پست الزامی است.")
            .MaximumLength(50)
            .WithMessage("کد پست حداکثر ۵۰ کاراکتر است.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان پست الزامی است.")
            .MaximumLength(200)
            .WithMessage("عنوان پست حداکثر ۲۰۰ کاراکتر است.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("شرح پست حداکثر ۱۰۰۰ کاراکتر است.")
            .When(x => x.Description is not null);
    }
}