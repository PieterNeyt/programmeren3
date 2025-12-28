using Castle.DynamicProxy;
using Neyt.Framework.Attributes;
using Neyt.Framework.Interception;
using Neyt.Framework.Logging;

namespace Neyt.Framework.DependencyInjection;

public class DiContainer
{
    private readonly List<ServiceDescriptor> _descriptors;
    // Cache
    private readonly Dictionary<Type, object> _singletonInstances = new();
    private readonly ILogger _logger;
    private readonly ProxyGenerator _proxyGenerator = new();

    public DiContainer(List<ServiceDescriptor> descriptors, ILogger logger)
    {
        _descriptors = descriptors;
        _logger = logger;
    }

    public object GetService(Type serviceType)
    {
        //DI geeft zichzelf terug
        if (serviceType == typeof(DiContainer))
        {
            return this;
        }

        var descriptor = _descriptors.FirstOrDefault(x => x.ServiceType == serviceType);
        
        if (descriptor == null)
            throw new Exception($"Service of type {serviceType.Name} is not registered.");

        if (descriptor.ImplementationInstance != null)
            return descriptor.ImplementationInstance;

        var actualType = descriptor.ImplementationType ?? throw new Exception($"No implementation type for {serviceType.Name}");

        if (_singletonInstances.TryGetValue(actualType, out var existingInstance))
        {
            return existingInstance;
        }

        _logger.Log($"[Container] Resolving {actualType.Name}", LogLevel.Debug);

        // Constructor selectie
        var constructors = actualType.GetConstructors();
        if (constructors.Length == 0) throw new Exception($"Type {actualType.Name} has no public constructors.");

        var bestConstructor = constructors
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        // Ambiguïteit check
        var allConstructors = constructors.OrderByDescending(c => c.GetParameters().Length).ToList();
        if (allConstructors.Count > 1 &&
            allConstructors[0].GetParameters().Length == allConstructors[1].GetParameters().Length)
        {
            throw new Exception($"Ambiguous constructors for {actualType.Name}");
        }

        //  Recursief parameters resolven
        var parameters = bestConstructor.GetParameters();
        var arguments = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            arguments[i] = GetService(parameters[i].ParameterType);
        }

        //  Interceptie check
        bool needsInterception = actualType.GetMethods()
            .Any(m => m.GetCustomAttributes(typeof(LogAttribute), true).Any() ||
                      m.GetCustomAttributes(typeof(TimedAttribute), true).Any());

        object instance;
        if (needsInterception)
        {
            _logger.Log($"[Container] Interception detected for {actualType.Name}. Creating Proxy.", LogLevel.Debug);
            var interceptor = new AspectInterceptor(_logger);
            // dynamische proxy
            instance = _proxyGenerator.CreateClassProxy(actualType, arguments, interceptor)
                       ?? throw new InvalidOperationException($"Failed to create proxy for {actualType.Name}");
        }
        else
        {
            instance = Activator.CreateInstance(actualType, arguments)
                       ?? throw new InvalidOperationException($"Failed to create instance of {actualType.Name}");
        }

        _singletonInstances[actualType] = instance;
        return instance;
    }

    public T GetService<T>() => (T)GetService(typeof(T));
}