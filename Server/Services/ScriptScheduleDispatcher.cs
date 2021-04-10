using Microsoft.Extensions.Logging;
using Remotely.Server.Hubs;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IScriptScheduleDispatcher
    {
        Task DispatchPendingScriptRuns();
    }

    public class ScriptScheduleDispatcher : IScriptScheduleDispatcher
    {
        private readonly IDataService _dataService;
        private readonly ICircuitConnection _circuitConnection;
        private readonly ILogger<ScriptScheduleDispatcher> _logger;

        public ScriptScheduleDispatcher(IDataService dataService,
            ICircuitConnection circuitConnection,
            ILogger<ScriptScheduleDispatcher> logger)
        {
            _dataService = dataService;
            _circuitConnection = circuitConnection;
            _logger = logger;
        }

        public async Task DispatchPendingScriptRuns()
        {
            try
            {
                _logger.LogInformation("Script Schedule Dispatcher started.");

                var schedules = await _dataService.GetScriptSchedulesDue();

                if (schedules?.Any() != true)
                {
                    _logger.LogInformation("No schedules are due.");
                    return;
                }

                foreach (var schedule in schedules)
                {
                    try
                    {
                        _logger.LogInformation("Considering {scheduleName}.  Interval: {interval}. Next Run: {nextRun}.",
                            schedule.Name,
                            schedule.Interval,
                            schedule.NextRun);

                        if (!AdvanceSchedule(schedule))
                        {
                            _logger.LogInformation("Schedule is not due.");
                            continue;
                        }

                        _logger.LogInformation($"Creating script run for schedule {schedule.Name}.");

                        var scriptRun = new ScriptRun()
                        {
                            OrganizationID = schedule.OrganizationID,
                            RunAt = Time.Now,
                            SavedScriptId = schedule.SavedScriptId,
                            RunOnNextConnect = schedule.RunOnNextConnect,
                            Initiator = $"Schedule: {schedule.Name}",
                            ScheduleId = schedule.Id,
                            InputType = ScriptInputType.ScheduledScript

                        };

                        var deviceIdsFromDeviceGroups = schedule.DeviceGroups?.SelectMany(dg => 
                            dg.Devices.Select(d => d.ID));

                        var deviceIds = schedule.Devices.Select(x => x.ID)
                            .Concat(deviceIdsFromDeviceGroups ?? Array.Empty<string>())
                            .Distinct()
                            .ToArray();

                        var onlineDevices = AgentHub.ServiceConnections
                            .Where(x => deviceIds.Contains(x.Value.ID))
                            .Select(x => x.Value.ID);

                        if (schedule.RunOnNextConnect)
                        {
                            scriptRun.Devices = _dataService.GetDevices(deviceIds);
                        }
                        else
                        {
                            scriptRun.Devices = _dataService.GetDevices(onlineDevices);
                        }

                        await _dataService.AddScriptRun(scriptRun);

                        await _circuitConnection.RunScript(onlineDevices, schedule.SavedScriptId, scriptRun.Id, ScriptInputType.ScheduledScript, true);

                        _logger.LogInformation($"Created script run for schedule {schedule.Name}.");

                        schedule.LastRun = Time.Now;
                        await _dataService.AddOrUpdateScriptSchedule(schedule);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while generating script run.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while dispatching script runs.");
            }
        }

        private bool AdvanceSchedule(ScriptSchedule schedule)
        {
            if (schedule is null)
            {
                return false;
            }

            if (schedule.NextRun > Time.Now)
            {
                return false;
            }

            switch (schedule.Interval)
            {
                case RepeatInterval.Hourly:
                    CatchUpNextRun(schedule, TimeSpan.FromHours(1));
                    break;
                case RepeatInterval.Daily:
                    CatchUpNextRun(schedule, TimeSpan.FromDays(1));
                    break;
                case RepeatInterval.Weekly:
                    CatchUpNextRun(schedule, TimeSpan.FromDays(7));
                    break;
                case RepeatInterval.Monthly:
                    for (var i = 0; schedule.NextRun < Time.Now; i++)
                    {
                        schedule.NextRun = schedule.StartAt.AddMonths(i);
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }


        private void CatchUpNextRun(ScriptSchedule schedule, TimeSpan interval)
        {
            while (schedule.NextRun < Time.Now)
            {
                schedule.NextRun += interval;
            }
        }
    }
}
