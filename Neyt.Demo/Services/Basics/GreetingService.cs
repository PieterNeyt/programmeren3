namespace Neyt.Demo.Services.Basics;

public class GreetingService : IGreetingService
{
    public string GetWelcomeMessage()
    {
        return "This message is from the GreetingService singleton";
    }
}