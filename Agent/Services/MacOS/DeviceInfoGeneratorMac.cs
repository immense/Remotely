using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Dtos;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Agent.Services.MacOS;

public class DeviceInfoGeneratorMac : DeviceInfoGeneratorBase, IDeviceInformationService
{
    private readonly IProcessInvoker _processInvoker;

    public DeviceInfoGeneratorMac(IProcessInvoker processInvoker, ILogger<DeviceInfoGeneratorMac> logger)
        : base(logger)
    {
        _processInvoker = processInvoker;
    }
    public async Task<DeviceClientDto> CreateDevice(string deviceId, string orgId)
    {
        var device = GetDeviceBase(deviceId, orgId);

        try
        {

            var (usedStorage, totalStorage) = GetSystemDriveInfo();
            var (usedMemory, totalMemory) = GetMemoryInGB();

            device.CurrentUser = GetCurrentUser();
            device.Drives = GetAllDrives();
            device.UsedStorage = usedStorage;
            device.TotalStorage = totalStorage;
            device.UsedMemory = usedMemory;
            device.TotalMemory = totalMemory;
            device.CpuUtilization = await GetCpuUtilization();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device info.");
        }

        return device;
    }

    public (double usedGB, double totalGB) GetMemoryInGB()
    {
        try
        {
            double totalGB = default;

            var totalMemoryString = _processInvoker.InvokeProcessOutput("zsh", "-c \"sysctl -n hw.memsize\"");
            if (double.TryParse(totalMemoryString, out var totalMemory))
            {
                totalGB = (double)Math.Round(totalMemory / 1024 / 1024 / 1024, 2);
            }


            double usedGB = default;

            var memPercentStrings = _processInvoker.InvokeProcessOutput("zsh", $"-c \"ps -A -o %mem\"");

            double usedMemPercent = 0;
            memPercentStrings
                .Split(Environment.NewLine)
                .ToList()
                .ForEach(x =>
                {
                    if (double.TryParse(x, out var result))
                    {
                        usedMemPercent += result;
                    }
                });

            usedMemPercent = usedMemPercent / 4 / 100;
            usedGB = usedMemPercent * totalGB;

            return (usedGB, totalGB);
        }
        catch
        {
            return (0, 0);
        }
    }

    private Task<double> GetCpuUtilization()
    {
        try
        {
            var cpuPercentStrings = _processInvoker.InvokeProcessOutput("zsh", "-c \"ps -A -o %cpu\"");

            double cpuPercent = 0;
            cpuPercentStrings
                .Split(Environment.NewLine)
                .ToList()
                .ForEach(x =>
                {
                    if (double.TryParse(x, out var result))
                    {
                        cpuPercent += result;
                    }
                });

            return Task.FromResult(cpuPercent / Environment.ProcessorCount / 100);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting CPU utilization.");
        }

        return Task.FromResult((double)0);
    }
    private string GetCurrentUser()
    {
        var users = _processInvoker.InvokeProcessOutput("users", "");
        return users?.Split()?.FirstOrDefault()?.Trim() ?? string.Empty;
    }
}
