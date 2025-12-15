namespace Neyt.Demo;

public class HelloService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine(">> Hello from the HelloService!");
    }
}