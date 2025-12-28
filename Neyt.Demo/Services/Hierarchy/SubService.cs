using Neyt.Framework.Attributes;

namespace Neyt.Demo.Services.Hierarchy;

[NeytService]
public class SubService : ISubService
{
    public void DoSubTask()
    {
        Console.WriteLine("SubService is doing work");
    }
}