namespace OrganizationalStructure.Application.Common;

/// <summary>
/// نتیجه صفحه‌بندی‌شده استاندارد.
/// </summary>
/// <typeparam name="TItem">نوع آیتم صفحه</typeparam>
public sealed record PagedResult<TItem>(
    IReadOnlyList<TItem> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    /// <summary>
    /// تعداد کل صفحات.
    /// </summary>
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}