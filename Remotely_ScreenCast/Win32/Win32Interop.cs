using Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static Win32.ADVAPI32;
using static Win32.User32;
using System.Windows.Forms;

namespace Win32
{
    public class Win32Interop
    {
        public static bool OpenInteractiveProcess(string applicationName, string desktopName, bool hiddenWindow, out PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new PROCESS_INFORMATION();

            // Obtain session ID for active session.
            uint dwSessionId = Kernel32.WTSGetActiveConsoleSessionId();

            // Check for RDP session.  If active, use that session ID instead.
            var rdpSessionID = GetRDPSession();
            if (rdpSessionID > 0)
            {
                dwSessionId = rdpSessionID;
            }

            // Obtain the process ID of the winlogon process that is running within the currently active session.
            Process[] processes = Process.GetProcessesByName("winlogon");
            foreach (Process p in processes)
            {
                if ((uint)p.SessionId == dwSessionId)
                {
                    winlogonPid = (uint)p.Id;
                }
            }

            // Obtain a handle to the winlogon process.
            hProcess = Kernel32.OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            // Obtain a handle to the access token of the winlogon process.
            if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                Kernel32.CloseHandle(hProcess);
                return false;
            }

            // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser.
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            // Copy the access token of the winlogon process; the newly created token will be a primary token.
            if (!DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, TOKEN_TYPE.TokenPrimary, out hUserTokenDup))
            {
                Kernel32.CloseHandle(hProcess);
                Kernel32.CloseHandle(hPToken);
                return false;
            }

            // By default, CreateProcessAsUser creates a process on a non-interactive window station, meaning
            // the window station has a desktop that is invisible and the process is incapable of receiving
            // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user 
            // interaction with the new process.
            STARTUPINFO si = new STARTUPINFO();
            si.cb = (int)Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\" + desktopName;

            // Flags that specify the priority and creation method of the process.
            uint dwCreationFlags;
            if (hiddenWindow)
            {
                dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NO_WINDOW | DETACHED_PROCESS;
            }
            else
            {
                dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;
            }

            // Create a new process in the current user's logon session.
            bool result = CreateProcessAsUser(hUserTokenDup, null, applicationName, ref sa, ref sa, false, dwCreationFlags, IntPtr.Zero, null, ref si, out procInfo);

            // Invalidate the handles.
            Kernel32.CloseHandle(hProcess);
            Kernel32.CloseHandle(hPToken);
            Kernel32.CloseHandle(hUserTokenDup);

            return result;
        }

        public static uint GetRDPSession()
        {
            IntPtr ppSessionInfo = IntPtr.Zero;
            Int32 count = 0;
            Int32 retval = WTSAPI32.WTSEnumerateSessions(WTSAPI32.WTS_CURRENT_SERVER_HANDLE, 0, 1, ref ppSessionInfo, ref count);
            Int32 dataSize = Marshal.SizeOf(typeof(WTSAPI32.WTS_SESSION_INFO));
            var sessList = new List<WTSAPI32.WTS_SESSION_INFO>();
            Int64 current = (Int64)ppSessionInfo;

            if (retval != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTSAPI32.WTS_SESSION_INFO sessInf = (WTSAPI32.WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTSAPI32.WTS_SESSION_INFO));
                    current += dataSize;
                    sessList.Add(sessInf);
                }
            }
            uint retVal = 0;
            var rdpSession = sessList.Find(ses => ses.pWinStationName.ToLower().Contains("rdp") && ses.State == 0);
            if (sessList.Exists(ses => ses.pWinStationName.ToLower().Contains("rdp") && ses.State == 0))
            {
                retVal = (uint)rdpSession.SessionID;
            }
            return retVal;
        }
        public static IntPtr OpenInputDesktop()
        {
            return User32.OpenInputDesktop(0, false, ACCESS_MASK.GENERIC_ALL);
        }
        public static uint SendLeftMouseDown(int x, int y)
        {
            // Coordinates must be normalized.  The bottom-right coordinate is mapped to 65535.
            var normalizedX = x * (double)65535;
            var normalizedY = y * (double)65535;
            var union = new InputUnion() { mi = new MOUSEINPUT() { dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.VIRTUALDESK, dx = (int)normalizedX, dy = (int)normalizedY, time = 0, mouseData = 0, dwExtraInfo = (UIntPtr)GetMessageExtraInfo() } };
            var input = new INPUT() { type = InputType.MOUSE, U = union };
            return SendInput(1, new INPUT[] { input }, INPUT.Size);
        }
        public static uint SendLeftMouseUp(int x, int y)
        {
            // Coordinates must be normalized.  The bottom-right coordinate is mapped to 65535.
            var normalizedX = x * (double)65535;
            var normalizedY = y * (double)65535;
            var union = new InputUnion() { mi = new MOUSEINPUT() { dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTUP | MOUSEEVENTF.VIRTUALDESK, dx = (int)normalizedX, dy = (int)normalizedY, time = 0, mouseData = 0, dwExtraInfo = (UIntPtr)GetMessageExtraInfo() } };
            var input = new INPUT() { type = InputType.MOUSE, U = union };
            return SendInput(1, new INPUT[] { input }, INPUT.Size);
        }
        public static uint SendRightMouseDown(int x, int y)
        {
            // Coordinates must be normalized.  The bottom-right coordinate is mapped to 65535.
            var normalizedX = x * (double)65535;
            var normalizedY = y * (double)65535;
            var union = new InputUnion() { mi = new MOUSEINPUT() { dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.RIGHTDOWN | MOUSEEVENTF.VIRTUALDESK, dx = (int)normalizedX, dy = (int)normalizedY, time = 0, mouseData = 0, dwExtraInfo = (UIntPtr)GetMessageExtraInfo() } };
            var input = new INPUT() { type = InputType.MOUSE, U = union };
            return SendInput(1, new INPUT[] { input }, INPUT.Size);
        }
        public static uint SendRightMouseUp(int x, int y)
        {
            // Coordinates must be normalized.  The bottom-right coordinate is mapped to 65535.
            var normalizedX = x * (double)65535;
            var normalizedY = y * (double)65535;
            var union = new InputUnion() { mi = new MOUSEINPUT() { dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.RIGHTUP | MOUSEEVENTF.VIRTUALDESK, dx = (int)normalizedX, dy = (int)normalizedY, time = 0, mouseData = 0, dwExtraInfo = (UIntPtr)GetMessageExtraInfo() } };
            var input = new INPUT() { type = InputType.MOUSE, U = union };
            return SendInput(1, new INPUT[] { input }, INPUT.Size);
        }

        // Offsets are used in case there's a multi-monitor setup where the left-most or top-most edge of the virtual screen
        // is not 0.
        public static uint SendMouseMove(double x, double y)
        {
            // Coordinates must be normalized.  The bottom-right coordinate is mapped to 65535.
            var normalizedX = x * (double)65535;
            var normalizedY = y * (double)65535;
            var union = new InputUnion() { mi = new MOUSEINPUT() { dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE | MOUSEEVENTF.VIRTUALDESK, dx = (int)normalizedX, dy = (int)normalizedY, time = 0, mouseData = 0, dwExtraInfo = (UIntPtr)GetMessageExtraInfo() } };
            var input = new INPUT() { type = InputType.MOUSE, U = union };
            return SendInput(1, new INPUT[] { input }, INPUT.Size);
        }

        public static uint SendMouseWheel(int deltaY)
        {
            if (deltaY < 0)
            {
                deltaY = -120;
            }
            else if (deltaY > 0)
            {
                deltaY = 120;
            }
            var union = new User32.InputUnion() { mi = new User32.MOUSEINPUT() { dwFlags = MOUSEEVENTF.WHEEL, dx = 0, dy = 0, time = 0, mouseData = deltaY, dwExtraInfo = GetMessageExtraInfo() } };
            var input = new User32.INPUT() { type = InputType.MOUSE, U = union };
            return SendInput(1, new User32.INPUT[] { input }, INPUT.Size);
        }

        public static void SendKeyDown(VirtualKey key)
        {
            var union = new InputUnion()
            {
                ki = new KEYBDINPUT()
                {
                    wVk = key,
                    wScan = 0,
                    time = 0,
                    dwExtraInfo = GetMessageExtraInfo()
                }
            };
            var input = new INPUT() { type = InputType.KEYBOARD, U = union };
            SendInput(1, new INPUT[] { input }, INPUT.Size);
        }
        public static void SendKeyUp(VirtualKey key)
        {
            var union = new InputUnion()
            {
                ki = new KEYBDINPUT()
                {
                    wVk = key,
                    wScan = 0,
                    time = 0,
                    dwFlags = KEYEVENTF.KEYUP,
                    dwExtraInfo = GetMessageExtraInfo()
                }
            };
            var input = new INPUT() { type = InputType.KEYBOARD, U = union };
            SendInput(1, new INPUT[] { input }, INPUT.Size);
        }
        public static string GetCurrentDesktop()
        {
            var inputDesktop = OpenInputDesktop();
            byte[] deskBytes = new byte[256];
            uint lenNeeded;
            var success = GetUserObjectInformationW(inputDesktop, UOI_NAME, deskBytes, 256, out lenNeeded);
            if (!success)
            {
                CloseDesktop(inputDesktop);
                return "Default";
            }
            var desktopName = Encoding.Unicode.GetString(deskBytes.Take((int)lenNeeded).ToArray()).Replace("\0", "");
            CloseDesktop(inputDesktop);
            return desktopName;
        }

        public static short ConvertJavaScriptKeyToVirtualKey(string key)
        {
            short keyCode;
            switch (key)
            {
                case "Down":
                case "ArrowDown":
                    keyCode = (short)VirtualKey.DOWN;
                    break;
                case "Up":
                case "ArrowUp":
                    keyCode = (short)VirtualKey.UP;
                    break;
                case "Left":
                case "ArrowLeft":
                    keyCode = (short)VirtualKey.LEFT;
                    break;
                case "Right":
                case "ArrowRight":
                    keyCode = (short)VirtualKey.RIGHT;
                    break;
                case "Enter":
                    keyCode = (short)VirtualKey.RETURN;
                    break;
                case "Esc":
                case "Escape":
                    keyCode = (short)VirtualKey.ESCAPE;
                    break;
                case "Alt":
                    keyCode = (short)VirtualKey.MENU;
                    break;
                case "Control":
                    keyCode = (short)VirtualKey.CONTROL;
                    break;
                case "Shift":
                    keyCode = (short)VirtualKey.SHIFT;
                    break;
                case "PAUSE":
                    keyCode = (short)VirtualKey.PAUSE;
                    break;
                case "BREAK":
                    keyCode = (short)VirtualKey.PAUSE;
                    break;
                case "Backspace":
                    keyCode = (short)VirtualKey.BACK;
                    break;
                case "Tab":
                    keyCode = (short)VirtualKey.TAB;
                    break;
                case "CapsLock":
                    keyCode = (short)VirtualKey.CAPITAL;
                    break;
                case "Delete":
                    keyCode = (short)VirtualKey.DELETE;
                    break;
                case "Home":
                    keyCode = (short)VirtualKey.HOME;
                    break;
                case "End":
                    keyCode = (short)VirtualKey.END;
                    break;
                case "PageUp":
                    keyCode = (short)VirtualKey.PRIOR;
                    break;
                case "PageDown":
                    keyCode = (short)VirtualKey.NEXT;
                    break;
                case "NumLock":
                    keyCode = (short)VirtualKey.NUMLOCK;
                    break;
                case "Insert":
                    keyCode = (short)VirtualKey.INSERT;
                    break;
                case "ScrollLock":
                    keyCode = (short)VirtualKey.SCROLL;
                    break;
                case "F1":
                    keyCode = (short)VirtualKey.F1;
                    break;
                case "F2":
                    keyCode = (short)VirtualKey.F2;
                    break;
                case "F3":
                    keyCode = (short)VirtualKey.F3;
                    break;
                case "F4":
                    keyCode = (short)VirtualKey.F4;
                    break;
                case "F5":
                    keyCode = (short)VirtualKey.F5;
                    break;
                case "F6":
                    keyCode = (short)VirtualKey.F6;
                    break;
                case "F7":
                    keyCode = (short)VirtualKey.F7;
                    break;
                case "F8":
                    keyCode = (short)VirtualKey.F8;
                    break;
                case "F9":
                    keyCode = (short)VirtualKey.F9;
                    break;
                case "F10":
                    keyCode = (short)VirtualKey.F10;
                    break;
                case "F11":
                    keyCode = (short)VirtualKey.F11;
                    break;
                case "F12":
                    keyCode = (short)VirtualKey.F12;
                    break;
                default:
                    keyCode = User32.VkKeyScan(Convert.ToChar(key));
                    break;
            }
            return keyCode;
        }
    }
}
