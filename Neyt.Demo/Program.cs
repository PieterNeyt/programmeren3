using System.Reflection;
using Neyt.Demo;
using Neyt.Demo.Services.Basics;
using Neyt.Demo.Services.Hierarchy;
using Neyt.Demo.Services.Lifecycle;
using Neyt.Demo.Services.Lifecycle.Bad;
using Neyt.Demo.Services.Lifecycle.Good;
using Neyt.Demo.Services.Reflection;
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


// STAP 3: Cycle Detection

Console.WriteLine("\n=== DI Container Demo Stap 3: Cycle Detection ===");

var services3 = new DiServiceCollection();

// registreren slechte services
services3.AddSingleton<BadServiceA, BadServiceA>();
services3.AddSingleton<BadServiceB, BadServiceB>();
services3.AddSingleton<BadServiceC, BadServiceC>();

try
{
    var container3 = services3.BuildServiceProvider();
    Console.WriteLine("FOUT: Dit had moeten crashen!");
}
catch (Exception ex)
{
    Console.WriteLine($"SUCCES: De fout is gevangen!");
    Console.WriteLine($"Melding: {ex.Message}");
}

Console.WriteLine();


Console.WriteLine("--- Test 3b: Valid Deep Dependency (A -> B -> C) ---");

// registreren goede services
var services3b = new DiServiceCollection();
services3b.AddSingleton<GoodServiceA, GoodServiceA>();
services3b.AddSingleton<GoodServiceB, GoodServiceB>();
services3b.AddSingleton<GoodServiceC, GoodServiceC>();

try
{
    var container3c = services3b.BuildServiceProvider();
    Console.WriteLine("Container gebouwd. Nu GoodServiceA ophalen...");
    
    var service = container3c.GetService<GoodServiceA>();
    service.DoWork();
    
    Console.WriteLine("SUCCES: Geen false positive detected.");
}
catch (Exception ex)
{
    Console.WriteLine($"FOUT: Er werd onterecht een fout gegooid!");
    Console.WriteLine(ex.Message);
}
Console.WriteLine();
// STAP 4: Assembly Scanning
Console.WriteLine("=== DI Container Demo Stap 4: Assembly Scanning ===");

var services4 = new DiServiceCollection();

var currentAssembly = Assembly.GetExecutingAssembly();

// scannen op attribuut alles dat het Component attribuut heeft
services4.RegisterByScanning(currentAssembly, type => type.GetCustomAttributes(typeof(ComponentAttribute), true).Any());

var container4 = services4.BuildServiceProvider();

//testen of de homecontroller is geregistreerd en gevonden
try 
{
    var controller = container4.GetService<HomeController>();
    controller.Index();
    Console.WriteLine("HomeController gevonden via scanning");
}
catch(Exception ex)
{
    Console.WriteLine($"FOUT: {ex.Message}");
}
