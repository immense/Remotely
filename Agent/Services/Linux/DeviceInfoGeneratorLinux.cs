using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Dtos;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Services.Linux;

public class DeviceInfoGeneratorLinux : DeviceInfoGeneratorBase, IDeviceInformationService
{
    private readonly IProcessInvoker _processInvoker;
    private readonly ICpuUtilizationSampler _cpuUtilSampler;

    public DeviceInfoGeneratorLinux(
        IProcessInvoker processInvoker, 
        ICpuUtilizationSampler cpuUtilSampler,
        ILogger<DeviceInfoGeneratorLinux> logger)
        : base(logger)
    {
        _processInvoker = processInvoker;
        _cpuUtilSampler = cpuUtilSampler;
    }

    public Task<DeviceClientDto> CreateDevice(string deviceId, string orgId)
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
            device.CpuUtilization = _cpuUtilSampler.CurrentUtilization;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device info.");
        }

        return Task.FromResult(device);
    }

    private string GetCurrentUser()
    {
        var users = _processInvoker.InvokeProcessOutput("users", "");
        return users?.Split()?.FirstOrDefault()?.Trim() ?? string.Empty;
    }

    public (double usedGB, double totalGB) GetMemoryInGB()
    {
        try
        {
            var results = _processInvoker.InvokeProcessOutput("cat", "/proc/meminfo");
            var resultsArr = results.Split("\n".ToCharArray());
            var freeKB = resultsArr
                        .FirstOrDefault(x => x.Trim().StartsWith("MemAvailable"))
                        ?.Trim()
                        ?.Split(" ".ToCharArray(), 2)
                        ?.Last() // 9168236 kB
                        ?.Trim()
                        ?.Split(' ')
                        ?.First() // 9168236
                        ?? "0";

            var totalKB = resultsArr
                        .FirstOrDefault(x => x.Trim().StartsWith("MemTotal"))
                        ?.Trim()
                        ?.Split(" ".ToCharArray(), 2)
                        ?.Last() // 16637468 kB
                        ?.Trim()
                        ?.Split(' ')
                        ?.First() // 16637468
                        ?? "0"; 

            var freeGB = Math.Round(double.Parse(freeKB) / 1024 / 1024, 2);
            var totalGB = Math.Round(double.Parse(totalKB) / 1024 / 1024, 2);

            return (totalGB - freeGB, totalGB);
        }
        catch
        {
            return (0, 0);
        }
    }
}
