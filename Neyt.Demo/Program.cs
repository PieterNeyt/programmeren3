using System.Reflection;
using Neyt.Demo.Services.Basics;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;
using Neyt.Framework.Routing;

Console.WriteLine("============================================");
Console.WriteLine("   Neyt DI Container - Final Demo App");
Console.WriteLine("============================================");

Console.WriteLine("Select Log Level:");
Console.WriteLine(" [1] Normal (Info)  - Shows application logs only");
Console.WriteLine(" [2] Verbose (Debug) - Shows container internals (creation/resolving)");
Console.Write("Choice [1]: ");
var choice = Console.ReadLine();

LogLevel selectedLevel;
if (choice?.Trim() == "2")
{
    selectedLevel= LogLevel.DEBUG;
}
else
{
   selectedLevel= LogLevel.INFO;
}

var myLogger = new ConsoleLogger();
myLogger.MinLevel = selectedLevel;

Console.WriteLine($"[System] Logger configured to: {selectedLevel}");


// Service Collection aanmaken
var services = new DiServiceCollection();
var assembly = Assembly.GetExecutingAssembly();

services.AddSingleton<ILogger>(myLogger);

// Interface Singleton registratie
services.AddSingleton<IGreetingService, GreetingService>();

// Assembly Scanning
Console.WriteLine("[System] Scanning assembly for components...");
services.RegisterByScanning(assembly, type => type.GetCustomAttributes(typeof(ComponentAttribute), true).Any());

DiContainer container;

try
{
    container = services.BuildServiceProvider();
    Console.WriteLine("[System] Container built successfully.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[FATAL] Startup failed due to dependency cycle: {ex.Message}");
    Console.ResetColor();
    return;
}

var logger = container.GetService<ILogger>();
var router = new CommandRouter(container, assembly, logger);

Console.WriteLine("\n[System] Router started. Type 'exit' to quit.");
Console.WriteLine("--------------------------------------------");
Console.WriteLine("Try NORMAL commands:");
Console.WriteLine(" > Home Index      (Shows Interface Injection)");
Console.WriteLine(" > Home Log        (Demonstrates [Log])");
Console.WriteLine(" > Home Timed      (Demonstrates [Timed])");
Console.WriteLine(" > Home Mixed      (Demonstrates Both)");
Console.WriteLine("");
Console.WriteLine("Try ERROR commands (Validation Tests):");
Console.WriteLine(" > System Cycle    (Attempts to build cyclic dependency -> Expect Error)");
Console.WriteLine(" > Home Plain      (Public method without [Action] -> Expect Error)");
Console.WriteLine(" > Home Secret     (Private method -> Expect Error)");
Console.WriteLine(" > Home Fake       (Non-existent method -> Expect Error)");
Console.WriteLine("--------------------------------------------\n");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Input > ");
    Console.ResetColor();

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) continue;
    if (input.Trim().ToLower() == "exit") break;

    router.HandleInput(input);
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");