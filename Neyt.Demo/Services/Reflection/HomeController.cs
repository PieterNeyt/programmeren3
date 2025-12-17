using Neyt.Framework;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Reflection;

[Component] 
public class HomeController
{
    private readonly ILogger _logger;
    private readonly InterceptedService _interceptedService;
    
    public HomeController(ILogger logger, InterceptedService interceptedService)
    {
        _logger = logger;
        _interceptedService = interceptedService;
    }

    [Action] 
    public void Index()
    {
        _logger.LogSection("Home Page");
        Console.WriteLine("Welkom! Probeer commando's zoals 'Home Timed' of 'Home Log'.");
    }

    [Action]
    public void Help()
    {
        Console.WriteLine("Beschikbare acties: Index, Help, Log, Timed, Mixed.");
    }

    [Action]
    public void Log()
    {
        Console.WriteLine("Demo: Calling method with [Log]...");
        _interceptedService.DoSomethingImportant();
    }

    [Action]
    public void Timed()
    {
        Console.WriteLine("Demo: Calling method with [Timed]...");
        _interceptedService.CalculateHugeSum();
    }

    [Action]
    public void Mixed()
    {
        Console.WriteLine("Demo: Calling method with [Log] AND [Timed]...");
        _interceptedService.CombinedAction();
    }
}