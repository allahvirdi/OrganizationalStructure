using FluentValidation;

namespace OrganizationalStructure.Application.Employees.AssignPost;

/// <summary>
/// اعتبارسنج دستور انتساب پرسنل به پست.
/// </summary>
public sealed class AssignPostCommandValidator : AbstractValidator<AssignPostCommand>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public AssignPostCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("شناسه پرسنل معتبر نیست.");

        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("شناسه پست معتبر نیست.");

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.")
            .WithName("DateRange");
    }
}