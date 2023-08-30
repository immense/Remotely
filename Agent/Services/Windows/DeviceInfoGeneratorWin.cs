using Immense.RemoteControl.Desktop.Shared.Native.Windows;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Shared.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Agent.Services.Windows;

public class DeviceInfoGeneratorWin : DeviceInfoGeneratorBase, IDeviceInformationService
{
    private readonly ICpuUtilizationSampler _cpuUtilSampler;

    public DeviceInfoGeneratorWin(
        ICpuUtilizationSampler cpuUtilSampler, 
        ILogger<DeviceInfoGeneratorWin> logger)
        : base(logger)
    { 
        _cpuUtilSampler = cpuUtilSampler;
    }

    public Task<DeviceClientDto> CreateDevice(string deviceId, string orgId)
    {
        var device = GetDeviceBase(deviceId, orgId);

        try
        {
            var (usedStorage, totalStorage) = GetSystemDriveInfo();
            var (usedMemory, totalMemory) = GetMemoryInGB();

            device.CurrentUser = Win32Interop.GetActiveSessions().LastOrDefault()?.Username ?? string.Empty;
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

    public (double usedGB, double totalGB) GetMemoryInGB()
    {
        try
        {
            var memoryStatus = new Kernel32.MEMORYSTATUSEX();
            double totalGB = 0;
            double freeGB = 0;

            if (Kernel32.GlobalMemoryStatusEx(memoryStatus))
            {
                freeGB = Math.Round((double)memoryStatus.ullAvailPhys / 1024 / 1024 / 1024, 2);
                totalGB = Math.Round((double)memoryStatus.ullTotalPhys / 1024 / 1024 / 1024, 2);
            }

            return (totalGB - freeGB, totalGB);
        }
        catch
        {
            return (0, 0);
        }
    }
}
