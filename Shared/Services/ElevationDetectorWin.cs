using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Services;

[SupportedOSPlatform("windows")]
public class ElevationDetectorWin : IElevationDetector
{
    public bool IsElevated()
    {
       return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
