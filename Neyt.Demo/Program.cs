using System.Reflection;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;
using Neyt.Framework.Routing;

Console.WriteLine("============================================");
Console.WriteLine("   Neyt DI Container - Final Demo App");
Console.WriteLine("============================================");

//  Log Level Selectie
Console.WriteLine("Select Log Level:");
Console.WriteLine(" [1] Normal (Info)  - Shows application logs only");
Console.WriteLine(" [2] Verbose (Debug) - Shows container internals (creation/resolving)");
Console.Write("Choice [1]: ");
var choice = Console.ReadLine();

LogLevel selectedLevel = choice?.Trim() == "2" ? LogLevel.Debug : LogLevel.Info;

var myLogger = new ConsoleLogger { MinLevel = selectedLevel };
Console.WriteLine($"[System] Logger configured to: {selectedLevel}");

//  Service Collection Initialisatie
var services = new DiServiceCollection();
var assembly = Assembly.GetExecutingAssembly();

// logger registreren
services.AddSingleton<ILogger>(myLogger);
services.RegisterServicesByAttribute(assembly);

// Assembly Scanning 
services.RegisterByScanning(assembly, type => type.GetCustomAttributes(typeof(NeytControllerAttribute), true).Any());

//  Container Bouwen
var container = services.BuildServiceProvider();
Console.WriteLine("[System] Container built successfully.");

var logger = container.GetService<ILogger>();
var router = new CommandRouter(container, assembly, logger);

Console.WriteLine("\n[System] Router started. Type 'exit' to quit.");
Console.WriteLine("--------------------------------------------");
Console.WriteLine("NORMAL commands:");
Console.WriteLine(" > Home Index      (Interface Injection Demo)");
Console.WriteLine(" > Home Help");
Console.WriteLine(" > Home Log        ([Log] Interception Demo)");
Console.WriteLine(" > Home Timed      ([Timed] Interception Demo)");
Console.WriteLine(" > Home Mixed      (Combined Interception Demo)");
Console.WriteLine("");
Console.WriteLine("ADVANCED commands:");
Console.WriteLine(" > Hierarchy Run   (Deep Interface Injection: Controller -> IMain -> ISub)");
Console.WriteLine(" > System Greedy   (Greedy Constructor)");
Console.WriteLine(" > System Deep     (Long dependency chain: A -> B -> C)");
Console.WriteLine("");
Console.WriteLine("ERROR commands:");
Console.WriteLine(" > System Cycle    (Attempts to build cyclic container -> Expect Error)");
Console.WriteLine(" > Home Plain      (Public method without [Action] -> Expect Block)");
Console.WriteLine(" > Home Secret     (Private method -> Expect Block)");
Console.WriteLine(" > Home Fake       (Non-existent method -> Expect Block)");
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