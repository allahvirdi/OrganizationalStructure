using FluentValidation;

namespace OrganizationalStructure.Application.Employees.GetEmployeeById;

/// <summary>
/// اعتبارسنج پرس‌وجوی دریافت پرسنل.
/// </summary>
public sealed class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");
    }
}