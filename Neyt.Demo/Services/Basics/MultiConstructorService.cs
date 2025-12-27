using Neyt.Framework.Attributes;
using Neyt.Framework.Logging;

namespace Neyt.Demo.Services.Basics;

[Component]
public class MultiCtorService
{
    public string Status { get; }
    
    public MultiCtorService()
    {
        Status = "FOUT: Default constructor gekozen";
    }

    
    public MultiCtorService(ILogger logger, IGreetingService greeting)
    {
        Status = "SUCCES: Greedy constructor gekozen ";
    }
}