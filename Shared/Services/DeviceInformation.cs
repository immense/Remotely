using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Services
{
    public class DeviceInformation
    {
        public static async Task<Device> Create(string deviceID, string orgID)
        {
            OSPlatform platform = OSUtils.GetPlatform();

            var systemDrive = DriveInfo.GetDrives().FirstOrDefault(x =>
                x.IsReady &&
                x.RootDirectory.FullName.Contains(Path.GetPathRoot(Environment.SystemDirectory ?? Environment.CurrentDirectory)));

            var device = new Device()
            {
                ID = deviceID,
                DeviceName = Environment.MachineName,
                Platform = platform.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                OSArchitecture = RuntimeInformation.OSArchitecture,
                OSDescription = RuntimeInformation.OSDescription,
                Is64Bit = Environment.Is64BitOperatingSystem,
                IsOnline = true,
                Drives = DriveInfo.GetDrives().Where(x => x.IsReady).Select(x => new Drive()
                {
                    DriveFormat = x.DriveFormat,
                    DriveType = x.DriveType,
                    Name = x.Name,
                    RootDirectory = x.RootDirectory.FullName,
                    FreeSpace = x.TotalSize > 0 ? x.TotalFreeSpace / x.TotalSize : 0,
                    TotalSize = x.TotalSize > 0 ? Math.Round((double)(x.TotalSize / 1024 / 1024 / 1024), 2) : 0,
                    VolumeLabel = x.VolumeLabel
                }).ToList(),
                OrganizationID = orgID,
                CurrentUser = DeviceInformation.GetCurrentUser()
            };

            if (systemDrive != null && systemDrive.TotalSize > 0 && systemDrive.TotalFreeSpace > 0)
            {
                device.TotalStorage = Math.Round((double)(systemDrive.TotalSize / 1024 / 1024 / 1024), 2);
                device.UsedStorage = Math.Round((double)((systemDrive.TotalSize - systemDrive.TotalFreeSpace) / 1024 / 1024 / 1024), 2);
            }


            var (usedMemory, totalMemory) = GetMemoryInGB();
            device.UsedMemory = usedMemory;
            device.TotalMemory = totalMemory;

            device.CpuUtilization = await GetCpuUtilization();

            if (File.Exists("Remotely_Agent.dll"))
            {
                device.AgentVersion = FileVersionInfo.GetVersionInfo("Remotely_Agent.dll")?.FileVersion?.ToString()?.Trim();
            }

            return device;
        }

        public static async Task<double> GetCpuUtilization()
        {
            double totalUtilization = 0;
            var utilizations = new Dictionary<int, Tuple<DateTime, TimeSpan>>();
            var processes = Process.GetProcesses();

            foreach (var proc in processes)
            {
                try
                {
                    var startTime = DateTime.UtcNow;
                    var startCpuUsage = proc.TotalProcessorTime;
                    utilizations.Add(proc.Id, new Tuple<DateTime, TimeSpan>(startTime, startCpuUsage));
                }
                catch
                {
                    continue;
                }
            }

            await Task.Delay(500);

            foreach (var kvp in utilizations)
            {
                var endTime = DateTime.UtcNow;
                try
                {
                    var proc = Process.GetProcessById(kvp.Key);
                    var startTime = kvp.Value.Item1;
                    var startCpuUsage = kvp.Value.Item2;
                    var endCpuUsage = proc.TotalProcessorTime;
                    var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                    var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                    var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
                    totalUtilization += cpuUsageTotal;
                }
                catch
                {
                    continue;
                }
            }

            return totalUtilization;
        }

        public static string GetCurrentUser()
        {
            try
            {
                if (OSUtils.IsWindows)
                {
                    return Win32Interop.GetActiveSessions().LastOrDefault()?.Username;
                }
                else if (OSUtils.IsLinux)
                {
                    var users = OSUtils.StartProcessWithResults("users", "");
                    return users?.Split()?.FirstOrDefault()?.Trim();
                }
                throw new Exception("Unsupported operating system.");
            }
            catch
            {
                return "Error Retrieving";
            }
        }

        public static (double, double) GetMemoryInGB()
        {
            if (OSUtils.IsWindows)
            {
                return GetWinMemoryInGB();
            }
            else if (OSUtils.IsLinux)
            {
                return GetLinxMemoryInGB();
            }
            else
            {
                return (0, 0);
            }
        }
        private static (double, double) GetLinxMemoryInGB()
        {
            try
            {
                var results = OSUtils.StartProcessWithResults("cat", "/proc/meminfo");
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

        private static (double, double) GetWinMemoryInGB()
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
