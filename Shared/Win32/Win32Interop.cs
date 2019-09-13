using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static Remotely.Shared.Win32.ADVAPI32;
using static Remotely.Shared.Win32.User32;

namespace Remotely.Shared.Win32
{
    public class Win32Interop
    {
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
            si.cb = Marshal.SizeOf(si);
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

        public static void SetMonitorState(MonitorState state)
        {
            User32.SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
        }

        public static void SwitchToInputDesktop()
        {
            var inputDesktop = OpenInputDesktop();
            SwitchDesktop(inputDesktop);
            SetThreadDesktop(inputDesktop);
            CloseDesktop(inputDesktop);
        }
    }
}
