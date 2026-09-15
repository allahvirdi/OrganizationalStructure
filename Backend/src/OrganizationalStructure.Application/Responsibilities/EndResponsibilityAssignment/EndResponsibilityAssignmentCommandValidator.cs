using FluentValidation;

namespace OrganizationalStructure.Application.Responsibilities.EndResponsibilityAssignment;

/// <summary>
/// اعتبارسنج دستور پایان انتساب مسئولیت.
/// </summary>
public sealed class EndResponsibilityAssignmentCommandValidator
    : AbstractValidator<EndResponsibilityAssignmentCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public EndResponsibilityAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage("شناسه انتساب معتبر نیست.");
    }
}