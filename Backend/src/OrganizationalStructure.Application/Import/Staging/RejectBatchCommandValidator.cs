using FluentValidation;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// اعتبارسنج ساختاری دستور رد بارگذاری واسط.
/// </summary>
public sealed class RejectBatchCommandValidator : AbstractValidator<RejectBatchCommand>
{
    /// <summary>
    /// تعریف قواعد.
    /// </summary>
    public RejectBatchCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("شناسه بارگذاری معتبر نیست.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("یادداشت نباید بیش از ۵۰۰ کاراکتر باشد.");
    }
}