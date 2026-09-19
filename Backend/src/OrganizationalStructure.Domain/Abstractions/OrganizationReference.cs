namespace OrganizationalStructure.Domain.Abstractions;

/// <summary>
/// مرجع یک واحد سازمانی داخل محدوده مشاهده کاربر (خود سازمان + زیرمجموعه‌ها).
/// </summary>
/// <remarks>
/// Master سازمان در IAM است و این سامانه فقط Reference نگه می‌دارد (ADR-002)؛
/// این مرجع برای نمایش «نام» سازمان در UI و انتخاب سازمان در فرم‌ها استفاده می‌شود
/// و هیچ‌گاه جایگزین Master نمی‌شود.
/// </remarks>
/// <param name="Id">شناسه واحد سازمانی (از IAM).</param>
/// <param name="Name">نام نمایشی واحد سازمانی (از IAM).</param>
/// <param name="Code">کد واحد سازمانی (از IAM).</param>
/// <param name="ParentId">شناسه واحد والد در محدوده (خالی یعنی ریشه محدوده = سازمان کاربر).</param>
/// <param name="Depth">عمق نسبت به سازمان کاربر (سازمان خود کاربر = ۰).</param>
public sealed record OrganizationReference(
    Guid Id,
    string Name,
    string Code,
    Guid? ParentId,
    int Depth);
