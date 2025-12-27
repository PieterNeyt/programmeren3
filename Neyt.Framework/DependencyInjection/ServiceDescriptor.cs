namespace Neyt.Framework.DependencyInjection;

public class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type? ImplementationType { get; }
    public object? ImplementationInstance { get; set; }
    public ServiceDescriptor(Type serviceType, Type implementationType)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
      
    }
    
    public ServiceDescriptor(Type serviceType, object implementationInstance)
    {
        ServiceType = serviceType;
        ImplementationInstance = implementationInstance;
       
    }
}
