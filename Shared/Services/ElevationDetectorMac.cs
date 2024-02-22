using System.Runtime.InteropServices;

namespace Remotely.Shared.Services;

public class ElevationDetectorMac : IElevationDetector
{
    [DllImport("libc", SetLastError = true)]
    private static extern uint geteuid();

    public bool IsElevated()
    {
        return geteuid() == 0;
    }
}
