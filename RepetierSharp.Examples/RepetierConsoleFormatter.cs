using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace RepetierSharp.Examples;

public class RepetierConsoleFormatter() : ConsoleFormatter("RepetierConsole")
{
    private static readonly Dictionary<LogLevel, (string Abbreviation, string Color)> LogLevelMap = new()
    {
        { LogLevel.Trace,       ("TRC", "\u001b[37m") }, // White
        { LogLevel.Debug,       ("DBG", "\u001b[36m") }, // Cyan
        { LogLevel.Information, ("INF", "\u001b[32m") }, // Green
        { LogLevel.Warning,     ("WRN", "\u001b[33m") }, // Yellow
        { LogLevel.Error,       ("ERR", "\u001b[31m") }, // Red
        { LogLevel.Critical,    ("CRT", "\u001b[35m") }, // Magenta
        { LogLevel.None,        ("NON", "\u001b[37m") }  // White
    };

    
    public override void Write<TState>(
        in LogEntry<TState> logEntry, 
        IExternalScopeProvider? scopeProvider, 
        TextWriter textWriter)
    {
        // Custom timestamp format
        // var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var (logAbbreviation, color) = LogLevelMap.GetValueOrDefault(logEntry.LogLevel, ("UNK", "\u001b[37m"));
        // Custom log message
        var logLevel = logEntry.LogLevel.ToString().ToLower()[..4];
        var categoryName = logEntry.Category.Split(".").Last();
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        textWriter.WriteLine($"{timestamp} {color}[{logAbbreviation}]\u001b[0m [{categoryName}] {message}");
    }
}
