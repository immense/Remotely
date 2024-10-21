using Remotely.Shared.Dtos;
using System.Threading.Tasks;

namespace Remotely.Agent.Interfaces;

public interface IDeviceInformationService
{
    Task<DeviceClientDto> CreateDevice(string deviceId, string orgId);
}
