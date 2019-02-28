using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using Win32;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using Remotely_ScreenCast.Utilities;
using System.Threading;

namespace Remotely_ScreenCast.Capture
{
    public class BitBltCapture : ICapturer
    {
        public Bitmap CurrentFrame { get; set; }
        public Bitmap PreviousFrame { get; set; }
        public bool IsCapturing { get; set; }
        public bool CaptureFullscreen { get; set; } = true;
        public int PauseForMilliseconds { get; set; }
        public EventHandler<Rectangle> ScreenChanged { get; set; }
        private Stopwatch FramerateTimer { get; } = Stopwatch.StartNew();
        public int SelectedScreen
        {
            get
            {
                return selectedScreen;
            }
            set
            {
                if (value == selectedScreen)
                {
                    return;
                }
                if (Screen.AllScreens.Length >= value + 1)
                {
                    selectedScreen = value;
                }
                else
                {
                    selectedScreen = 0;
                }
                CurrentScreenBounds = Screen.AllScreens[selectedScreen].Bounds;
                ScreenChanged?.Invoke(this, CurrentScreenBounds);
            }
        }
        public Rectangle CurrentScreenBounds { get; set; } = Screen.PrimaryScreen.Bounds;
        private int selectedScreen = 0;
        private Graphics graphic;
        private string desktopName;


        public BitBltCapture()
        {
            CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            graphic = Graphics.FromImage(CurrentFrame);
			desktopName = Win32Interop.GetCurrentDesktop();
        }

        public void Capture()
        {
			var currentDesktop = Win32Interop.GetCurrentDesktop();
            if (currentDesktop != desktopName)
            {
                desktopName = currentDesktop;
                var inputDesktop = Win32Interop.OpenInputDesktop();
                var success = User32.SetThreadDesktop(inputDesktop);
                User32.CloseDesktop(inputDesktop);
                Logger.Write($"Set thread desktop: {success}");
            }

            // Keep framerate below 30 FPS.
            if (FramerateTimer.Elapsed.TotalMilliseconds > 33)
            {
                Thread.Sleep((int)FramerateTimer.Elapsed.TotalMilliseconds);
            }
            FramerateTimer.Restart();

            PreviousFrame = (Bitmap)CurrentFrame.Clone();

            try
            {
                graphic.CopyFromScreen(0 + CurrentScreenBounds.Left, 0 + CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
