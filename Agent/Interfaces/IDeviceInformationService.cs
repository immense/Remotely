using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Interfaces
{
    public interface IDeviceInformationService
    {
        Task<Device> CreateDevice(string deviceId, string orgId);
        Device GetDeviceBase(string deviceID, string orgID);
        (double usedStorage, double totalStorage) GetSystemDriveInfo();
        (double usedGB, double totalGB) GetMemoryInGB();
        string GetAgentVersion();
        List<Drive> GetAllDrives();
        Task<double> GetCpuUtilization();
    }
}
