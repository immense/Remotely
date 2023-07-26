using Microsoft.Extensions.Logging;
using Remotely.Shared.Dtos;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Remotely.Agent.Services;

public abstract class DeviceInfoGeneratorBase
{
    protected readonly ILogger<DeviceInfoGeneratorBase> _logger;

    public DeviceInfoGeneratorBase(ILogger<DeviceInfoGeneratorBase> logger) 
    {
        _logger = logger;
    }

    protected DeviceClientDto GetDeviceBase(string deviceID, string orgID)
    {

        return new DeviceClientDto()
        {
            ID = deviceID,
            DeviceName = Environment.MachineName,
            Platform = EnvironmentHelper.Platform.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            OSArchitecture = RuntimeInformation.OSArchitecture,
            OSDescription = RuntimeInformation.OSDescription,
            Is64Bit = Environment.Is64BitOperatingSystem,
            IsOnline = true,
            MacAddresses = GetMacAddresses().ToArray(),
            OrganizationID = orgID,
            AgentVersion = AppVersionHelper.GetAppVersion()
    };
    }

    protected (double usedStorage, double totalStorage) GetSystemDriveInfo()
    {
        try
        {
            DriveInfo? systemDrive;
            var allDrives = DriveInfo.GetDrives();

            if (EnvironmentHelper.IsWindows)
            {
                var rootPath = Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\";
                systemDrive = allDrives.FirstOrDefault(x =>
                     x.IsReady &&
                     x.RootDirectory.FullName.Contains(rootPath));
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
            _logger.LogError(ex, "Error getting system drive info.");
        }

        return (0, 0);
    }

    protected List<Drive> GetAllDrives()
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
            _logger.LogError(ex, "Error getting drive info.");
            return new();
        }
    }

    private IEnumerable<string> GetMacAddresses()
    {
        var macAddress = new List<string>();

        try
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();

            if (!nics.Any())
            {
                return macAddress;
            }

            var onlineNics = nics
                .Where(c =>
                    c.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    c.OperationalStatus == OperationalStatus.Up);

            foreach (var adapter in onlineNics)
            {
                var ipProperties = adapter.GetIPProperties();

                var unicastAddresses = ipProperties.UnicastAddresses;
                if (!unicastAddresses.Any(temp => temp.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    continue;
                }

                var address = adapter.GetPhysicalAddress();
                macAddress.Add(address.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting MAC addresses.");
        }

        return macAddress;
    }
}
