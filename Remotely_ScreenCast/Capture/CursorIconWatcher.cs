using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Win32;

namespace Remotely_ScreenCast.Capture
{
    /// <summary>
    /// A class that can be used to watch for cursor icon changes.
    /// </summary>
    public class CursorIconWatcher
    {
        public static CursorIconWatcher Current { get; private set; } = new CursorIconWatcher();
        public event EventHandler<int> OnChange;
        private System.Timers.Timer ChangeTimer { get; set; }
        private int PreviousCursorHandle { get; set; }
        private User32.CursorInfo CursorInfo;
        private CursorIconWatcher()
        {
            ChangeTimer = new System.Timers.Timer(100);
            ChangeTimer.Elapsed += ChangeTimer_Elapsed;
            ChangeTimer.Start();
        }

        private void ChangeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (OnChange == null)
            {
                return;
            }
            try
            {
                CursorInfo = new User32.CursorInfo();
                CursorInfo.cbSize = Marshal.SizeOf(CursorInfo);
                User32.GetCursorInfo(out CursorInfo);
                if (CursorInfo.flags == User32.CURSOR_SHOWING)
                {
                    var currentCursor = CursorInfo.hCursor.ToInt32();
                    if (currentCursor != PreviousCursorHandle)
                    {
                        OnChange(this, currentCursor);
                    }
                }
            }
            catch { }
        }

    }
}
