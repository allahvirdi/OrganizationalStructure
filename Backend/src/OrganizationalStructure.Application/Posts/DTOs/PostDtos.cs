namespace OrganizationalStructure.Application.Posts.DTOs;

/// <summary>
/// نمایشی از مسئولیت پست برای انتقال داده.
/// </summary>
/// <param name="Title">عنوان مسئولیت</param>
/// <param name="Description">شرح مسئولیت</param>
public sealed record ResponsibilityDto(
    string Title,
    string? Description);

/// <summary>
/// DTO پست سازمانی.
/// </summary>
public sealed record PostDto
{
    /// <summary>
    /// شناسه پست.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// شناسه سازمان (مرجع IAM).
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// کد پست.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// عنوان پست.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// شرح پست.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// شناسه والد مستقیم.
    /// </summary>
    public Guid? ParentId { get; init; }

    /// <summary>
    /// آیا صاحب امضا است؟
    /// </summary>
    public bool HasSigningAuthority { get; init; }

    /// <summary>
    /// مسئولیت‌ها.
    /// </summary>
    public IReadOnlyList<ResponsibilityDto> Responsibilities { get; init; } =
        Array.Empty<ResponsibilityDto>();

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO درختی پست (گره + فرزندان بازگشتی).
/// </summary>
public sealed record PostTreeDto
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
    /// آیا صاحب امضا است؟
    /// </summary>
    public bool HasSigningAuthority { get; init; }

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// فرزندان مستقیم.
    /// </summary>
    public List<PostTreeDto> Children { get; init; } = new();
}