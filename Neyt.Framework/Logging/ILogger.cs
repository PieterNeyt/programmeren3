namespace Neyt.Framework.Logging;

public interface ILogger
{
    void Log(string message, LogLevel level);
    void LogSection(string title);
}