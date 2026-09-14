namespace OrganizationalStructure.Domain.Abstractions;

/// <summary>
/// Abstraction ساعت برای انجام آزمایش‌پذیری و کنترل زمان در کل سامانه.
/// </summary>
/// <remarks>
/// استفاده مستقیم از <c>DateTime.Now</c> یا <c>DateTime.UtcNow</c> در کد ممنوع است؛
/// باید از این Abstraction استفاده شود (Fitness Function مربوطه نیز بررسی می‌کند).
/// </remarks>
public interface IClock
{
    /// <summary>
    /// زمان فعلی به‌صورت UTC.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}