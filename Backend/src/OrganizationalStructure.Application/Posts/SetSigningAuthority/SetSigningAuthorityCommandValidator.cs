using FluentValidation;

namespace OrganizationalStructure.Application.Posts.SetSigningAuthority;

/// <summary>
/// اعتبارسنج دستور تعیین صاحب‌امضا.
/// </summary>
public sealed class SetSigningAuthorityCommandValidator : AbstractValidator<SetSigningAuthorityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SetSigningAuthorityCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}