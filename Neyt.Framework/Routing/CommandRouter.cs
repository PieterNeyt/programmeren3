using System.Reflection;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;

namespace Neyt.Framework.Routing;

public class CommandRouter
{
    private readonly DiContainer _container;
    private readonly ILogger _logger;
    
    private readonly Dictionary<string, Type> _controllerCache;

    public CommandRouter(DiContainer container, Assembly assemblyToScan, ILogger logger)
    {
        _container = container;
        _logger = logger;
        
        _controllerCache = assemblyToScan.GetTypes()
            .Where(t => t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) && !t.IsAbstract && t.IsClass)
            .ToDictionary(
                keySelector: t => t.Name.Substring(0, t.Name.Length - "Controller".Length),
                elementSelector: t => t,
                comparer: StringComparer.OrdinalIgnoreCase
            );
    }

    public void HandleInput(string inputLine)
    {
        if (string.IsNullOrWhiteSpace(inputLine)) return;

        var parts = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            Console.WriteLine("Use the format: [ControllerName] [ActionName] (ex: Home Index)");
            return;
        }

        var controllerName = parts[0]; 
        var actionName = parts[1]; 
        
        if (!_controllerCache.TryGetValue(controllerName, out var controllerType))
        {
            Console.WriteLine($"Controller '{controllerName}' not found (or doesn't end with 'Controller').");
            return;
        }

        try
        {
            var controllerInstance = _container.GetService(controllerType);
            
            var method = controllerType.GetMethod(actionName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (method == null)
            {
                Console.WriteLine($"Action '{actionName}' not found in {controllerType.Name}.");
                return;
            }
            
            // Checken op [Action] attribuut
            if (!method.GetCustomAttributes(typeof(ActionAttribute), true).Any())
            {
                Console.WriteLine($"Method '{actionName}' is not public [Action].");
                return;
            }
            
            _logger.Log($"[Router] Invoking {controllerType.Name}.{method.Name}");
            method.Invoke(controllerInstance, null); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error by invoking action: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Details: {ex.InnerException.Message}");
        }
    }
}