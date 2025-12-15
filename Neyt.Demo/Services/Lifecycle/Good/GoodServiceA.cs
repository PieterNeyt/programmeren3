namespace Neyt.Demo.Services.Lifecycle.Good;

public class GoodServiceA
{
    public GoodServiceA(GoodServiceB b) { }
        
    public void DoWork() => Console.WriteLine("   -> GoodServiceA is working correctly!");
}