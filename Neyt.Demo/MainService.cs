namespace Neyt.Demo;

public class MainService : IMainService
{
    private readonly ISubService _subService;
    
    public MainService(ISubService subService)
    {
        _subService = subService;
    }

    public void DoMainTask()
    {
        Console.WriteLine(">> MainService starting...");
        _subService.DoSubTask(); 
        Console.WriteLine(">> MainService finished.");
    }
}