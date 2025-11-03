using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Remotely.Desktop.Win;

/// <summary>
/// Fullscreen black overlay per monitor that is excluded from screen capture
/// (local user sees black) but is click-through and non-activating so remote
/// input still reaches underlying windows.
/// </summary>
internal static class PrivacyOverlay
{
    private static readonly List<Form> _overlays = new();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

    private const uint WDA_NONE = 0;
    private const uint WDA_EXCLUDEFROMCAPTURE = 0x11; // Windows 10 2004+ (19041)

    // ---- Custom overlay form (click-through, non-activating, hidden from Alt+Tab) ----
    private sealed class OverlayForm : Form
    {
        // WS_EX_TOOLWINDOW (0x00000080)   - hide from Alt+Tab
        // WS_EX_LAYERED    (0x00080000)   - layered window
        // WS_EX_TRANSPARENT(0x00000020)   - mouse clicks pass through
        // WS_EX_NOACTIVATE (0x08000000)   - never takes focus
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080 | 0x00080000 | 0x00000020 | 0x08000000;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation => true;
    }

    public static bool IsActive => _overlays.Count > 0;

    public static void Enable()
    {
        if (IsActive) return;

        foreach (var screen in Screen.AllScreens)
        {
            var f = new OverlayForm
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Bounds = screen.Bounds,
                BackColor = Color.Black,
                TopMost = true,
                ShowInTaskbar = false,
                Opacity = 1
            };

            f.Load += (_, __) =>
            {
                // Exclude from capture so only the local user sees black.
                _ = SetWindowDisplayAffinity(f.Handle, WDA_EXCLUDEFROMCAPTURE);
            };

            // Best-effort: restore affinity on close
            f.FormClosed += (_, __) =>
            {
                try { _ = SetWindowDisplayAffinity(f.Handle, WDA_NONE); } catch { }
            };

            f.Show(); // non-activating
            _overlays.Add(f);
        }
    }

    public static void Disable()
    {
        foreach (var f in _overlays)
        {
            try
            {
                _ = SetWindowDisplayAffinity(f.Handle, WDA_NONE);
                if (!f.IsDisposed) f.Close();
                f.Dispose();
            }
            catch { /* ignore */ }
        }
        _overlays.Clear();
    }

    public static void Toggle()
    {
        if (IsActive) Disable();
        else Enable();
    }
}
