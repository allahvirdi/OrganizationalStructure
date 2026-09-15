using FluentValidation;

namespace OrganizationalStructure.Application.Posts.MovePost;

/// <summary>
/// اعتبارسنج دستور جابجایی پست.
/// </summary>
public sealed class MovePostCommandValidator : AbstractValidator<MovePostCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public MovePostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}