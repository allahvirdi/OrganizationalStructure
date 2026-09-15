namespace OrganizationalStructure.Application.Authorities.DTOs;

/// <summary>
/// DTO اختیار سازمانی.
/// </summary>
public sealed record AuthorityDto
{
    /// <summary>
    /// شناسه اختیار.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// کد یکتا.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// عنوان.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// شرح.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO انتساب اختیار به پست.
/// </summary>
public sealed record AuthorityAssignmentDto
{
    /// <summary>
    /// شناسه انتساب.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// شناسه اختیار.
    /// </summary>
    public Guid AuthorityId { get; init; }

    /// <summary>
    /// کد اختیار.
    /// </summary>
    public string AuthorityCode { get; init; } = string.Empty;

    /// <summary>
    /// عنوان اختیار.
    /// </summary>
    public string AuthorityTitle { get; init; } = string.Empty;

    /// <summary>
    /// شناسه سازمان (Scope).
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// شناسه پست.
    /// </summary>
    public Guid PostId { get; init; }

    /// <summary>
    /// کد پست.
    /// </summary>
    public string PostCode { get; init; } = string.Empty;

    /// <summary>
    /// عنوان پست.
    /// </summary>
    public string PostTitle { get; init; } = string.Empty;

    /// <summary>
    /// تاریخ شروع.
    /// </summary>
    public DateOnly? StartDate { get; init; }

    /// <summary>
    /// تاریخ پایان.
    /// </summary>
    public DateOnly? EndDate { get; init; }

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }
}