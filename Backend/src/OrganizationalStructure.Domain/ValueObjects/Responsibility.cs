using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.ValueObjects;

/// <summary>
/// مسئولیت سازمانی ویژه‌ی یک پست.
/// </summary>
/// <remarks>
/// Value Object تغییرناپذیر است و مالک مستقلی ندارد؛ همراه Aggregate
/// <c>Post</c> نگهداری می‌شود (در EF به‌صورت Owned ذخیره خواهد شد).
/// </remarks>
public sealed record Responsibility
{
    /// <summary>
    /// عنوان مسئولیت.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// شرح اختیاری مسئولیت.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// ساخت مسئولیت جدید.
    /// </summary>
    /// <param name="title">عنوان مسئولیت</param>
    /// <param name="description">شرح اختیاری</param>
    /// <exception cref="ArgumentException">در صورت خالی بودن عنوان</exception>
    public Responsibility(string title, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("عنوان مسئولیت نمی‌تواند خالی باشد.", nameof(title));
        }

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}