using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Linux.X11Interop;
using Remotely.ScreenCast.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.ScreenCast.Core.Models;

namespace Remotely.ScreenCast.Linux.Services
{
    public class X11Input : IKeyboardMouseInput
    {
        public X11Input()
        {
            Display = LibX11.XOpenDisplay(null);
        }

        public IntPtr Display { get; }

        public void SendKeyDown(string key, Viewer viewer)
        {
            try
            {
                key = ConvertJavaScriptKeyToX11Key(key);
                var keySim = LibX11.XStringToKeysym(key);
                if (keySim == null)
                {
                    Logger.Write($"Key not mapped: {key}");
                    return;
                }

                var keyCode = LibX11.XKeysymToKeycode(Display, keySim);
                LibXtst.XTestFakeKeyEvent(Display, keyCode, true, 0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendKeyUp(string key, Viewer viewer)
        {
            try
            {
                key = ConvertJavaScriptKeyToX11Key(key);
                var keySim = LibX11.XStringToKeysym(key);
                if (keySim == null)
                {
                    Logger.Write($"Key not mapped: {key}");
                    return;
                }

                var keyCode = LibX11.XKeysymToKeycode(Display, keySim);
                LibXtst.XTestFakeKeyEvent(Display, keyCode, false, 0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

        }

        public void SendLeftMouseDown(double percentX, double percentY, Viewer viewer)
        {
            try
            {
                SendMouseMove(percentX, percentY, viewer);
                LibXtst.XTestFakeButtonEvent(Display, 1, true, 0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendLeftMouseUp(double percentX, double percentY, Viewer viewer)
        {
            try
            {
                SendMouseMove(percentX, percentY, viewer);
                LibXtst.XTestFakeButtonEvent(Display, 1, false, 0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendMouseMove(double percentX, double percentY, Viewer viewer)
        {
            try
            {
                LibXtst.XTestFakeMotionEvent(Display, 
                    viewer.Capturer.SelectedScreen,
                    (int)(viewer.Capturer.CurrentScreenBounds.Width * percentX), 
                    (int)(viewer.Capturer.CurrentScreenBounds.Height * percentY),
                    0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendMouseWheel(int deltaY, Viewer viewer)
        {
            try
            {
                if (deltaY > 0)
                {
                    LibXtst.XTestFakeButtonEvent(Display, 4, true, 0);
                    LibXtst.XTestFakeButtonEvent(Display, 4, false, 0);
                }
                else
                {
                    LibXtst.XTestFakeButtonEvent(Display, 5, true, 0);
                    LibXtst.XTestFakeButtonEvent(Display, 5, false, 0);
                }
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendRightMouseDown(double percentX, double percentY, Viewer viewer)
        {
            try
            {
                SendMouseMove(percentX, percentY, viewer);
                LibXtst.XTestFakeButtonEvent(Display, 3, true, 0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendRightMouseUp(double percentX, double percentY, Viewer viewer)
        {
            try
            {
                SendMouseMove(percentX, percentY, viewer);
                LibXtst.XTestFakeButtonEvent(Display, 3, false, 0);
                LibX11.XSync(Display, false);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SendText(string transferText, Viewer viewer)
        {
            foreach (var key in transferText)
            {
                SendKeyDown(key.ToString(), viewer);
                SendKeyUp(key.ToString(), viewer);
            }
        }

        private string ConvertJavaScriptKeyToX11Key(string key)
        {
            string keySym;
            switch (key)
            {
                case "ArrowDown":
                    keySym = "Down";
                    break;
                case "ArrowUp":
                    keySym = "Up";
                    break;
                case "ArrowLeft":
                    keySym = "Left";
                    break;
                case "ArrowRight":
                    keySym = "Right";
                    break;
                case "Enter":
                    keySym = "Return";
                    break;
                case "Esc":
                    keySym = "Escape";
                    break;
                case "Alt":
                    keySym = "Alt_L";
                    break;
                case "Control":
                    keySym = "Control_L";
                    break;
                case "Shift":
                    keySym = "Shift_L";
                    break;
                case "PAUSE":
                    keySym = "Pause";
                    break;
                case "BREAK":
                    keySym = "Break";
                    break;
                case "Backspace":
                    keySym = "BackSpace";
                    break;
                case "Tab":
                    keySym = "Tab";
                    break;
                case "CapsLock":
                    keySym = "Caps_Lock";
                    break;
                case "Delete":
                    keySym = "Delete";
                    break;
                case "PageUp":
                    keySym = "Page_Up";
                    break;
                case "PageDown":
                    keySym = "Page_Down";
                    break;
                case "NumLock":
                    keySym = "Num_Lock";
                    break;
                case "ScrollLock":
                    keySym = "Scroll_Lock";
                    break;
                case " ":
                    keySym = "space";
                    break;
                case "!":
                    keySym = "exclam";
                    break;
                case "\"":
                    keySym = "quotedbl";
                    break;
                case "#":
                    keySym = "numbersign";
                    break;
                case "$":
                    keySym = "dollar";
                    break;
                case "%":
                    keySym = "percent";
                    break;
                case "&":
                    keySym = "ampersand";
                    break;
                case "'":
                    keySym = "apostrophe";
                    break;
                case "(":
                    keySym = "parenleft";
                    break;
                case ")":
                    keySym = "parenright";
                    break;
                case "*":
                    keySym = "asterisk";
                    break;
                case "+":
                    keySym = "plus";
                    break;
                case ",":
                    keySym = "comma";
                    break;
                case "-":
                    keySym = "minus";
                    break;
                case ".":
                    keySym = "period";
                    break;
                case "/":
                    keySym = "slash";
                    break;
                case ":":
                    keySym = "colon";
                    break;
                case ";":
                    keySym = "semicolon";
                    break;
                case "<":
                    keySym = "less";
                    break;
                case "=":
                    keySym = "equal";
                    break;
                case ">":
                    keySym = "greater";
                    break;
                case "?":
                    keySym = "question";
                    break;
                case "@":
                    keySym = "at";
                    break;
                case "[":
                    keySym = "bracketleft";
                    break;
                case "\\":
                    keySym = "backslash";
                    break;
                case "]":
                    keySym = "bracketright";
                    break;
                case "_":
                    keySym = "underscore";
                    break;
                case "`":
                    keySym = "grave";
                    break;
                case "{":
                    keySym = "braceleft";
                    break;
                case "|":
                    keySym = "bar";
                    break;
                case "}":
                    keySym = "braceright";
                    break;
                case "~":
                    keySym = "asciitilde";
                    break;
                default:
                    keySym = key;
                    break;
            }
            return keySym;
        }

    }
}
