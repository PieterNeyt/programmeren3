using Neyt.Framework;

namespace Neyt.Demo.Services.Reflection;

public class InterceptedService
{

    [Log]
    public virtual void DoSomethingImportant()
    {
        Console.WriteLine("   -> Doing important work inside the service...");
    }
    
    public void DoNormalWork()
    {
        Console.WriteLine("   -> Doing normal work (no interception).");
    }
}