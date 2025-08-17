using System.Reflection;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Template.Common.SharedKernel.Application.CQRS.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // No validators? Let it through.
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        // Run all validators for the request
        ValidationResult[] results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Any failures?
        var hasFailures = results.Any(r => !r.IsValid);
        if (!hasFailures)
        {
            return await next(cancellationToken);
        }

        // Convert all failures to Ardalis.ValidationError instances using your extension
        var errors = results
            .Where(r => !r.IsValid)
            .SelectMany(r => r.AsErrors())
            .ToList();

        // Non-generic Result
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Invalid(errors);
        }

        // Generic Result<T>
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];

            // Find Result<T>.Invalid(IEnumerable<ValidationError>) and call it
            MethodInfo? invalidMethod = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod(
                    nameof(Result.Invalid),
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    [typeof(IEnumerable<ValidationError>)],
                    null);

            if (invalidMethod is not null)
            {
                return (TResponse)invalidMethod.Invoke(null, [errors])!;
            }
        }

        // If the handler doesn't return Ardalis.Result/Result<T>, fall back to throwing
        var allFailures = results.SelectMany(r => r.Errors).ToList();
        throw new ValidationException(allFailures);
    }
}
