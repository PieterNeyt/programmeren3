namespace Neyt.Framework.Logging;

public class ConsoleLogger : ILogger
{
    
    public LogLevel MinLevel { get; set; } = LogLevel.INFO; 

    public void Log(string message, LogLevel level)
    {
        if (level >= MinLevel)
        {
            var color = level == LogLevel.DEBUG ? ConsoleColor.Gray : ConsoleColor.White;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
            Console.ResetColor();
        }
    }

    public void LogSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n=== {title} ===");
        Console.ResetColor();
    }
}