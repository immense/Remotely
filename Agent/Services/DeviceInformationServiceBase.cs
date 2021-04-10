using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class DeviceInformationServiceBase
    {
        public Device GetDeviceBase(string deviceID, string orgID)
        {

            return new Device()
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
        }

        public (double usedStorage, double totalStorage) GetSystemDriveInfo()
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
                else
                {
                    systemDrive = allDrives.FirstOrDefault(x =>
                        x.IsReady &&
                        x.RootDirectory.FullName == Path.GetPathRoot(Environment.CurrentDirectory));
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

        public string GetAgentVersion()
        {
            try
            {
                if (File.Exists("Remotely_Agent.dll"))
                {
                    return FileVersionInfo.GetVersionInfo("Remotely_Agent.dll").FileVersion.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error getting agent version.");
            }

            return "0.0.0.0";
        }

        public List<Drive> GetAllDrives()
        {
            try
            {
                return DriveInfo.GetDrives().Where(x => x.IsReady).Select(x => new Drive()
                {
                    DriveFormat = x.DriveFormat,
                    DriveType = x.DriveType,
                    Name = x.Name,
                    RootDirectory = x.RootDirectory.FullName,
                    FreeSpace = x.TotalFreeSpace > 0 ? Math.Round((double)(x.TotalFreeSpace / 1024 / 1024 / 1024), 2) : 0,
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

        public async Task<double> GetCpuUtilization()
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

    }
}
