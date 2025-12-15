using Neyt.Demo;
using Neyt.Framework;

Console.WriteLine("DI Container Demo Stap 1: Singleton Service");

//opzet
var services = new DiServiceCollection();

services.AddSingleton<IHelloService, HelloService>();
var container = services.BuildServiceProvider();


//Testen

Console.WriteLine("Service opvragen.");
var myService = container.GetService<IHelloService>();

myService.SayHello();

// kijk na of het een echt een Singleton is
var myService2 = container.GetService<IHelloService>();

if (ReferenceEquals(myService, myService2))
{
    Console.WriteLine("Succes: Beide services zijn dezelfde instantie ");
}
else
{
    Console.WriteLine("Fout: Er zijn verschillende instanties gemaakt.");
}

