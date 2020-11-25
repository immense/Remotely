using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Remotely.Server.Services
{
    public class DbLogger : ILogger
    {
        private readonly string categoryName;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IServiceProvider serviceProvider;

        protected static ConcurrentStack<string> ScopeStack { get; } = new ConcurrentStack<string>();

        public DbLogger(string categoryName, IWebHostEnvironment hostEnvironment, IServiceProvider serviceProvider)
        {
            this.categoryName = categoryName;
            this.hostEnvironment = hostEnvironment;
            this.serviceProvider = serviceProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            ScopeStack.Push(state.ToString());
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    break;
                case LogLevel.Debug:
                case LogLevel.Information:
                    if (hostEnvironment.IsDevelopment())
                    {
                        return true;
                    }
                    break;
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                    return true;
                case LogLevel.None:
                    break;
                default:
                    break;
            }
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using var scope = serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
            dataService.WriteLog(logLevel, categoryName, eventId, state.ToString(), exception, ScopeStack.ToList());
        }


        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
                while (!ScopeStack.TryPop(out _))
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
