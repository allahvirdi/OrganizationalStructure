namespace OrganizationalStructure.Domain.ValueObjects;

/// <summary>
/// سابقه خدمت در حراست (به شکل «سال و ماه»).
/// </summary>
/// <remarks>
/// Value Object تغییرناپذیر و غیر PII است (مدت خدمت، غیر هویتی).
/// </remarks>
public sealed record HerasatServiceRecord
{
    /// <summary>
    /// سال‌های خدمت (صفر یا بیشتر).
    /// </summary>
    public int Years { get; init; }

    /// <summary>
    /// ماه‌های خدمت (۰ تا ۱۱).
    /// </summary>
    public int Months { get; init; }

    /// <summary>
    /// ساخت سابقه خدمت.
    /// </summary>
    /// <param name="years">سال‌ها (۰ یا بیشتر)</param>
    /// <param name="months">ماه‌ها (۰ تا ۱۱)</param>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن بازه</exception>
    public HerasatServiceRecord(int years, int months)
    {
        if (years < 0)
        {
            throw new ArgumentException("سال سابقه نمی‌تواند منفی باشد.", nameof(years));
        }

        if (months is < 0 or > 11)
        {
            throw new ArgumentException("ماه سابقه باید بین ۰ تا ۱۱ باشد.", nameof(months));
        }

        Years = years;
        Months = months;
    }

    /// <summary>
    /// نمایش متنی «X سال و Y ماه».
    /// </summary>
    /// <returns>نمایش متنی سابقه</returns>
    public override string ToString() => $"{Years} سال و {Months} ماه";
}