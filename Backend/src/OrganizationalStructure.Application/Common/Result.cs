namespace OrganizationalStructure.Application.Common;

/// <summary>
/// نتیجه‌ی یک عملیات بدون مقدار بازگشتی (Success/Failure).
/// </summary>
public class Result
{
    /// <summary>
    /// نتیجه‌ی موفق.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// نتیجه‌ی ناموفق.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// خطای مرتبط با نتیجه‌ی ناموفق.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// مقداردهی اولیه نتیجه.
    /// </summary>
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// ساخت نتیجه‌ی موفق.
    /// </summary>
    /// <returns>نتیجه‌ی موفق</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// ساخت نتیجه‌ی ناموفق.
    /// </summary>
    /// <param name="error">خطا</param>
    /// <returns>نتیجه‌ی ناموفق</returns>
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// نتیجه‌ی یک عملیات با مقدار بازگشتی.
/// </summary>
/// <typeparam name="TValue">نوع مقدار بازگشتی در حالت موفق</typeparam>
public sealed class Result<TValue> : Result
{
    /// <summary>
    /// مقدار نتیجه‌ی موفق.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// مقداردهی اولیه نتیجه.
    /// </summary>
    private Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// ساخت نتیجه‌ی موفق با مقدار.
    /// </summary>
    /// <param name="value">مقدار</param>
    /// <returns>نتیجه‌ی موفق</returns>
    public static Result<TValue> Success(TValue value) => new(value, true, null);

    /// <summary>
    /// ساخت نتیجه‌ی ناموفق.
    /// </summary>
    /// <param name="error">خطا</param>
    /// <returns>نتیجه‌ی ناموفق</returns>
    public new static Result<TValue> Failure(Error error) => new(default, false, error);
}