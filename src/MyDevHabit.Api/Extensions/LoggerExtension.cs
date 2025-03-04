namespace MyDevHabit.Api.Extensions;

internal static class LoggerExtension
{
    public static void LogInformation(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Information, new EventId(), message, null, (state, ex) => state.ToString());
    }
}
