using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using Remotely.ScreenCast.Core.Services;
using System.Threading;
using Remotely.ScreenCast.Core.Capture;

namespace Remotely.ScreenCast.Win.Capture
{
    public class BitBltCapture : ICapturer
    {
        public BitBltCapture()
        {
            Init();
        }

        public bool CaptureFullscreen { get; set; } = true;
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; set; } = Screen.PrimaryScreen.Bounds;
        public bool IsCapturing { get; set; }
        public int PauseForMilliseconds { get; set; }
        public Bitmap PreviousFrame { get; set; }
        public event EventHandler<Rectangle> ScreenChanged;
        public int SelectedScreen { get; private set; } = Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen);
        private Graphics Graphic { get; set; }

        private object ScreenLock { get; } = new object();

        public void Capture()
        {
            try
            {
                lock (ScreenLock)
                {
                    PreviousFrame = (Bitmap)CurrentFrame.Clone();
                    Graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Init();
            }
        }

        public void Dispose()
        {
            Graphic.Dispose();
            CurrentFrame.Dispose();
            PreviousFrame.Dispose();
        }

        public int GetScreenCount()
        {
            return Screen.AllScreens.Length;
        }

        public Rectangle GetVirtualScreenBounds()
        {
            return SystemInformation.VirtualScreen;
        }

        public void Init()
        {
            CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            Graphic = Graphics.FromImage(CurrentFrame);
        }

        public void SetSelectedScreen(int screenNumber)
        {
            if (screenNumber == SelectedScreen)
            {
                return;
            }
            lock (ScreenLock)
            {
                if (GetScreenCount() >= screenNumber + 1)
                {
                    SelectedScreen = screenNumber;
                }
                else
                {
                    SelectedScreen = 0;
                }
                CurrentScreenBounds = Screen.AllScreens[SelectedScreen].Bounds;
                CaptureFullscreen = true;
                Init();
                ScreenChanged?.Invoke(this, CurrentScreenBounds);
            }
        }
    }
}
