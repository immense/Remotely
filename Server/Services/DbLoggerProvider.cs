using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IServiceProvider serviceProvider;

        public DbLoggerProvider(IWebHostEnvironment hostEnvironment, IServiceProvider serviceProvider)
        {
            this.hostEnvironment = hostEnvironment;
            this.serviceProvider = serviceProvider;
        }


        public ILogger CreateLogger(string categoryName)
        {
            return new DbLogger(categoryName, hostEnvironment, serviceProvider);
        }

        public void Dispose()
        {
        }
    }
}
