namespace OrganizationalStructure.Application.Organizations.DTOs;

/// <summary>
/// DTO ساختار کامل یک سازمان (برای کش سامانه ارجاعات — §3.5 قرارداد).
/// </summary>
public sealed record OrganizationStructureDto
{
    /// <summary>
    /// شناسه سازمان.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// نام سازمان (از محدوده کاربر جاری).
    /// </summary>
    public string? OrganizationName { get; init; }

    /// <summary>
    /// لیست تخت پست‌ها با رابطه والد و مسئولیت‌ها.
    /// </summary>
    public IReadOnlyList<OrganizationPostStructureDto> Posts { get; init; } =
        Array.Empty<OrganizationPostStructureDto>();
}

/// <summary>
/// DTO یک پست در ساختار سازمان (نمای تخت برای ساخت درخت در مصرف‌کننده).
/// </summary>
public sealed record OrganizationPostStructureDto
{
    /// <summary>
    /// شناسه پست.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// کد پست.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// عنوان پست.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// شناسه والد مستقیم (خالی یعنی ریشه).
    /// </summary>
    public Guid? ParentId { get; init; }

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// آیا صاحب‌امضا است؟
    /// </summary>
    public bool HasSigningAuthority { get; init; }

    /// <summary>
    /// کدهای مسئولیت‌های جاری منتسب به این پست.
    /// </summary>
    public IReadOnlyList<string> ResponsibilityCodes { get; init; } =
        Array.Empty<string>();
}
