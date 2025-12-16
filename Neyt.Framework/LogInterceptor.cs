using Castle.DynamicProxy;
using Neyt.Framework.Logging;

namespace Neyt.Framework;

public class LogInterceptor : IInterceptor
{
    private readonly ILogger _logger;

    public LogInterceptor(ILogger logger)
    {
        _logger = logger;
    }

    public void Intercept(IInvocation invocation)
    {
        var hasLogAttribute = invocation.Method.GetCustomAttributes(typeof(LogAttribute), true).Any();

        if (hasLogAttribute)
        {
            _logger.Log($"[Interceptor] Before execution of method: {invocation.Method.Name}");
        }
        
        invocation.Proceed();

        if (hasLogAttribute)
        {
            _logger.Log($"[Interceptor] After execution of method: {invocation.Method.Name}");
        }
    }
}