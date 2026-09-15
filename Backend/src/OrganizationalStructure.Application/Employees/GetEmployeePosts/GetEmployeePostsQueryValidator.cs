using FluentValidation;

namespace OrganizationalStructure.Application.Employees.GetEmployeePosts;

/// <summary>
/// اعتبارسنج پرس‌وجوی پست‌های پرسنل.
/// </summary>
public sealed class GetEmployeePostsQueryValidator : AbstractValidator<GetEmployeePostsQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public GetEmployeePostsQueryValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");
    }
}