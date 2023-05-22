using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class DataCleanupService : IHostedService, IDisposable
    {
        private readonly ILogger<DataCleanupService> _logger;

        private readonly IServiceProvider _services;

        private System.Timers.Timer _cleanupTimer = new(TimeSpan.FromDays(1));


        public DataCleanupService(
            IServiceProvider serviceProvider,
            ILogger<DataCleanupService> logger)
        {
            _services = serviceProvider;
            _logger = logger;

            _cleanupTimer.Elapsed += CleanupTimer_Elapsed;
        }
        public void Dispose()
        {
            _cleanupTimer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer.Stop();
            _cleanupTimer.Dispose();
            return Task.CompletedTask;
        }

        private void CleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                using var scope = _services.CreateScope();
                var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
                dataService.CleanupOldRecords();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during data cleanup.");
            }
        }
    }
}
