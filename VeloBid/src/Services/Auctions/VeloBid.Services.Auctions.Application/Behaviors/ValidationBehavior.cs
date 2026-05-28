using FluentValidation;
using MediatR;
using VeloBid.Services.Auctions.Application.Common;

namespace VeloBid.Services.Auctions.Application.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailure => validationFailure is not null)
            .ToArray();

        if (errors.Length == 0)
        {
            return await next(cancellationToken);
        }

        var errorMessage = string.Join(
            "; ",
            errors.Select(error => error.ErrorMessage));

        var responseType = typeof(TResponse);

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(Result<object>.Failure));

            var failureResult = failureMethod!.Invoke(
                null,
                [Error.Validation(errorMessage)]);

            return (TResponse)failureResult!;
        }

        throw new ValidationException(errors);
    }
}