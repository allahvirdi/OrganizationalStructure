using FluentValidation;

namespace OrganizationalStructure.Application.Employees.SearchEmployees;

/// <summary>
/// اعتبارسنج پرس‌وجوی جستجوی پرسنل.
/// </summary>
public sealed class SearchEmployeesQueryValidator : AbstractValidator<SearchEmployeesQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SearchEmployeesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("شماره صفحه باید حداقل ۱ باشد.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
    }
}