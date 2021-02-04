using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Remotely.Server.Services
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IServiceProvider _serviceProvider;

        public DbLoggerProvider(IWebHostEnvironment hostEnvironment, IServiceProvider serviceProvider)
        {
            _hostEnvironment = hostEnvironment;
            _serviceProvider = serviceProvider;
        }


        public ILogger CreateLogger(string categoryName)
        {
            return new DbLogger(categoryName, _hostEnvironment, _serviceProvider);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
