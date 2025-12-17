using Neyt.Demo.Services.Lifecycle.Bad;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;

namespace Neyt.Demo.Services.Reflection;

[Component]
public class SystemController
{
    
    [Action]
    public void Cycle()
    {
        Console.WriteLine("[System] Attempting to build a container with circular dependencies...");
        
        var badServices = new DiServiceCollection();
        
        badServices.AddSingleton<BadServiceA, BadServiceA>();
        badServices.AddSingleton<BadServiceB, BadServiceB>();
        badServices.AddSingleton<BadServiceC, BadServiceC>();

        try
        {
            var container = badServices.BuildServiceProvider();
            Console.WriteLine("[FAIL] Container built unexpectedly!");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCCESS] Caught expected error:\n   -> {ex.Message}");
            Console.ResetColor();
        }
    }
}