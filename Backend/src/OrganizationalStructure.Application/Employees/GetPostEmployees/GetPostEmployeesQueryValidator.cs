using FluentValidation;

namespace OrganizationalStructure.Application.Employees.GetPostEmployees;

/// <summary>
/// اعتبارسنج پرس‌وجوی پرسنل پست.
/// </summary>
public sealed class GetPostEmployeesQueryValidator : AbstractValidator<GetPostEmployeesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetPostEmployeesQueryValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");
    }
}