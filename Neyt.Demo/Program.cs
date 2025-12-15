using Neyt.Demo;
using Neyt.Framework;


//STAP 1: Singleton Basis

Console.WriteLine("=== DI Container Demo Stap 1: Singleton Service ===");


var services = new DiServiceCollection();
services.AddSingleton<IHelloService, HelloService>();
var container = services.BuildServiceProvider();

// Testen
Console.WriteLine("Service opvragen...");
var myService = container.GetService<IHelloService>();
myService.SayHello();

// Check Singleton
var myService2 = container.GetService<IHelloService>();
if (ReferenceEquals(myService, myService2))
{
    Console.WriteLine("Succes: Beide services zijn dezelfde instantie.");
}
else
{
    Console.WriteLine("Fout: Er zijn verschillende instanties gemaakt.");
}

Console.WriteLine(); 


// STAP 2: Constructor Injection

Console.WriteLine("=== DI Container Demo Stap 2: Constructor Injection ===");

var services2 = new DiServiceCollection();

services2.AddSingleton<ISubService, SubService>();
services2.AddSingleton<IMainService, MainService>();

var container2 = services2.BuildServiceProvider();

// 3. MainService opvrage
Console.WriteLine("Resolving MainService...");

// De container zou nu op zoek moeten gaan naar de subservice en deze automatisch injecteten 
var main = container2.GetService<IMainService>();

main.DoMainTask();

Console.ReadLine();