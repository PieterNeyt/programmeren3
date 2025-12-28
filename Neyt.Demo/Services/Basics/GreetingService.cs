using Neyt.Framework.Attributes;

namespace Neyt.Demo.Services.Basics;

[NeytService]
public class GreetingService : IGreetingService
{
    public string GetWelcomeMessage()
    {
        return "This message is from the GreetingService singleton";
    }
}