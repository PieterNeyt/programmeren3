using Neyt.Framework.Logging;

namespace Neyt.Tests;

public class MockLogger : ILogger
{
    public List<string> Messages { get; } = new List<string>();

   
    public void Log(string message, LogLevel level)
    {
        Messages.Add(message);
    }

    public void LogSection(string title)
    {
        Messages.Add($"SECTION: {title}");
    }
}