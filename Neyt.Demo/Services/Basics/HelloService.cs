namespace Neyt.Demo.Services.Basics;

public class HelloService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello from the HelloService");
    }
}