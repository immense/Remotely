using Microsoft.Extensions.Logging;
using System;

namespace Remotely.Shared.Services
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _applicationName;

        public FileLoggerProvider(string applicationName)
        {
            _applicationName = applicationName;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_applicationName, categoryName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
