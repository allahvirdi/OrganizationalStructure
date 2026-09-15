namespace OrganizationalStructure.Application.Responsibilities.DTOs;

/// <summary>
/// DTO مسئولیت سازمانی.
/// </summary>
public sealed record ResponsibilityDto
{
    /// <summary>
    /// شناسه مسئولیت.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// کد یکتا (Business Routing Key).
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
/// DTO انتساب مسئولیت به پست.
/// </summary>
public sealed record ResponsibilityAssignmentDto
{
    /// <summary>
    /// شناسه انتساب.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// شناسه مسئولیت.
    /// </summary>
    public Guid ResponsibilityId { get; init; }

    /// <summary>
    /// کد مسئولیت.
    /// </summary>
    public string ResponsibilityCode { get; init; } = string.Empty;

    /// <summary>
    /// عنوان مسئولیت.
    /// </summary>
    public string ResponsibilityTitle { get; init; } = string.Empty;

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