# Dependency Injection Container

**Student:** Pieter Neyt  
**Vak:** Programmeren 3  

---
## Features
### Dependency Injection
- Ondersteuning voor **Singleton** lifetime
- **Constructor Injection**
  - Automatische selectie van de constructor met de meeste parameters
- Ondersteuning voor complexe dependency graphs  
  (bv. `A -> B -> C`)

### Cycle Detection
- Detecteert cyclische afhankelijkheden  
  (bv. `A -> B -> A`)
- Validatie gebeurt bij het **bouwen van de container**
- Implementatie via **QuikGraph** 

### Assembly Scanning
- Automatische registratie van:
  - Services
  - Controllers
- Op basis van attributen zoals `[Component]`

### Interception (AOP)
- Gebaseerd op **Castle.Core** (Dynamic Proxies)
- Ondersteunde interceptors:
  - `[Log]` — logt start en einde van methode-aanroepen
  - `[Timed]` — meet en logt de uitvoertijd (`Stopwatch`)
- Methoden die geïntercepteerd worden moeten **`virtual`** zijn

### Multi-level Logging
- Ingebouwd logging-systeem met drie niveaus: `Debug`, `Info`, `Warning`.
- **Debug-modus**: Toont de interne werking van de container (welke objecten op welk moment worden ge-resolve en wanneer er proxies worden gemaakt).

### Console Router (Mini-MVC)
- Mapt console-commando’s naar controller-acties  
  (bv. `"Home Index" -> HomeController.Index()`)

---

## Gebruik van het Framework

### 1. Container opzetten

Gebruik `DiServiceCollection` om services te registreren, manueel of via assembly scanning.

```csharp
using Neyt.Framework;
using System.Reflection;

// 1. Initialiseer de service collectie
var services = new DiServiceCollection();

// 2. Manuele registratie (interface -> implementatie)
services.AddSingleton<ILogger>(new ConsoleLogger());

// 3. Assembly scanning via attributen
var assembly = Assembly.GetExecutingAssembly();
services.RegisterByScanning(
    assembly,
    type => type.GetCustomAttributes(typeof(ComponentAttribute), true).Any()
);

// 4. Bouw de container (cycle detection gebeurt hier)
try
{
    var container = services.BuildServiceProvider();
}
catch (Exception ex)
{
    Console.WriteLine($"Cycle detected: {ex.Message}");
}
````

---

### 2. Services en Interception definiëren

Gebruik attributen om services automatisch te laten registreren en interceptors toe te passen.
Methodes die geïntercepteerd worden moeten **`virtual`** zijn.

```csharp
using Neyt.Framework;

[Component]
public class CalculationService
{
    [Log]
    [Timed]
    public virtual void CalculateHeavySum()
    {
        Thread.Sleep(500);
        Console.WriteLine("Berekening uitgevoerd.");
    }
}
```

---

### 3. Console Router gebruiken (MVC-pattern)

Controllers kunnen `[Action]`-methodes bevatten die via tekstcommando’s aangeroepen worden.

```csharp
[Component]
public class HomeController
{
    private readonly CalculationService _service;

    public HomeController(CalculationService service)
    {
        _service = service;
    }

    [Action]
    public void Index()
    {
        Console.WriteLine("Home Page");
        _service.CalculateHeavySum();
    }
}
```

---

## Build & Run

Gebruik onderstaande commando’s vanuit de **root van de solution**.

### Build

```bash
dotnet build
```

### Demo applicatie starten

Start de interactieve console router.
Typ `exit` om de applicatie te stoppen.

```bash
dotnet run --project Neyt.Demo
```

### Unit tests uitvoeren

```bash
dotnet test
```

---

## Architectuur & Libraries

### Projecten

* **Neyt.Framework**
  Core library met DI-container, scanners en interceptors
* **Neyt.Demo**
  Console applicatie die de werking demonstreert
* **Neyt.Tests**
  xUnit testproject

### Externe packages

* **QuikGraph** — Validatie van dependency graphs 
* **Castle.Windsor** — Dynamic proxies voor AOP / interception

