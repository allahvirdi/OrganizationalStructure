using FluentValidation;

namespace OrganizationalStructure.Application.Employees.EndAssignment;

/// <summary>
/// اعتبارسنج دستور پایان انتساب.
/// </summary>
public sealed class EndAssignmentCommandValidator : AbstractValidator<EndAssignmentCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public EndAssignmentCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");

        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}