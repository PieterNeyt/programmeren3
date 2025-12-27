using Neyt.Demo.Services.Hierarchy;
using Neyt.Framework.Attributes;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Reflection;

[Component]
public class HierarchyController
{
    private readonly IMainService _mainService;
    private readonly ILogger _logger;
    public HierarchyController(IMainService mainService, ILogger logger)
    {
        _logger = logger;
        _mainService = mainService;
    }

    [Action]
    public void Run()
    {
        _logger.Log("[Hierarchy Demo] Starting the task", LogLevel.Info);
        _mainService.DoMainTask();
    }
}