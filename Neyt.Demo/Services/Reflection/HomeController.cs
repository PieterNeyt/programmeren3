using Neyt.Framework;

namespace Neyt.Demo.Services.Reflection;

[Component] 
public class HomeController
{
    public void Index()
    {
        Console.WriteLine("HomeController Index Action Executed!");
    }
}