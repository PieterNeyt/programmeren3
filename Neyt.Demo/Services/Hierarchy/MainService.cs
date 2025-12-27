using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Hierarchy;


public class MainService : IMainService
{
    private readonly ISubService _subService;
    private readonly ILogger _logger;
    
    
    public MainService(ISubService subService, ILogger logger)
    {
        _logger=logger;
        _subService = subService;
    }

    public void DoMainTask()
    {
        _logger.Log("MainService starting", LogLevel.INFO);
        _subService.DoSubTask(); 
        _logger.Log("MainService finished", LogLevel.INFO);
    }
}