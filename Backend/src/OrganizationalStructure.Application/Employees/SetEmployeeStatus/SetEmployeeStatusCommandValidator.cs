using FluentValidation;

namespace OrganizationalStructure.Application.Employees.SetEmployeeStatus;

/// <summary>
/// اعتبارسنج دستور تعیین وضعیت پرسنل.
/// </summary>
public sealed class SetEmployeeStatusCommandValidator : AbstractValidator<SetEmployeeStatusCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SetEmployeeStatusCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");
    }
}