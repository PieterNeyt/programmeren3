using System.Reflection;
using Neyt.Framework.Logging;

namespace Neyt.Framework;

public class CommandRouter
{
    private readonly DiContainer _container;
    private readonly Assembly _assemblyToScan;
    private readonly ILogger _logger;

    public CommandRouter(DiContainer container, Assembly assemblyToScan, ILogger logger)
    {
        _container = container;
        _assemblyToScan = assemblyToScan;
        _logger = logger;
    }

    public void HandleInput(string inputLine)
    {
        if (string.IsNullOrWhiteSpace(inputLine)) return;

        // Input splitsen
        var parts = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            Console.WriteLine("Gebruik formaat: [ControllerNaam] [ActieNaam]");
            return;
        }

        var controllerName = parts[0]; 
        var actionName = parts[1]; 

        // controller zoeken met Type 
        var controllerType = _assemblyToScan.GetTypes()
            .FirstOrDefault(t => t.Name.Equals($"{controllerName}Controller", StringComparison.OrdinalIgnoreCase));

        if (controllerType == null)
        {
            Console.WriteLine($"Controller '{controllerName}' niet gevonden.");
            return;
        }

        try
        {
            // instantie van controller ophalen via DI container
            var controllerInstance = _container.GetService(controllerType);

            // zoeken naar methode in opgehaalde controller
            var method = controllerType.GetMethod(actionName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (method == null)
            {
                Console.WriteLine($"Actie '{actionName}' niet gevonden op {controllerType.Name}.");
                return;
            }
            
            if (!method.GetCustomAttributes(typeof(ActionAttribute), true).Any())
            {
                Console.WriteLine($"Methode '{actionName}' is geen publieke Action.");
                return;
            }
            
            _logger.Log($"[Router] Invoking {controllerType.Name}.{method.Name}");
            method.Invoke(controllerInstance, null); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij uitvoeren actie: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Details: {ex.InnerException.Message}");
        }
    }
}