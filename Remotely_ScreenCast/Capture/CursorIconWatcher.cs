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
        public static CursorIconWatcher Current { get; } = new CursorIconWatcher();
        public event EventHandler<string> OnChange;
        private System.Timers.Timer ChangeTimer { get; set; }
        private string PreviousCursorHandle { get; set; }
        private User32.CursorInfo cursorInfo;


        public string GetCurrentCursor()
        {
            var ci = new User32.CursorInfo();
            ci.cbSize = Marshal.SizeOf(ci);
            User32.GetCursorInfo(out ci);
            return ci.hCursor.ToString();
        }
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
                cursorInfo = new User32.CursorInfo();
                cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
                User32.GetCursorInfo(out cursorInfo);
                if (cursorInfo.flags == User32.CURSOR_SHOWING)
                {
                    var currentCursor = cursorInfo.hCursor.ToString();
                    if (currentCursor != PreviousCursorHandle)
                    {
                        PreviousCursorHandle = currentCursor;
                        OnChange?.Invoke(this, currentCursor);
                    }
                }
                else if (PreviousCursorHandle != "0")
                {
                    PreviousCursorHandle = "0";
                    OnChange?.Invoke(this, "0");
                }
            }
            catch { }
        }

    }
}
