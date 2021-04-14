using Remotely.Shared.Models;
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
    // TODO: Use https://github.com/dotnet/pinvoke for all p/invokes.  Remove signatures from this project.
    public class Win32Interop
    {
        private static IntPtr _lastInputDesktop;

        public static List<WindowsSession> GetActiveSessions()
        {
            var sessions = new List<WindowsSession>();
            var consoleSessionId = Kernel32.WTSGetActiveConsoleSessionId();
            sessions.Add(new WindowsSession()
            {
                ID = consoleSessionId,
                Type = SessionType.Console,
                Name = "Console",
                Username = GetUsernameFromSessionId(consoleSessionId)
            });

            IntPtr ppSessionInfo = IntPtr.Zero;
            var count = 0;
            var enumSessionResult = WTSAPI32.WTSEnumerateSessions(WTSAPI32.WTS_CURRENT_SERVER_HANDLE, 0, 1, ref ppSessionInfo, ref count);
            var dataSize = Marshal.SizeOf(typeof(WTSAPI32.WTS_SESSION_INFO));
            var current = ppSessionInfo;

            if (enumSessionResult != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTSAPI32.WTS_SESSION_INFO sessionInfo = (WTSAPI32.WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTSAPI32.WTS_SESSION_INFO));
                    current += dataSize;
                    if (sessionInfo.State == WTSAPI32.WTS_CONNECTSTATE_CLASS.WTSActive && sessionInfo.SessionID != consoleSessionId)
                    {

                        sessions.Add(new WindowsSession()
                        {
                            ID = sessionInfo.SessionID,
                            Name = sessionInfo.pWinStationName,
                            Type = SessionType.RDP,
                            Username = GetUsernameFromSessionId(sessionInfo.SessionID)
                        });
                    }
                }
            }

            return sessions;
        }

        public static string GetCommandLine()
        {
            var commandLinePtr = Kernel32.GetCommandLine();
            return Marshal.PtrToStringAuto(commandLinePtr);
        }

        public static bool GetCurrentDesktop(out string desktopName)
        {
            var inputDesktop = OpenInputDesktop();
            try
            {
                byte[] deskBytes = new byte[256];
                if (!GetUserObjectInformationW(inputDesktop, UOI_NAME, deskBytes, 256, out uint lenNeeded))
                {
                    desktopName = string.Empty;
                    return false;
                }

                desktopName = Encoding.Unicode.GetString(deskBytes.Take((int)lenNeeded).ToArray()).Replace("\0", "");
                return true;
            }
            finally
            {
                CloseDesktop(inputDesktop);
            }
        }

        public static string GetUsernameFromSessionId(uint sessionId)
        {
            var username = string.Empty;

            if (WTSAPI32.WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTSAPI32.WTS_INFO_CLASS.WTSUserName, out var buffer, out var strLen) && strLen > 1)
            {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSAPI32.WTSFreeMemory(buffer);
            }

            return username;
        }

        public static IntPtr OpenInputDesktop()
        {
            return User32.OpenInputDesktop(0, true, ACCESS_MASK.GENERIC_ALL);
        }

        public static bool OpenInteractiveProcess(string applicationName,
             int targetSessionId,
             bool forceConsoleSession,
             string desktopName,
             bool hiddenWindow,
             out PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new PROCESS_INFORMATION();

            // If not force console, find target session.  If not present,
            // use last active session.
            var dwSessionId = Kernel32.WTSGetActiveConsoleSessionId();
            if (!forceConsoleSession)
            {
                var activeSessions = GetActiveSessions();
                if (activeSessions.Any(x => x.ID == targetSessionId))
                {
                    dwSessionId = (uint)targetSessionId;
                }
                else
                {
                    dwSessionId = activeSessions.Last().ID;
                }
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
            SECURITY_ATTRIBUTES sa = new();
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
            STARTUPINFO si = new();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\" + desktopName;

            // Flags that specify the priority and creation method of the process.
            uint dwCreationFlags;
            if (hiddenWindow)
            {
                dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT | CREATE_NO_WINDOW;
                si.dwFlags = STARTF_USESHOWWINDOW;
                si.wShowWindow = 0;
            }
            else
            {
                dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT | CREATE_NEW_CONSOLE;
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
            SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
        }

        public static MessageBoxResult ShowMessageBox(IntPtr owner,
            string message,
            string caption,
            MessageBoxType messageBoxType)
        {
            return (MessageBoxResult)MessageBox(owner, message, caption, (long)messageBoxType);
        }

        public static bool SwitchToInputDesktop()
        {
            var inputDesktop = OpenInputDesktop();
            try
            {
                if (inputDesktop == IntPtr.Zero)
                {
                    return false;
                }

                var result = SetThreadDesktop(inputDesktop) && SwitchDesktop(inputDesktop);
                CloseDesktop(_lastInputDesktop);
                _lastInputDesktop = inputDesktop;
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}
