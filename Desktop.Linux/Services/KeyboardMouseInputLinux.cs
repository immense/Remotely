using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Linux.X11Interop;
using Remotely.Shared.Utilities;
using System;

namespace Remotely.Desktop.Linux.Services
{
    public class KeyboardMouseInputLinux : IKeyboardMouseInput
    {
        public KeyboardMouseInputLinux()
        {
            Display = LibX11.XOpenDisplay(null);
        }

        public IntPtr Display { get; }

        public void SendKeyDown(string key)
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

        public void SendKeyUp(string key)
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
                    viewer.Capturer.GetSelectedScreenIndex(),
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

        public void SendMouseWheel(int deltaY)
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

        public void SendText(string transferText)
        {
            foreach (var key in transferText)
            {
                SendKeyDown(key.ToString());
                SendKeyUp(key.ToString());
            }
        }

        public void SetKeyStatesUp()
        {
            // Not implemented.
        }

        public void ToggleBlockInput(bool toggleOn)
        {
            // Not implemented.
        }

        private string ConvertJavaScriptKeyToX11Key(string key)
        {
            string keySym = key switch
            {
                "ArrowDown" => "Down",
                "ArrowUp" => "Up",
                "ArrowLeft" => "Left",
                "ArrowRight" => "Right",
                "Enter" => "Return",
                "Esc" => "Escape",
                "Alt" => "Alt_L",
                "Control" => "Control_L",
                "Shift" => "Shift_L",
                "PAUSE" => "Pause",
                "BREAK" => "Break",
                "Backspace" => "BackSpace",
                "Tab" => "Tab",
                "CapsLock" => "Caps_Lock",
                "Delete" => "Delete",
                "PageUp" => "Page_Up",
                "PageDown" => "Page_Down",
                "NumLock" => "Num_Lock",
                "ScrollLock" => "Scroll_Lock",
                "ContextMenu" => "Menu",
                " " => "space",
                "!" => "exclam",
                "\"" => "quotedbl",
                "#" => "numbersign",
                "$" => "dollar",
                "%" => "percent",
                "&" => "ampersand",
                "'" => "apostrophe",
                "(" => "parenleft",
                ")" => "parenright",
                "*" => "asterisk",
                "+" => "plus",
                "," => "comma",
                "-" => "minus",
                "." => "period",
                "/" => "slash",
                ":" => "colon",
                ";" => "semicolon",
                "<" => "less",
                "=" => "equal",
                ">" => "greater",
                "?" => "question",
                "@" => "at",
                "[" => "bracketleft",
                "\\" => "backslash",
                "]" => "bracketright",
                "_" => "underscore",
                "`" => "grave",
                "{" => "braceleft",
                "|" => "bar",
                "}" => "braceright",
                "~" => "asciitilde",
                _ => key,
            };
            return keySym;
        }

    }
}
