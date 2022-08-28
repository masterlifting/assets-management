using Microsoft.Extensions.Logging;
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Shared.Core.Extensions;

public static class LoggerExtensions
{
    private static readonly string pattern = "[{time:dd-MM HH:mm:ss}] - {initiator}. {action}. {result}. {info}";
    private static readonly string errorPattern = "[{time:dd-MM HH:mm:ss}] - {initiator}. {action}. Error message: {error}";

    private static readonly Action<ILogger, DateTime, string, string, string, Exception?> Error =
        LoggerMessage.Define<DateTime, string, string, string>(LogLevel.Error, LogEvents.Error, errorPattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Warning =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Warning, LogEvents.Warning, pattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Information =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Information, LogEvents.Information, pattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Debug =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Debug, LogEvents.Debug, pattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Trace =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Trace, LogEvents.Trace, pattern);


    public static void LogError(this ILogger logger, string initiator, string action, Exception exception) =>
        Error(logger, DateTime.UtcNow, initiator, action, exception.InnerException?.Message ?? exception.Message, null);
    public static void LogWarn(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Warning(logger, DateTime.UtcNow, initiator, action, result, info, null);
    public static void LogInfo(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Information(logger, DateTime.UtcNow, initiator, action, result, info, null);
    public static void LogDebug(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Debug(logger, DateTime.UtcNow, initiator, action, result, info, null);
    public static void LogTrace(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Trace(logger, DateTime.UtcNow, initiator, action, result, info, null);

    private static class LogEvents
    {
        public static readonly EventId Error = new(1000, "error native log");
        public static readonly EventId Warning = new(1001, "warning native log");
        public static readonly EventId Information = new(1002, "information native log");
        public static readonly EventId Debug = new(1003, "debug native log");
        public static readonly EventId Trace = new(1004, "trace native log");

    }
}