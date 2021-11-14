using System;
using System.Runtime.InteropServices;

namespace Remotely.Desktop.XPlat.Native.Linux
{
    public class LibXtst_32
    {
        [DllImport("libXtst")]
        public static extern void XTestFakeKeyEvent(IntPtr display, uint keycode, bool is_press, uint delay);
        [DllImport("libXtst")]
        public static extern void XTestFakeButtonEvent(IntPtr display, uint button, bool is_press, uint delay);
        [DllImport("libXtst")]
        public static extern void XTestFakeMotionEvent(IntPtr display, int screen_number, int x, int y, uint delay);
    }
}
