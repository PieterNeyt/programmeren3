using System;
using Neyt.Framework;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;
using Neyt.Framework.Routing;
using Xunit;

namespace Neyt.Tests;

public class ContainerTests
{
    interface IService
    {
    }

    class ServiceImpl : IService
    {
    }
    
    class ClassA
    {
        public ClassA(ClassB b)
        {
        }
    }

    class ClassB
    {
        public ClassB(ClassA a)
        {
        }
    }
    
    class AmbiguousClass
    {
        public AmbiguousClass(string s)
        {
        }

        public AmbiguousClass(int i)
        {
        }
    }


    public class ServiceWithLog
    {
        [Log] 
        public virtual void DoWork()
        {
        }
    }

   
    [Fact]
    public void RegisterAndResolve_ShouldReturnInstance()
    {
        // Arrange
        var services = new DiServiceCollection();
        services.AddSingleton<IService, ServiceImpl>(); 
        var container = services.BuildServiceProvider();

        // Act
        var instance = container.GetService<IService>();

        // Assert
        Assert.NotNull(instance);
        Assert.IsType<ServiceImpl>(instance);
    }
    
    [Fact]
    public void CircularDependency_ShouldThrowException_DuringBuild()
    {
        // Arrange
        var services = new DiServiceCollection();
        services.AddSingleton<ClassA, ClassA>();
        services.AddSingleton<ClassB, ClassB>();

        // Act + Assert
        var ex = Assert.Throws<Exception>(() => services.BuildServiceProvider());
        Assert.Contains("cyclic", ex.Message.ToLower());
    }
    
    [Fact]
    public void AmbiguousConstructors_ShouldThrowException()
    {
        // Arrange
        var services = new DiServiceCollection();
        services.AddSingleton<AmbiguousClass, AmbiguousClass>();
        
        services.AddSingleton("test string");
        services.AddSingleton(123);

        var container = services.BuildServiceProvider();

        // Act + Assert
        Assert.Throws<Exception>(() => container.GetService<AmbiguousClass>());
    }

  
    [Fact]
    public void InterceptedMethod_ShouldLogMessage()
    {
        // Arrange
        var services = new DiServiceCollection();
        var mockLogger = new MockLogger();
        
        services.AddSingleton<ILogger>(mockLogger);
        services.AddSingleton<ServiceWithLog, ServiceWithLog>();

        var container = services.BuildServiceProvider();
        var service = container.GetService<ServiceWithLog>();

        // Act
        service.DoWork(); 

        // Assert
        Assert.NotEmpty(mockLogger.Messages);
        Assert.Contains(mockLogger.Messages, m => m.Contains("[Log] Start"));
        Assert.Contains(mockLogger.Messages, m => m.Contains("[Log] End"));
    }
    
    [Fact]
    public void Singleton_ShouldAlwaysReturnSameInstance()
    {
        // Arrange
        var services = new DiServiceCollection();
        services.AddSingleton<ServiceImpl, ServiceImpl>();
        var container = services.BuildServiceProvider();

        // Act
        var instance1 = container.GetService<ServiceImpl>();
        var instance2 = container.GetService<ServiceImpl>();

        // Assert
        Assert.Same(instance1, instance2); 
    }
    
    class LevelD { }
    class LevelC { public LevelC(LevelD d) { } }
    class LevelB { public LevelB(LevelC c) { } }
    class LevelA { public LevelA(LevelB b) { } }

    [Fact]
    public void Resolve_DeepHierarchy_ShouldSucceed()
    {
        // Arrange
        var services = new DiServiceCollection();
        services.AddSingleton<LevelA, LevelA>();
        services.AddSingleton<LevelB, LevelB>();
        services.AddSingleton<LevelC, LevelC>();
        services.AddSingleton<LevelD, LevelD>();
        var container = services.BuildServiceProvider();

        // Act
        var instance = container.GetService<LevelA>();

        // Assert
        Assert.NotNull(instance);
    }
    
    public class TestController
    {
        [Action] 
        private void SecretAction() => Console.WriteLine("Hidden");
    }

    [Fact]
    public void Router_ShouldNotInvokePrivateActions()
    {
        // Arrange
        var services = new DiServiceCollection();
        services.AddSingleton<TestController, TestController>();
        var mockLogger = new MockLogger();
        var container = services.BuildServiceProvider();
        var router = new CommandRouter(container, typeof(TestController).Assembly, mockLogger);

        // Act
        router.HandleInput("Test SecretAction");

        // Assert
        Assert.Contains(mockLogger.Messages, m => m.Contains("not public"));
    }
    
    [NeytController] class ScannedA { }
    class ScannedB { } 

    [Fact]
    public void Scanner_ShouldOnlyRegisterTypesMatchingPredicate()
    {
        // Arrange
        var services = new DiServiceCollection();
        var assembly = typeof(ScannedA).Assembly;

        // Act
        services.RegisterByScanning(assembly, t => t.GetCustomAttributes(typeof(NeytControllerAttribute), true).Any());
        var container = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(container.GetService<ScannedA>());
        Assert.Throws<Exception>(() => container.GetService<ScannedB>());
    }
    
    [Fact]
    public void AddSingleton_IncompatibleTypes_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new DiServiceCollection();

        // Act + Assert
        Assert.Throws<ArgumentException>(() => services.AddSingleton(typeof(ILogger), typeof(ServiceImpl)));
    }
}