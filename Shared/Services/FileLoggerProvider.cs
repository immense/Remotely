using Microsoft.Extensions.Logging;
using System;

namespace Remotely.Shared.Services
{
    public class FileLoggerProvider : ILoggerProvider
    {

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
