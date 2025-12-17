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
        Console.WriteLine("Available actions: Index, Help, Log, Timed, Mixed.");
    }

    [Action]
    public void Log()
    {
        _logger.Log("Demo: Calling method with [Log]", LogLevel.INFO);
        _interceptedService.DoSomethingImportant();
    }

    [Action]
    public void Timed()
    {
        _logger.Log("Demo: Calling method with [Timed]", LogLevel.INFO);
        _interceptedService.CalculateHugeSum();
    }

    [Action]
    public void Mixed()
    {
        _logger.Log("Demo: Calling method with [Log] AND [Timed]", LogLevel.INFO);
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