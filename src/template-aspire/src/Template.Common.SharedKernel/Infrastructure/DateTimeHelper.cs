namespace Template.Common.SharedKernel.Infrastructure;

public static class DateTimeHelper
{
    public static DateTimeOffset UtcNowOffset()
    {
        return TimeProvider.System.GetUtcNow();
    }

    public static DateTime UtcNow()
    {
        return TimeProvider.System.GetUtcNow().UtcDateTime;
    }
}
