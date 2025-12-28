using Neyt.Framework.Attributes;

namespace Neyt.Demo.Services.Lifecycle.Good;

[NeytService]
public class GoodServiceA
{
    public GoodServiceA(GoodServiceB b) { }
        
    public void DoWork() => Console.WriteLine("GoodServiceA is working correctly");
}