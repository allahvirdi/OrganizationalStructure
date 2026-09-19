namespace OrganizationalStructure.Application.Organizations.DTOs;

/// <summary>
/// گزینه سازمان برای انتخاب/جستجو در UI.
/// </summary>
/// <remarks>
/// فقط شامل سازمان کاربر جاری و زیرمجموعه‌های آن است؛ «شناسه خام» تنها برای ارسال در
/// بدنه درخواست‌ها استفاده می‌شود و در UI نام سازمان نمایش داده می‌شود.
/// </remarks>
/// <param name="Id">شناسه سازمان.</param>
/// <param name="Name">نام نمایشی سازمان.</param>
/// <param name="Code">کد سازمان.</param>
/// <param name="ParentId">شناسه سازمان والد در محدوده (خالی یعنی سازمان خود کاربر).</param>
/// <param name="Depth">عمق نسبت به سازمان کاربر (سازمان خود کاربر = ۰).</param>
/// <param name="IsCurrent">آیا سازمان خود کاربر است؟ (برای انتخاب پیش‌فرض در فرم‌ها)</param>
public sealed record OrganizationOptionDto(
    Guid Id,
    string Name,
    string Code,
    Guid? ParentId,
    int Depth,
    bool IsCurrent);
