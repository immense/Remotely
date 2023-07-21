using Remotely.Shared.Dtos;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Interfaces;

public interface IDeviceInformationService
{
    Task<DeviceClientDto> CreateDevice(string deviceId, string orgId);
}
