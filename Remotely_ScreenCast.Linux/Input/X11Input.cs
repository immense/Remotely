using Remotely_ScreenCast.Core.Input;
using Remotely_ScreenCast.Core.Utilities;
using Remotely_ScreenCast.Linux.X11Interop;
using Remotely_ScreenCast.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Remotely_ScreenCast.Linux.Capture;

namespace Remotely_ScreenCast.Linux.Input
{
    public class X11Input : IKeyboardMouseInput
    {

        public void SendKeyDown(string key)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                var keySim = LibX11.XStringToKeysym(key);
                if (keySim == null)
                {
                    Logger.Write($"Key not mapped: {key}");
                    return;
                }

                var keyCode = LibX11.XKeysymToKeycode(display, keySim);
                LibX11.XTestFakeKeyEvent(display, keyCode, true, 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendKeyUp(string key)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                var keySim = LibX11.XStringToKeysym(key);
                if (keySim == null)
                {
                    Logger.Write($"Key not mapped: {key}");
                    return;
                }

                var keyCode = LibX11.XKeysymToKeycode(display, keySim);
                LibX11.XTestFakeKeyEvent(display, keyCode, false, 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

        }

        public uint SendLeftMouseDown(double percentX, double percentY)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                SendMouseMove(percentX, percentY);
                LibX11.XTestFakeButtonEvent(display, 1, true, 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return 1;
            }
        }

        public uint SendLeftMouseUp(double percentX, double percentY)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                SendMouseMove(percentX, percentY);
                LibX11.XTestFakeButtonEvent(display, 1, false, 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return 1;
            }
        }

        public uint SendMouseMove(double percentX, double percentY)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                var capturer = new X11Capture();
                var width = capturer.GetVirtualScreenWidth();
                var height = capturer.GetVirtualScreenHeight();
                LibX11.XTestFakeMotionEvent(display, 0, (int)(width * percentX), (int)(height * percentY), 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            return 0;
        }

        public uint SendMouseWheel(int deltaY)
        {
            return 0;
        }

        public uint SendRightMouseDown(double percentX, double percentY)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                SendMouseMove(percentX, percentY);
                LibX11.XTestFakeButtonEvent(display, 3, true, 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return 1;
            }
        }

        public uint SendRightMouseUp(double percentX, double percentY)
        {
            try
            {
                var display = LibX11.XOpenDisplay(null);
                SendMouseMove(percentX, percentY);
                LibX11.XTestFakeButtonEvent(display, 3, false, 1);
                LibX11.XSync(display, false);
                //LibX11.XCloseDisplay(display);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return 1;
            }
        }
    }
}
