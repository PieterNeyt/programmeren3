namespace Neyt.Framework;

public class DiContainer
{
    private readonly List<ServiceDescriptor> _descriptors;
    private readonly Dictionary<Type, object> _singletonInstances = new Dictionary<Type, object>();

    public DiContainer(List<ServiceDescriptor> descriptors)
    {
        _descriptors = descriptors;
    }

    public object GetService(Type serviceType)
    {
        var descriptor = _descriptors.FirstOrDefault(x => x.ServiceType == serviceType);

        if (descriptor == null)
        {
            throw new Exception($"Service of type {serviceType.Name} is not registered.");
        }
        
        if (descriptor.Lifetime == ServiceLifetime.SINGLETON)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            if (_singletonInstances.ContainsKey(serviceType))
            {
                return _singletonInstances[serviceType];
            }
        }
        
        var actualType = descriptor.ImplementationType;
        var instance = Activator.CreateInstance(actualType);
        
        if (descriptor.Lifetime == ServiceLifetime.SINGLETON)
        {
            _singletonInstances[serviceType] = instance;
        }

        return instance;
    }
    
    public T GetService<T>()
    {
        return (T)GetService(typeof(T));
    }
}