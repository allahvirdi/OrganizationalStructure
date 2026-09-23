using OrganizationalStructure.Application.Authorities.DTOs;
using OrganizationalStructure.Application.Responsibilities.DTOs;

namespace OrganizationalStructure.Application.Posts.DTOs;

/// <summary>
/// DTO تفصیلی پست سازمانی (همراه انتساب‌های جاری مسئولیت و اختیار).
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
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// آیا پست در حال حاضر صاحب‌امضا است؟
    /// (وجود حداقل یک انتساب جاری حق امضا — DEC-035)
    /// </summary>
    public bool HasSigningAuthority { get; init; }

    /// <summary>
    /// انتساب‌های جاری مسئولیت.
    /// </summary>
    public IReadOnlyList<ResponsibilityAssignmentDto> Responsibilities { get; init; } =
        Array.Empty<ResponsibilityAssignmentDto>();

    /// <summary>
    /// انتساب‌های جاری اختیار.
    /// </summary>
    public IReadOnlyList<AuthorityAssignmentDto> Authorities { get; init; } =
        Array.Empty<AuthorityAssignmentDto>();
}

/// <summary>
/// DTO خلاصه پست (فهرست‌ها و جستجو — بدون انتساب‌ها).
/// </summary>
public sealed record PostSummaryDto
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
    /// نام سازمان در محدوده مشاهده کاربر جاری (خالی اگر سازمان در محدوده نباشد).
    /// </summary>
    public string? OrganizationName { get; init; }

    /// <summary>
    /// کد پست.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// عنوان پست.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// شناسه والد مستقیم.
    /// </summary>
    public Guid? ParentId { get; init; }

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// آیا پست در حال حاضر صاحب‌امضا است؟
    /// </summary>
    public bool HasSigningAuthority { get; init; }

    /// <summary>
    /// عنوان مسئولیت‌های جاری پست.
    /// </summary>
    public IReadOnlyList<string> ResponsibilityTitles { get; init; } =
        Array.Empty<string>();

    /// <summary>
    /// عنوان حق امضاهای جاری پست.
    /// </summary>
    public IReadOnlyList<string> AuthorityTitles { get; init; } =
        Array.Empty<string>();
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
    /// آیا پست در حال حاضر صاحب‌امضا است؟
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