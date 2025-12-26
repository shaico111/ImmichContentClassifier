// Purpose: Provide simple structured logging to console and file with log levels.

namespace ImmichNsfwLocal.Logging;

public sealed class AppLogger : IDisposable
{
    private readonly LogLevel _level;
    private readonly StreamWriter? _fileWriter;
    private readonly bool _enableConsole;
    private readonly object _lock = new();

    public AppLogger(string logLevel, string? logFilePath, bool enableConsole)
    {
        _level = ParseLevel(logLevel);
        _enableConsole = enableConsole;

        if (!string.IsNullOrWhiteSpace(logFilePath))
        {
            var directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _fileWriter = new StreamWriter(File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                AutoFlush = true
            };
        }
    }

    public void Trace(string message) => Write(LogLevel.Trace, message);
    public void Debug(string message) => Write(LogLevel.Debug, message);
    public void Info(string message) => Write(LogLevel.Info, message);
    public void Warn(string message) => Write(LogLevel.Warn, message);
    public void Error(string message) => Write(LogLevel.Error, message);

    private void Write(LogLevel level, string message)
    {
        if (level < _level) return;

        string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

        lock (_lock)
        {
            if (_enableConsole)
            {
                if (level >= LogLevel.Warn) Console.ForegroundColor = ConsoleColor.Yellow;
                if (level >= LogLevel.Error) Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(logLine);
                Console.ResetColor();
            }

            _fileWriter?.WriteLine(logLine);
        }
    }

    public void Dispose() => _fileWriter?.Dispose();

    private static LogLevel ParseLevel(string raw) =>
        raw.Trim().ToLowerInvariant() switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            "info" => LogLevel.Info,
            "warn" => LogLevel.Warn,
            "error" => LogLevel.Error,
            _ => LogLevel.Info
        };
}

public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warn = 3,
    Error = 4
}
