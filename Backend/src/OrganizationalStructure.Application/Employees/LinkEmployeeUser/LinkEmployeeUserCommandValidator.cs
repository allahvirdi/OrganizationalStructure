using FluentValidation;

namespace OrganizationalStructure.Application.Employees.LinkEmployeeUser;

/// <summary>
/// اعتبارسنج دستور اتصال پرسنل به کاربر.
/// </summary>
public sealed class LinkEmployeeUserCommandValidator : AbstractValidator<LinkEmployeeUserCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public LinkEmployeeUserCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("شناسه کاربر معتبر نیست.");
    }
}