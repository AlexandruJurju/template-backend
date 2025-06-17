using Domain.Abstractions.Result;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class ControllerExtensions
{
    public static IActionResult Problem(this ControllerBase controller, Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem from a successful result");
        }

        Error error = result.Error;
        int statusCode = GetStatusCode(error.Type);
        
        ObjectResult problemDetails = controller.Problem(
            type: GetType(error.Type),
            title: GetTitle(error),
            detail: GetDetail(error),
            statusCode: statusCode);

        // Add validation errors to ProblemDetails if applicable
        if (error is ValidationError validationError)
        {
            var problemDetailsObj = (ProblemDetails)problemDetails.Value!;
            problemDetailsObj.Extensions["errors"] = validationError.Errors;
        }

        return problemDetails;

        static string GetTitle(Error error) =>
            error.Type switch
            {
                ErrorType.Validation => error.Code,
                ErrorType.Problem => error.Code,
                ErrorType.NotFound => error.Code,
                ErrorType.Conflict => error.Code,
                _ => "Server failure"
            };

        static string GetDetail(Error error) =>
            error.Type switch
            {
                ErrorType.Validation => error.Description,
                ErrorType.Problem => error.Description,
                ErrorType.NotFound => error.Description,
                ErrorType.Conflict => error.Description,
                _ => "An unexpected error occurred"
            };

        static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

        static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Problem => StatusCodes.Status400BadRequest, 
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
