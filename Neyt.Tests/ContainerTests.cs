using System;
using Neyt.Framework;
using Neyt.Framework.Attributes;
using Neyt.Framework.DependencyInjection;
using Neyt.Framework.Logging;
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

        // Act & Assert
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

        // Act & Assert
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
}