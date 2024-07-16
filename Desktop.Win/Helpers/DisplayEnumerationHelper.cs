using Remotely.Shared.Models;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Remotely.Desktop.Win.Helpers;

internal static class DisplaysEnumerationHelper
{
    delegate bool EnumMonitorsDelegate(nint hMonitor, nint hdcMonitor, ref RECT lprcMonitor, nint dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private const int CCHDEVICENAME = 32;
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MonitorInfoEx
    {
        public int Size;
        public RECT Monitor;
        public RECT WorkArea;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string DeviceName;
    }

    [DllImport("user32.dll")]
    static extern bool EnumDisplayMonitors(nint hdc, nint lprcClip, EnumMonitorsDelegate lpfnEnum, nint dwData);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

    public static IEnumerable<DisplayInfo> GetDisplays()
    {
        var result = new List<DisplayInfo>();

        EnumDisplayMonitors(nint.Zero, nint.Zero,
            delegate (nint hMonitor, nint hdcMonitor, ref RECT lprcMonitor, nint dwData)
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf(mi);
                bool success = GetMonitorInfo(hMonitor, ref mi);
                if (success)
                {
                    var info = new DisplayInfo
                    {
                        ScreenSize = new Vector2(mi.Monitor.Right - mi.Monitor.Left, mi.Monitor.Bottom - mi.Monitor.Top),
                        MonitorArea = new Rectangle(mi.Monitor.Left, mi.Monitor.Top, mi.Monitor.Right - mi.Monitor.Left, mi.Monitor.Bottom - mi.Monitor.Top),
                        WorkArea = new Rectangle(mi.WorkArea.Left, mi.WorkArea.Top, mi.WorkArea.Right - mi.WorkArea.Left, mi.WorkArea.Bottom - mi.WorkArea.Top),
                        IsPrimary = mi.Flags > 0,
                        Hmon = hMonitor,
                        DeviceName = mi.DeviceName
                    };
                    result.Add(info);
                }
                return true;
            }, nint.Zero);
        return result;
    }
}