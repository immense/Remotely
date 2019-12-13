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
        [StringLength(100)]
        public string Alias { get; set; }
        public string CurrentUser { get; set; }
        public virtual DeviceGroup DeviceGroup { get; set; }
        public string DeviceGroupID { get; set; }
        public string DeviceName { get; set; }
        public List<Drive> Drives { get; set; }

        public double FreeMemory { get; set; }

        public double FreeStorage { get; set; }

        [Key]
        public string ID { get; set; }

        public bool Is64Bit { get; set; }

        public bool IsOnline { get; set; }

        public DateTime LastOnline { get; set; }
        [JsonIgnore]
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
                CurrentUser = DeviceInformation.GetCurrentUser()
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
                totalMemory = DeviceInformation.GetWinMemoryInGB();
            }
            else if (OSUtils.IsLinux)
            {
                totalMemory = DeviceInformation.GetLinxMemoryInGB();
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

    }
}