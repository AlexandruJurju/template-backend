using Ardalis.Result;

namespace Template.SharedKernel.Application;

public static class ResultExtensions
{
    public static bool IsFailure<T>(this Result<T> result)
    {
        return !result.IsSuccess;
    }
}
