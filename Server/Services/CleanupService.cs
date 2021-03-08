using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class CleanupService : IHostedService, IDisposable
    {
        public CleanupService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        private IServiceProvider Services { get; }
        private System.Timers.Timer CleanupTimer { get; set; }

        public void Dispose()
        {
            CleanupTimer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CleanupTimer?.Dispose();
            CleanupTimer = new System.Timers.Timer(TimeSpan.FromDays(1).TotalMilliseconds);
            CleanupTimer.Elapsed += CleanupTimer_Elapsed;
            CleanupTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            CleanupTimer?.Dispose();
            return Task.CompletedTask;
        }

        private void CleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            using var scope = Services.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();

            dataService.CleanupOldRecords();
        }
    }
}
