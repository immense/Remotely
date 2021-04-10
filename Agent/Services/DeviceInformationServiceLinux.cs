using Remotely.Agent.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class DeviceInformationServiceLinux : DeviceInformationServiceBase, IDeviceInformationService
    {
        public async Task<Device> CreateDevice(string deviceId, string orgId)
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
                device.AgentVersion = GetAgentVersion();
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting device info.");
            }

            return device;
        }

        private string GetCurrentUser()
        {
            var users = EnvironmentHelper.StartProcessWithResults("users", "");
            return users?.Split()?.FirstOrDefault()?.Trim();
        }

        public (double usedGB, double totalGB) GetMemoryInGB()
        {
            try
            {
                var results = EnvironmentHelper.StartProcessWithResults("cat", "/proc/meminfo");
                var resultsArr = results.Split("\n".ToCharArray());
                var freeKB = resultsArr
                            .FirstOrDefault(x => x.Trim().StartsWith("MemAvailable"))
                            .Trim()
                            .Split(" ".ToCharArray(), 2)
                            .Last() // 9168236 kB
                            .Trim()
                            .Split(' ')
                            .First(); // 9168236

                var totalKB = resultsArr
                            .FirstOrDefault(x => x.Trim().StartsWith("MemTotal"))
                            .Trim()
                            .Split(" ".ToCharArray(), 2)
                            .Last() // 16637468 kB
                            .Trim()
                            .Split(' ')
                            .First(); // 16637468

                var freeGB = Math.Round((double.Parse(freeKB) / 1024 / 1024), 2);
                var totalGB = Math.Round((double.Parse(totalKB) / 1024 / 1024), 2);

                return (totalGB - freeGB, totalGB);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
