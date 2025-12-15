namespace Neyt.Framework;

public class DiServiceCollection
{
    private List<ServiceDescriptor> _descriptors = new List<ServiceDescriptor>();
    
    public void AddSingleton<TService, TImplementation>() 
        where TImplementation : TService
    {
        _descriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.SINGLETON));
    }
    
    public void AddSingleton<TService>(TService implementationInstance)
    {
        _descriptors.Add(new ServiceDescriptor(typeof(TService), implementationInstance, ServiceLifetime.SINGLETON));
    }
    
    public DiContainer BuildServiceProvider()
    {
        return new DiContainer(_descriptors);
    }
}