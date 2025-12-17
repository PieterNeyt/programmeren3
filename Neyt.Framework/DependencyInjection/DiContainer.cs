using Castle.DynamicProxy;
using Neyt.Framework.Attributes;
using Neyt.Framework.Interception;
using Neyt.Framework.Logging;

namespace Neyt.Framework.DependencyInjection;

public class DiContainer
{
    private readonly List<ServiceDescriptor> _descriptors;
    private readonly Dictionary<Type, object> _singletonInstances = new Dictionary<Type, object>();
    private readonly ILogger _logger;
    private readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

    public DiContainer(List<ServiceDescriptor> descriptors, ILogger logger)
    {
        _descriptors = descriptors;
        _logger = logger;
    }

    public object GetService(Type serviceType)
    {
        var descriptor = _descriptors.FirstOrDefault(x => x.ServiceType == serviceType);

        if (descriptor == null) throw new Exception($"Service of type {serviceType.Name} is not registered.");

        // Singleton Cache check
        if (descriptor.Lifetime == ServiceLifetime.SINGLETON)
        {
            if (descriptor.ImplementationInstance != null) return descriptor.ImplementationInstance;
            if (_singletonInstances.ContainsKey(serviceType)) return _singletonInstances[serviceType];
        }

        var actualType = descriptor.ImplementationType;

        _logger.Log($"[Container] Resolving {actualType.Name}", LogLevel.DEBUG);

        var constructors = actualType.GetConstructors();
        if (constructors.Length == 0) throw new Exception($"Type {actualType.Name} has no public constructors.");

        var sortedConstructors = constructors.OrderByDescending(c => c.GetParameters().Length).ToList();
        var bestConstructor = sortedConstructors.First();

        // Ambiguity check
        if (sortedConstructors.Count > 1)
        {
            var firstCount = sortedConstructors[0].GetParameters().Length;
            var secondCount = sortedConstructors[1].GetParameters().Length;
            if (firstCount == secondCount) throw new Exception($"Ambiguous constructors for {actualType.Name}");
        }

        var parameters = bestConstructor.GetParameters();
        var arguments = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            arguments[i] = GetService(parameters[i].ParameterType);
        }

        bool needsInterception = actualType.GetMethods()
            .Any(m => m.GetCustomAttributes(typeof(LogAttribute), true).Any() ||
                      m.GetCustomAttributes(typeof(TimedAttribute), true).Any());

        object instance;

        if (needsInterception)
        {
            _logger.Log($"[Container] Interception detected for {actualType.Name}. Creating Proxy.", LogLevel.DEBUG);

            var interceptor = new AspectInterceptor(_logger);

            instance = _proxyGenerator.CreateClassProxy(actualType, arguments, interceptor);
        }
        else
        {
            instance = Activator.CreateInstance(actualType, arguments);
        }

        // Singleton opslaan
        if (descriptor.Lifetime == ServiceLifetime.SINGLETON)
        {
            _singletonInstances[serviceType] = instance;
        }

        return instance;
    }

    public T GetService<T>() => (T)GetService(typeof(T));
}