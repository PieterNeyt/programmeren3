using Neyt.Demo.Services.Lifecycle.Bad;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Reflection;

[Component]
public class SystemController
{
    
    private readonly ILogger _logger;

    public SystemController(ILogger logger)
    {
        _logger = logger;
    }
    
    [Action]
    public void Cycle()
    {
        _logger.Log("[System] Attempting to build a container with circular dependencies...", LogLevel.INFO);
        
        var badServices = new DiServiceCollection();
        
        badServices.AddSingleton<BadServiceA, BadServiceA>();
        badServices.AddSingleton<BadServiceB, BadServiceB>();
        badServices.AddSingleton<BadServiceC, BadServiceC>();

        try
        {
            var container = badServices.BuildServiceProvider();
            _logger.Log("[FAIL] Container built unexpectedly!", LogLevel.WARNING);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            _logger.Log($"[SUCCESS] Caught expected error: {ex.Message}", LogLevel.INFO);
            Console.ResetColor();
        }
    }
}