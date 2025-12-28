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
- Automatische registratie van klassen met het `[NeytService]` attribuut.
- Automatische registratie van controllers met het `[NeytController]` attribuut.
- Ondersteuning voor custom scanning via predicates.

### Interception (AOP)
- Dynamic Proxy-techniek via **Castle.Core**.
- `[Log]`: Logt entry en exit van een methode naar de standaard output.
- `[Timed]`: Meet de doorlooptijd van een methode.
- *Belangrijk:* Methoden moeten **`virtual`** zijn om interceptie mogelijk te maken.

### Multi-level Logging
- Ingebouwd framework met niveaus: `Debug`, `Info`, `Warning`.
- In **Debug-modus** is exact te volgen welke objecten wanneer ge-instancieerd worden (beperkt reflection na opstart).

### Command Router
- Een keyboard-input router die tekst commando's mapt naar `[Action]` methodes op controllers.
- Formaat: `[Controller Naam] [Action Naam]` (bijv. `Home Index`).

---

## Gebruik van het Framework

### 1. Container opzetten

Gebruik `DiServiceCollection` om services te registreren en de container te bouwen.

```csharp
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;
using System.Reflection;

var services = new DiServiceCollection();
var assembly = Assembly.GetExecutingAssembly();

// 1.  registratie
services.AddSingleton<ILogger>(new ConsoleLogger { MinLevel = LogLevel.Debug });

// 2. Registratie via attributen ([NeytService])
services.RegisterServicesByAttribute(assembly);

// 3. Scanning voor controllers
services.RegisterByScanning(assembly, type => 
    type.GetCustomAttributes(typeof(NeytControllerAttribute), true).Any());

// 4. Bouwen (inclusief Cycle Detection)
var container = services.BuildServiceProvider();
````

---

### 2. Services en Interception definiëren

Gebruik attributen om services automatisch te laten registreren en interceptors toe te passen.
Methodes die geïntercepteerd worden moeten **`virtual`** zijn.

```csharp
using Neyt.Framework;

[NeytService]
public class CalculationService
{
    [Log]
    [Timed]
    public virtual void Calculate()
    {
        // Deze methode wordt gelogd en getimed
    }
}
```

---

### 3. Console Router gebruiken (MVC-pattern)

Controllers kunnen `[Action]`-methodes bevatten die via tekstcommando’s aangeroepen worden.

```csharp
[NeytController]
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
        _service.Calculate();
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

