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
using Remotely.ScreenCast.Core.Services;
using System.Threading;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.Shared.Win32;

namespace Remotely.ScreenCast.Win.Capture
{
    public class BitBltCapture : ICapturer
    {
        public BitBltCapture()
        {
            Init();
        }

        public event EventHandler<Rectangle> ScreenChanged;

        public bool CaptureFullscreen { get; set; } = true;
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; set; } = Screen.PrimaryScreen.Bounds;
        public Bitmap PreviousFrame { get; set; }
        public int SelectedScreen { get; private set; } = Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen);
        private Graphics Graphic { get; set; }
        public void Dispose()
        {
            Graphic.Dispose();
            CurrentFrame.Dispose();
            PreviousFrame.Dispose();
        }

        public void GetNextFrame()
        {
            try
            {
                PreviousFrame = (Bitmap)CurrentFrame.Clone();
                Graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Logger.Write("Capturer error.  Trying to switch desktops in BitBltCapture.");
                if (Win32Interop.SwitchToInputDesktop())
                {
                    Logger.Write("Switched desktops after capture error in BitBltCapture.");
                }
                Init();
            }
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
