using Neyt.Framework;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Reflection;

[Component] 
public class HomeController
{
    private readonly ILogger _logger;
    
    public HomeController(ILogger logger)
    {
        _logger = logger;
    }

    [Action] 
    public void Index()
    {
        _logger.LogSection("Home Page");
        Console.WriteLine("Welkom op de Home Controller!");
    }

    [Action]
    public void Help()
    {
        Console.WriteLine("Dit is de help functie.");
    }
    
    public void InternalLogic()
    {
        Console.WriteLine("Dit mag je niet zien.");
    }
}