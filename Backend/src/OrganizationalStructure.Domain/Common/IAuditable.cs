namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// قرارداد موجودیت دارای ویژگی‌های حسابرسی (Audit).
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// زمان ایجاد رکورد (به‌صورت UTC).
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// شناسه کاربر ایجادکننده رکورد.
    /// </summary>
    Guid CreatedById { get; set; }

    /// <summary>
    /// زمان آخرین به‌روزرسانی رکورد (خالی در صورت عدم ویرایش).
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// شناسه کاربر آخرین ویرایش‌کننده رکورد.
    /// </summary>
    Guid? UpdatedById { get; set; }
}