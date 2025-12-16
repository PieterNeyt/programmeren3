namespace Neyt.Framework.Logging;

public interface ILogger
{
    void Log(string message);
    void LogSection(string title); 
}