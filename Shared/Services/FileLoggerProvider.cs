using Microsoft.Extensions.Logging;
using System;

namespace Remotely.Shared.Services;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _componentName;
    private readonly string _componentVersion;

    public FileLoggerProvider(string componentName, string componentVersion)
    {
        _componentName = componentName;
        _componentVersion = componentVersion;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_componentName, _componentVersion, categoryName);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
