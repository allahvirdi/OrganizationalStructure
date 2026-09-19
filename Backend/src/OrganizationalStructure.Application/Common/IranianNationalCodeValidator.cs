namespace OrganizationalStructure.Application.Common;

/// <summary>
/// اعتبارسنج کد ملی ایران بر اساس الگوریتم رسمی چک‌سام.
/// </summary>
/// <remarks>
/// الگوریتم: کد ملی ۱۰ رقم است. ارقام اول تا نهم در ضرایب ۱۰ تا ۲ ضرب می‌شوند.
/// باقی‌مانده مجموع بر ۱۱ محاسبه می‌شود. اگر باقی‌مانده کمتر از ۲ باشد،
/// رقم کنترل برابر باقی‌مانده است؛ در غیر این صورت برابر ۱۱ منهای باقی‌مانده.
/// </remarks>
public static class IranianNationalCodeValidator
{
    /// <summary>
    /// بررسی اعتبار کد ملی ایران.
    /// </summary>
    /// <param name="nationalCode">کد ملی ورودی</param>
    /// <returns>در صورت معتبر بودن true</returns>
    public static bool IsValid(string? nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            return false;
        }

        var digits = nationalCode.Trim();

        if (digits.Length != 10 || !digits.All(char.IsDigit))
        {
            return false;
        }

        // رد کدهایی که تمام ارقام یکسان هستند
        if (digits.Distinct().Count() == 1)
        {
            return false;
        }

        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (digits[i] - '0') * (10 - i);
        }

        var remainder = sum % 11;
        var checkDigit = digits[9] - '0';

        var expectedControl = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == expectedControl;
    }
}