using FluentValidation;

namespace OrganizationalStructure.Application.Posts.SearchPosts;

/// <summary>
/// اعتبارسنج پرس‌وجوی جستجوی پست‌ها.
/// </summary>
public sealed class SearchPostsQueryValidator : AbstractValidator<SearchPostsQuery>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public SearchPostsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("شماره صفحه باید حداقل ۱ باشد.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
    }
}