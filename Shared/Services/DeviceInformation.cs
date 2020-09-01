using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Remotely.Shared.Services
{
    public class DeviceInformation
    {
        public static async Task<Device> Create(string deviceID, string orgID)
        {
            var device = new Device()
            {
                ID = deviceID,
                DeviceName = Environment.MachineName,
                Platform = EnvironmentHelper.Platform.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                OSArchitecture = RuntimeInformation.OSArchitecture,
                OSDescription = RuntimeInformation.OSDescription,
                Is64Bit = Environment.Is64BitOperatingSystem,
                IsOnline = true,
                OrganizationID = orgID
            };

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

        public static List<Drive> GetAllDrives()
        {
            try
            {
                return DriveInfo.GetDrives().Where(x => x.IsReady).Select(x => new Drive()
                {
                    DriveFormat = x.DriveFormat,
                    DriveType = x.DriveType,
                    Name = x.Name,
                    RootDirectory = x.RootDirectory.FullName,
                    FreeSpace = x.TotalSize > 0 ? x.TotalFreeSpace / x.TotalSize : 0,
                    TotalSize = x.TotalSize > 0 ? Math.Round((double)(x.TotalSize / 1024 / 1024 / 1024), 2) : 0,
                    VolumeLabel = x.VolumeLabel
                }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting drive info.");
                return null;
            }
        }

        public static async Task<double> GetCpuUtilization()
        {
            double totalUtilization = 0;
            var utilizations = new Dictionary<int, Tuple<DateTimeOffset, TimeSpan>>();
            var processes = Process.GetProcesses();

            foreach (var proc in processes)
            {
                try
                {
                    var startTime = DateTimeOffset.Now;
                    var startCpuUsage = proc.TotalProcessorTime;
                    utilizations.Add(proc.Id, new Tuple<DateTimeOffset, TimeSpan>(startTime, startCpuUsage));
                }
                catch
                {
                    continue;
                }
            }

            await Task.Delay(500);

            foreach (var kvp in utilizations)
            {
                var endTime = DateTimeOffset.Now;
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
                if (EnvironmentHelper.IsWindows)
                {
                    return Win32Interop.GetActiveSessions().LastOrDefault()?.Username;
                }
                else if (EnvironmentHelper.IsLinux)
                {
                    var users = EnvironmentHelper.StartProcessWithResults("users", "");
                    return users?.Split()?.FirstOrDefault()?.Trim();
                }
                throw new Exception("Unsupported operating system.");
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting current user.");
                return "Error Retrieving";
            }
        }

        public static (double usedGB, double totalGB) GetMemoryInGB()
        {
            if (EnvironmentHelper.IsWindows)
            {
                return GetWinMemoryInGB();
            }
            else if (EnvironmentHelper.IsLinux)
            {
                return GetLinuxMemoryInGB();
            }
            else
            {
                return (0, 0);
            }
        }

        public static (double usedStorage, double totalStorage) GetSystemDriveInfo()
        {
            try
            {
                DriveInfo systemDrive;
                var allDrives = DriveInfo.GetDrives();

                if (EnvironmentHelper.IsWindows)
                {
                    systemDrive = allDrives.FirstOrDefault(x =>
                         x.IsReady &&
                         x.RootDirectory.FullName.Contains(Path.GetPathRoot(Environment.SystemDirectory ?? Environment.CurrentDirectory)));
                }
                else if (EnvironmentHelper.IsLinux)
                {
                    systemDrive = allDrives.FirstOrDefault(x =>
                        x.IsReady &&
                        x.RootDirectory.FullName == Path.GetPathRoot(Environment.CurrentDirectory));
                }
                else
                {
                    systemDrive = allDrives.FirstOrDefault();
                }


                if (systemDrive != null && systemDrive.TotalSize > 0 && systemDrive.TotalFreeSpace > 0)
                {
                    var totalStorage = Math.Round((double)(systemDrive.TotalSize / 1024 / 1024 / 1024), 2);
                    var usedStorage = Math.Round((double)((systemDrive.TotalSize - systemDrive.TotalFreeSpace) / 1024 / 1024 / 1024), 2);

                    return (usedStorage, totalStorage);
                }

            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting system drive info.");
            }

            return (0, 0);
        }

        private static string GetAgentVersion()
        {
            try
            {
                if (File.Exists("Remotely_Agent.dll"))
                {
                    return FileVersionInfo.GetVersionInfo("Remotely_Agent.dll")?.FileVersion?.ToString()?.Trim();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting agent version.");
            }

            return "0.0.0.0";
        }

        private static (double usedGB, double totalGB) GetLinuxMemoryInGB()
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

        private static (double usedGB, double totalGB) GetWinMemoryInGB()
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
