namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// موجودیت پایه با حسابرسی، چندمستأجری، Soft Delete و کنترل همروندی خوش‌بینانه.
/// </summary>
/// <remarks>
/// نوع نهایی در زنجیره:
/// <code>BaseEntity → AuditableEntity → TenantEntity → FullAuditableEntity</code>.
/// ستون <see cref="Version"/> توسط پایگاه داده به‌صورت rowversion مدیریت می‌شود.
/// </remarks>
public abstract class FullAuditableEntity : TenantEntity
{
    /// <summary>
    /// Token کنترل همروندی (rowversion دیتابیس).
    /// </summary>
    public byte[] Version { get; set; } = Array.Empty<byte>();
}