using Neyt.Demo.Services.Basics;
using Neyt.Framework.Attributes;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Reflection;

[Component]
public class HomeController
{
    private readonly ILogger _logger;
    private readonly InterceptedService _interceptedService;
    private readonly IGreetingService _greetingService;
    
    public HomeController(ILogger logger, InterceptedService interceptedService, IGreetingService greetingService)
    {
        _logger = logger;
        _interceptedService = interceptedService;
        _greetingService = greetingService;
    }

    [Action]
    public void Index()
    {
        _logger.LogSection("Home Page");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(_greetingService.GetWelcomeMessage());
        Console.ResetColor();

        Console.WriteLine("Welcome! Try different commands like 'Home Timed' or 'System Cycle'.");
    }

    [Action]
    public void Help()
    {
        Console.WriteLine("Available actions acties: Index, Help, Log, Timed, Mixed.");
    }

    [Action]
    public void Log()
    {
        Console.WriteLine("Demo: Calling method with [Log]");
        _interceptedService.DoSomethingImportant();
    }

    [Action]
    public void Timed()
    {
        Console.WriteLine("Demo: Calling method with [Timed]");
        _interceptedService.CalculateHugeSum();
    }

    [Action]
    public void Mixed()
    {
        Console.WriteLine("Demo: Calling method with [Log] AND [Timed]");
        _interceptedService.CombinedAction();
    }
    
    public void Plain()
    {
        Console.WriteLine("Router should block this request since there's no [Action] attribute.");
    }
    
    [Action]
    private void Secret()
    {
        Console.WriteLine("Router should block this request since this method is private.");
    }
}