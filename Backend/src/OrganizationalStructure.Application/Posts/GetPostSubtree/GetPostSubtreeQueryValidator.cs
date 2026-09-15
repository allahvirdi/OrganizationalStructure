using FluentValidation;

namespace OrganizationalStructure.Application.Posts.GetPostSubtree;

/// <summary>
/// اعتبارسنج پرس‌وجوی زیرشاخه پست.
/// </summary>
public sealed class GetPostSubtreeQueryValidator : AbstractValidator<GetPostSubtreeQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetPostSubtreeQueryValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");

        RuleFor(x => x.MaxDepth)
            .InclusiveBetween(1, 20)
            .WithMessage("حداکثر عمق باید بین ۱ تا ۲۰ باشد.")
            .When(x => x.MaxDepth.HasValue);
    }
}