using FluentValidation;

namespace OrganizationalStructure.Application.Responsibilities.DisableResponsibility;

/// <summary>
/// اعتبارسنج دستور غیرفعال‌سازی مسئولیت.
/// </summary>
public sealed class DisableResponsibilityCommandValidator
    : AbstractValidator<DisableResponsibilityCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public DisableResponsibilityCommandValidator()
    {
        RuleFor(x => x.ResponsibilityId)
            .NotEmpty()
            .WithMessage("شناسه مسئولیت معتبر نیست.");
    }
}