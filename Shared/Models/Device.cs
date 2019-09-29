using Remotely.Shared.Services;
using Microsoft.Management.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class Device
    {
        public string AgentVersion { get; set; }
        public string CurrentUser { get; set; }
        public string DeviceName { get; set; }

        [JsonIgnore]
        public virtual ICollection<DevicePermissionLink> DevicePermissionLinks { get; set; } = new List<DevicePermissionLink>();
        public List<Drive> Drives { get; set; }

        public double FreeMemory { get; set; }

        public double FreeStorage { get; set; }

        [Key]
        public string ID { get; set; }

        public bool Is64Bit { get; set; }

        public bool IsOnline { get; set; }

        public DateTime LastOnline { get; set; }
        public virtual Organization Organization { get; set; }
        public string OrganizationID { get; set; }
        public Architecture OSArchitecture { get; set; }

        public string OSDescription { get; set; }
        public string Platform { get; set; }

        public int ProcessorCount { get; set; }
        public string ServerVerificationToken { get; set; }
        [StringLength(200)]
        public string Tags { get; set; } = "";

        public double TotalMemory { get; set; }
        public double TotalStorage { get; set; }
        public static Device Create(ConnectionInfo connectionInfo)
        {
            OSPlatform platform = OSUtils.GetPlatform();
            DriveInfo systemDrive;

            if (!string.IsNullOrWhiteSpace(Environment.SystemDirectory))
            {
                systemDrive = DriveInfo.GetDrives()
                    .Where(x=>x.IsReady)
                    .FirstOrDefault(x =>
                        x.RootDirectory.FullName.Contains(Path.GetPathRoot(Environment.SystemDirectory ?? Environment.CurrentDirectory))
                    );
            }
            else
            {
                systemDrive = DriveInfo.GetDrives().FirstOrDefault(x => 
                    x.IsReady &&
                    x.RootDirectory.FullName == Path.GetPathRoot(Environment.CurrentDirectory));
            }

            var device = new Device()
            {
                ID = connectionInfo.DeviceID,
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
                OrganizationID = connectionInfo.OrganizationID,
                CurrentUser = GetCurrentUser()
            };

            if (systemDrive != null && systemDrive.TotalSize > 0 && systemDrive.TotalFreeSpace > 0)
            {
                device.TotalStorage = Math.Round((double)(systemDrive.TotalSize / 1024 / 1024 / 1024), 2);
                var freeStorage = Math.Round((double)(systemDrive.TotalFreeSpace / 1024 / 1024 / 1024), 2);
                device.FreeStorage = freeStorage / device.TotalStorage;
            }

            Tuple<double, double> totalMemory = new Tuple<double, double>(0, 0);

            if (OSUtils.IsWindows)
            {
                totalMemory = GetWinMemoryInGB();
            }
            else if (OSUtils.IsLinux)
            {
                totalMemory = GetLinxMemoryInGB();
            }
          
            if (totalMemory.Item2 > 0)
            {
                device.FreeMemory = totalMemory.Item1 / totalMemory.Item2;
            }
            else
            {
                device.FreeMemory = 0;
            }
            device.TotalMemory = totalMemory.Item2;

            if (File.Exists("Remotely_Agent.dll"))
            {
                device.AgentVersion = FileVersionInfo.GetVersionInfo("Remotely_Agent.dll")?.FileVersion?.ToString()?.Trim();
            }

            return device;
        }


        private static string GetCurrentUser()
        {
            try
            {
                if (OSUtils.IsWindows)
                {
                    var session = CimSession.Create(null);
                    var computerSystem = session.EnumerateInstances("root\\cimv2", "CIM_ComputerSystem");
                    var username = computerSystem.FirstOrDefault().CimInstanceProperties["UserName"].Value ?? "";
                    return username as string;
                }
                else if (OSUtils.IsLinux)
                {
                    var users = OSUtils.StartProcessWithResults("users", "");
                    var username = users?.Split()?.FirstOrDefault()?.Trim();
                    return $"{Environment.UserDomainName}\\{username}";
                }
                throw new Exception("Unsupported operating system.");
            }
            catch
            {
                return "Error Retrieving";
            }
        }

        private static Tuple<double, double> GetLinxMemoryInGB()
        {
            try
            {
                var results = OSUtils.StartProcessWithResults("cat", "/proc/meminfo");
                var resultsArr = results.Split('n');
                var freeKB = resultsArr
                            .FirstOrDefault(x => x.StartsWith("FreeMem"))
                            .Split(" ".ToCharArray(), 2)
                            .Last() // 9168236 kB
                            .Trim()
                            .Split(' ')
                            .First(); // 9168236

                var totalKB = resultsArr
                            .FirstOrDefault(x => x.StartsWith("MemTotal"))
                            .Split(" ".ToCharArray(), 2)
                            .Last() // 16637468 kB
                            .Trim()
                            .Split(' ')
                            .First(); // 16637468

                var freeGB = Math.Round((double.Parse(freeKB) / 1024 / 1024), 2);
                var totalGB = Math.Round((double.Parse(totalKB) / 1024 / 1024), 2);

                return new Tuple<double, double>(freeGB, totalGB);
            }
            catch
            {
                return new Tuple<double, double>(0, 0);
            }
        }

        private static Tuple<double, double> GetWinMemoryInGB()
        {
            try
            {
                var session = CimSession.Create(null);
                var cimOS = session.EnumerateInstances("root\\cimv2", "CIM_OperatingSystem");
                var free = (ulong)(cimOS.FirstOrDefault()?.CimInstanceProperties["FreePhysicalMemory"]?.Value ?? 0);
                var freeGB = Math.Round(((double)free / 1024 / 1024), 2);
                var total = (ulong)(cimOS.FirstOrDefault()?.CimInstanceProperties["TotalVisibleMemorySize"]?.Value ?? 0);
                var totalGB = Math.Round(((double)total / 1024 / 1024), 2);

                return new Tuple<double, double>(freeGB, totalGB);
            }
            catch
            {
                return new Tuple<double, double>(0, 0);
            }
        }
    }
}