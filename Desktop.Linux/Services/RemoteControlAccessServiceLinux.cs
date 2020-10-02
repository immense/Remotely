using Remotely.Desktop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Services
{
    public class RemoteControlAccessServiceLinux : IRemoteControlAccessService
    {
        public Task<bool> PromptForAccess()
        {
            throw new NotImplementedException();
        }
    }
}
