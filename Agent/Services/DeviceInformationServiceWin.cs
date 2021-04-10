using Remotely.Agent.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class DeviceInformationServiceWin : DeviceInformationServiceBase, IDeviceInformationService
    {
        public async Task<Device> CreateDevice(string deviceId, string orgId)
        {
            var device = GetDeviceBase(deviceId, orgId);

            try
            {

                var (usedStorage, totalStorage) = GetSystemDriveInfo();
                var (usedMemory, totalMemory) = GetMemoryInGB();

                device.CurrentUser = Win32Interop.GetActiveSessions().LastOrDefault()?.Username;
                device.Drives = GetAllDrives();
                device.UsedStorage = usedStorage;
                device.TotalStorage = totalStorage;
                device.UsedMemory = usedMemory;
                device.TotalMemory = totalMemory;
                device.CpuUtilization = await GetCpuUtilization();
                device.AgentVersion = GetAgentVersion();
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting device info.");
            }

            return device;
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
                    freeGB = Math.Round(((double)memoryStatus.ullAvailPhys / 1024 / 1024 / 1024), 2);
                    totalGB = Math.Round(((double)memoryStatus.ullTotalPhys / 1024 / 1024 / 1024), 2);
                }

                return (totalGB - freeGB, totalGB);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
