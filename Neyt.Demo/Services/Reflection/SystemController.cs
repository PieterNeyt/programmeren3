using Neyt.Demo.Services.Basics;
using Neyt.Demo.Services.Lifecycle.Bad;
using Neyt.Demo.Services.Lifecycle.Good;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Reflection;

[NeytController]
public class SystemController
{
    private readonly ILogger _logger;
    private readonly DiContainer _container;
    private readonly GoodServiceA _goodServiceA;
    public SystemController(ILogger logger, DiContainer container,GoodServiceA goodServiceA)
    {
        _logger = logger;
        _container = container;
        _goodServiceA = goodServiceA;
    }
    [Action]
    public void Cycle()
    {
        _logger.Log("[System] Attempting to build a container with circular dependencies", LogLevel.Info);
        
        var badServices = new DiServiceCollection();
        
        badServices.AddSingleton<BadServiceA, BadServiceA>();
        badServices.AddSingleton<BadServiceB, BadServiceB>();
        badServices.AddSingleton<BadServiceC, BadServiceC>();

        try
        {
            badServices.BuildServiceProvider();
            _logger.Log("[FAIL] Container built unexpectedly!", LogLevel.Warning);
        }
        catch (Exception ex)
        {
            _logger.Log($"[SUCCESS] Caught expected error: {ex.Message}", LogLevel.Info);
        }
    }
    [Action]
    public void Greedy()
    {
        _logger.Log("[System] Testing Greedy Constructor selection", LogLevel.Info);
        var service = _container.GetService<MultiCtorService>();
        _logger.Log(service.Status, LogLevel.Info);
    }
    [Action]
    public void Deep()
    {
        _logger.Log("[System] Using the GoodServiceA", LogLevel.Info);
        _goodServiceA.DoWork(); 
    }

}