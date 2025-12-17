namespace Neyt.Framework.DependencyInjection;

public class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public object ImplementationInstance { get; set; }
    public ServiceLifetime Lifetime { get; }

    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
    }
    
    public ServiceDescriptor(Type serviceType, object implementationInstance, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationInstance = implementationInstance;
        Lifetime = lifetime;
    }
}
