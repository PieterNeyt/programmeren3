using Neyt.Framework;

namespace Neyt.Demo.Services.Reflection;

[Component]
public class InterceptedService
{
    [Log]
    public virtual void DoSomethingImportant()
    {
        Console.WriteLine("   -> Doing important work inside the service");
        Thread.Sleep(100); 
    }
    
    [Timed]
    public virtual void CalculateHugeSum()
    {
        Console.WriteLine("   -> Calculating huge sum");
        Thread.Sleep(500); 
    }
    
    [Log]
    [Timed]
    public virtual void CombinedAction()
    {
        Console.WriteLine("   -> Doing log AND timed work");
        Thread.Sleep(200);
    }

    public void DoNormalWork()
    {
        Console.WriteLine("   -> Doing normal work");
    }
}