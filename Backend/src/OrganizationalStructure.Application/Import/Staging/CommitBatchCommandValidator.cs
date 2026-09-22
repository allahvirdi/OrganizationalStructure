using FluentValidation;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// اعتبارسنج ساختاری دستور ثبت نهایی بارگذاری واسط.
/// </summary>
public sealed class CommitBatchCommandValidator : AbstractValidator<CommitBatchCommand>
{
    /// <summary>
    /// تعریف قواعد.
    /// </summary>
    public CommitBatchCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("شناسه بارگذاری معتبر نیست.");
    }
}