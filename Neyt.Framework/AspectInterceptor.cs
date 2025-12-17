using System.Diagnostics;
using Castle.DynamicProxy;
using Neyt.Framework.Logging;

namespace Neyt.Framework;

public class AspectInterceptor : IInterceptor
{
    private readonly ILogger _logger;

    public AspectInterceptor(ILogger logger)
    {
        _logger = logger;
    }

    public void Intercept(IInvocation invocation)
    {
        bool hasLog = invocation.Method.GetCustomAttributes(typeof(LogAttribute), true).Any();
        bool hasTimed = invocation.Method.GetCustomAttributes(typeof(TimedAttribute), true).Any();
        
        if (hasLog)
        {
            _logger.Log($"[Log] Start method: {invocation.Method.Name}");
        }

        var stopwatch = new Stopwatch();
        if (hasTimed)
        {
            stopwatch.Start(); 
        }
        
        try
        {
            invocation.Proceed();
        }
        finally
        {
            
            if (hasTimed)
            {
                stopwatch.Stop();
                _logger.Log($"[Timed] Method {invocation.Method.Name} duurde {stopwatch.ElapsedMilliseconds} ms");
            }

            if (hasLog)
            {
                _logger.Log($"[Log] End method: {invocation.Method.Name}");
            }
        }
    }
}