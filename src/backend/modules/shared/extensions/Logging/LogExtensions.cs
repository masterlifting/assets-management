using Microsoft.Extensions.Logging;
using Shared.Exceptions.Abstractions;

// ReSharper disable ConvertToConstant.Local

namespace Shared.Extensions.Logging;

public static class LogExtensions
{
    private static readonly string Pattern = "[{time:dd-MM HH:mm:ss}] - {initiator}. {action}. {result}. {info}";
    private static readonly string ErrorPattern = "[{time:dd-MM HH:mm:ss}] - Error: {error}";

    private static readonly Action<ILogger, DateTime, string, Exception?> Error =
        LoggerMessage.Define<DateTime, string>(LogLevel.Error, LogEvents.Error, ErrorPattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Warning =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Warning, LogEvents.Warning, Pattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Information =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Information, LogEvents.Information, Pattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Debug =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Debug, LogEvents.Debug, Pattern);
    private static readonly Action<ILogger, DateTime, string, string, string, object?, Exception?> Trace =
        LoggerMessage.Define<DateTime, string, string, string, object?>(LogLevel.Trace, LogEvents.Trace, Pattern);


    public static void LogError(this ILogger logger, SharedException exception) =>
        Error(logger, DateTime.UtcNow, exception.InnerException?.Message ?? exception.Message, null);
    public static void LogWarn(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Warning(logger, DateTime.UtcNow, initiator, action, result, info ?? string.Empty, null);
    public static void LogInfo(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Information(logger, DateTime.UtcNow, initiator, action, result, info ?? string.Empty, null);
    public static void LogDebug(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Debug(logger, DateTime.UtcNow, initiator, action, result, info ?? string.Empty, null);
    public static void LogTrace(this ILogger logger, string initiator, string action, string result, object? info = null) =>
        Trace(logger, DateTime.UtcNow, initiator, action, result, info ?? string.Empty, null);

    private static class LogEvents
    {
        public static readonly EventId Error = new(1000, "error native log");
        public static readonly EventId Warning = new(1001, "warning native log");
        public static readonly EventId Information = new(1002, "information native log");
        public static readonly EventId Debug = new(1003, "debug native log");
        public static readonly EventId Trace = new(1004, "trace native log");
    }
}