using FluentValidation;
using MediatR;
using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Behaviors;

/// <summary>
/// رفتار خط لوله MediatR برای اجرای خودکار اعتبارسنجی FluentValidation.
/// </summary>
/// <typeparam name="TRequest">نوع درخواست</typeparam>
/// <typeparam name="TResponse">نوع پاسخ (باید <see cref="Result"/> یا مشتق آن باشد)</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="validators">اعتبارسنج‌های ثبت‌شده برای درخواست</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        var message = string.Join("؛ ", failures.Select(f => f.ErrorMessage));
        var error = new Error("Validation.Failed", message, ErrorType.Validation);

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        var responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod(
                nameof(Result<object>.Failure),
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!;
            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new ValidationException("خطای اعتبارسنجی رخ داد.", failures);
    }
}