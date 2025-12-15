namespace Neyt.Demo;

public class SubService : ISubService
{
    public void DoSubTask()
    {
        Console.WriteLine("   -> SubService is doing work!");
    }
}