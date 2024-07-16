using Remotely.Shared.Primitives;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Remotely.Shared.Extensions;

public static class ILoggerExtensions
{
    public static IDisposable Enter<T>(
        this ILogger<T> logger,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string memberName = "")
    {
        logger.Log(logLevel, "Enter: {name}", memberName);

        return new CallbackDisposable(() =>
        {
            logger.Log(logLevel, "Exit: {name}", memberName);
        });
    }

    public static void LogResult<T, ResultT>(
       this ILogger<T> logger,
       Result<ResultT> result,
       [CallerMemberName] string callerName = "")
    {
        using var logScope = string.IsNullOrWhiteSpace(callerName) ?
            new NoopDisposable() :
            logger.BeginScope(callerName);


        if (result.IsSuccess)
        {
            logger.LogInformation("Successful result.");
        }
        else if (result.HadException)
        {
            logger.LogError(result.Exception, "Error result.");
        }
        else
        {
            logger.LogWarning("Failed result. Reason: {reason}", result.Reason);
        }
    }

    public static void LogResult<T>(
      this ILogger<T> logger,
      Result result,
      [CallerMemberName] string callerName = "")
    {
        using var logScope = string.IsNullOrWhiteSpace(callerName) ?
            new NoopDisposable() :
            logger.BeginScope(callerName);


        if (result.IsSuccess)
        {
            logger.LogInformation("Successful result.");
        }
        else if (result.HadException)
        {
            logger.LogError(result.Exception, "Error result.");
        }
        else
        {
            logger.LogWarning("Failed result. Reason: {reason}", result.Reason);
        }
    }
}
