using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remotely.Server.Hubs;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class ScriptScheduler : IHostedService, IDisposable
    {
        private static readonly SemaphoreSlim _dispatchLock = new(1, 1);

        private readonly TimeSpan _timerInterval = EnvironmentHelper.IsDebug ?
            TimeSpan.FromSeconds(30) :
            TimeSpan.FromMinutes(10);

        private IServiceProvider _serviceProvider;
        private System.Timers.Timer _schedulerTimer;


        public ScriptScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public void Dispose()
        {
            _schedulerTimer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _schedulerTimer?.Dispose();
            _schedulerTimer = new System.Timers.Timer(_timerInterval.TotalMilliseconds);
            _schedulerTimer.Elapsed += SchedulerTimer_Elapsed;
            _schedulerTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _schedulerTimer?.Dispose();
            return Task.CompletedTask;
        }

        private void SchedulerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _ = DispatchScriptRuns();
        }

        public async Task DispatchScriptRuns()
        {
            using var scope = _serviceProvider.CreateScope();
            var scriptScheduleDispatcher = scope.ServiceProvider.GetRequiredService<IScriptScheduleDispatcher>();
            var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ScriptScheduler>>();
            var circuitConnection = scope.ServiceProvider.GetRequiredService<ICircuitConnection>();

            try
            {
                if (!await _dispatchLock.WaitAsync(0))
                {
                    logger.LogWarning("Script schedule dispatcher is already running.  Returning.");
                    return;
                }

                await scriptScheduleDispatcher.DispatchPendingScriptRuns();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while dispatching script runs.");
            }
            finally
            {
                _dispatchLock.Release();
            }
        }
    }
}
