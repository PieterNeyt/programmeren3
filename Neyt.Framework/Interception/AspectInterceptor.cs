using System.Diagnostics;
using Castle.DynamicProxy;
using Neyt.Framework.Attributes;
using Neyt.Framework.Logging;

namespace Neyt.Framework.Interception;

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
            _logger.Log($"[Log] Start method: {invocation.Method.Name}",LogLevel.INFO);
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
                _logger.Log($"[Timed] Method {invocation.Method.Name} ran for {stopwatch.ElapsedMilliseconds} ms",LogLevel.INFO);
            }

            if (hasLog)
            {
                _logger.Log($"[Log] End method: {invocation.Method.Name}",LogLevel.INFO);
            }
        }
    }
}