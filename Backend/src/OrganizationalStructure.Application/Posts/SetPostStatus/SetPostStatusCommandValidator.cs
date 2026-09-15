using FluentValidation;

namespace OrganizationalStructure.Application.Posts.SetPostStatus;

/// <summary>
/// اعتبارسنج دستور تعیین وضعیت پست.
/// </summary>
public sealed class SetPostStatusCommandValidator : AbstractValidator<SetPostStatusCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SetPostStatusCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}