using System.Runtime.InteropServices;

namespace Immense.RemoteControl.Desktop.Shared.Native.Linux;

public class LibXtst
{
    [DllImport("libXtst")]
    public static extern bool XTestQueryExtension(nint display, out int event_base, out int error_base, out int major_version, out int minor_version);
    [DllImport("libXtst")]
    public static extern void XTestFakeKeyEvent(nint display, uint keycode, bool is_press, ulong delay);
    [DllImport("libXtst")]
    public static extern void XTestFakeButtonEvent(nint display, uint button, bool is_press, ulong delay);
    [DllImport("libXtst")]
    public static extern void XTestFakeMotionEvent(nint display, int screen_number, int x, int y, ulong delay);
}
