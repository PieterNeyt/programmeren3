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
            
            var constructors = actualType.GetConstructors();

            if (constructors.Length == 0)
            {
                throw new Exception($"Type {actualType.Name} has no public constructors.");
            }
            
            var sortedConstructors = constructors
                .OrderByDescending(c => c.GetParameters().Length)
                .ToList();

            var bestConstructor = sortedConstructors.First();
            
            if (sortedConstructors.Count > 1)
            {
                var firstCount = sortedConstructors[0].GetParameters().Length;
                var secondCount = sortedConstructors[1].GetParameters().Length;

                if (firstCount == secondCount)
                {
                    throw new Exception($"Ambiguous constructors found for {actualType.Name}. Multiple constructors have {firstCount} parameters.");
                }
            }
            
            var parameters = bestConstructor.GetParameters();
            var arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                var parameterType = parameterInfo.ParameterType;
                
                arguments[i] = GetService(parameterType);
            }
            
            var instance = Activator.CreateInstance(actualType, arguments);
            
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