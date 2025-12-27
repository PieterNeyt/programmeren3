using System.Reflection;
using Neyt.Framework.Logging;
using QuikGraph;
using QuikGraph.Algorithms;

namespace Neyt.Framework.DependencyInjection;

public class DiServiceCollection
{
    private readonly List<ServiceDescriptor> _descriptors = new ();
    private readonly ILogger _logger = new ConsoleLogger();

    public void AddSingleton<TService, TImplementation>()
        where TImplementation : TService
    {
        _descriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation)));
    }

    public void AddSingleton<TService>(TService implementationInstance)
    {
        if (implementationInstance is null)
            throw new ArgumentNullException(nameof(implementationInstance));

        _descriptors.Add(new ServiceDescriptor(typeof(TService), implementationInstance));
    }


    public void AddSingleton(Type serviceType, Type implementationType)
    {
        if (!serviceType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException($"{implementationType.Name} does not inherit from {serviceType.Name}");
        }

        _descriptors.Add(new ServiceDescriptor(serviceType, implementationType));
    }

    public void RegisterByScanning(Assembly assembly, Func<Type, bool> predicate)
    {
        var foundTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(predicate);

        foreach (var type in foundTypes)
        {
            AddSingleton(type, type);
            _logger.Log($"[Scanner] Registered: {type.Name}", LogLevel.Debug);
        }
    }

    public DiContainer BuildServiceProvider()
    {
        ILogger loggerToUse = _logger;

        var userLoggerDescriptor = _descriptors.FirstOrDefault(d => d.ServiceType == typeof(ILogger));

        if (userLoggerDescriptor != null)
        {
            if (userLoggerDescriptor.ImplementationInstance is ILogger registeredInstance)
            {
                loggerToUse = registeredInstance;
            }
        }
        else
        {
            AddSingleton(_logger);
        }

        ValidateDependencyGraph();

        return new DiContainer(_descriptors, loggerToUse);
    }

    private void ValidateDependencyGraph()
    {
        var graph = new BidirectionalGraph<Type, Edge<Type>>();

        foreach (var descriptor in _descriptors)
        {
            Type? typeToAdd = descriptor.ImplementationType;

            if (typeToAdd is null && descriptor.ImplementationInstance is not null)
            {
                typeToAdd = descriptor.ImplementationInstance.GetType();
            }

            if (typeToAdd is not null)
            {
                graph.AddVertex(typeToAdd);
            }
        }



        foreach (var descriptor in _descriptors)
        {
            if (descriptor.ImplementationInstance != null)
                continue;

            var serviceType = descriptor.ImplementationType;
            if (serviceType == null) continue;

            var constructors = serviceType.GetConstructors();
            if (constructors.Length == 0) continue;

            var bestConstructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();

            foreach (var param in bestConstructor.GetParameters())
            {
                var dependencyDescriptor = _descriptors.FirstOrDefault(d => d.ServiceType == param.ParameterType);
                if (dependencyDescriptor != null)
                {
                    var dependencyType = dependencyDescriptor.ImplementationType ??
                                         dependencyDescriptor.ImplementationInstance?.GetType();

                    if (dependencyType != null)
                    {
                        graph.AddEdge(new Edge<Type>(serviceType, dependencyType));
                    }
                }
            }
        }

        if (graph.IsDirectedAcyclicGraph())
        {
            return;
        }

        throw new Exception($"Cyclic dependency detected: {FindCyclePath(graph)}");
    }

    private string FindCyclePath(BidirectionalGraph<Type, Edge<Type>> graph)
    {
        var visited = new HashSet<Type>();
        var recursionStack = new List<Type>();

        foreach (var vertex in graph.Vertices)
        {
            if (FindCycleRecursive(vertex, graph, visited, recursionStack, out var collisionNode))
            {
                var index = recursionStack.IndexOf(collisionNode!);
                var cyclePart = recursionStack.Skip(index).ToList();
                cyclePart.Add(collisionNode!);
                return string.Join(" -> ", cyclePart.Select(t => t.Name));
            }
        }

        return "Unknown cycle";
    }


    private bool FindCycleRecursive(Type current, BidirectionalGraph<Type, Edge<Type>> graph, HashSet<Type> visited, List<Type> stack, out Type? collisionNode)
    {
        collisionNode = null;

        if (stack.Contains(current))
        {
            collisionNode = current;
            return true;
        }

        if (!visited.Add(current)) return false; 

        stack.Add(current);

        if (graph.TryGetOutEdges(current, out var edges))
        {
            foreach (var edge in edges)
            {
                if (FindCycleRecursive(edge.Target, graph, visited, stack, out collisionNode)) return true;
            }
        }

        stack.Remove(current);
        return false;
    }

}