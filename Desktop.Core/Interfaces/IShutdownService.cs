using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IShutdownService
    {
        Task Shutdown();
    }
}
