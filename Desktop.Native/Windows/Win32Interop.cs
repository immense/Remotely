using Remotely.Shared.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using static Remotely.Desktop.Native.Windows.ADVAPI32;
using static Remotely.Desktop.Native.Windows.User32;

namespace Remotely.Desktop.Native.Windows;

// TODO: Use https://github.com/microsoft/CsWin32 for all p/invokes.
public class Win32Interop
{
    public static List<WindowsSession> GetActiveSessions()
    {
        var sessions = new List<WindowsSession>();
        var consoleSessionId = Kernel32.WTSGetActiveConsoleSessionId();
        sessions.Add(new WindowsSession()
        {
            Id = consoleSessionId,
            Type = WindowsSessionType.Console,
            Name = "Console",
            Username = GetUsernameFromSessionId(consoleSessionId)
        });

        nint ppSessionInfo = nint.Zero;
        var count = 0;
        var enumSessionResult = WTSAPI32.WTSEnumerateSessions(WTSAPI32.WTS_CURRENT_SERVER_HANDLE, 0, 1, ref ppSessionInfo, ref count);
        var dataSize = Marshal.SizeOf(typeof(WTSAPI32.WTS_SESSION_INFO));
        var current = ppSessionInfo;

        if (enumSessionResult != 0)
        {
            for (int i = 0; i < count; i++)
            {
                var wtsInfo = Marshal.PtrToStructure(current, typeof(WTSAPI32.WTS_SESSION_INFO));
                if (wtsInfo is null)
                {
                    continue;
                }
                var sessionInfo = (WTSAPI32.WTS_SESSION_INFO)wtsInfo;
                current += dataSize;
                if (sessionInfo.State == WTSAPI32.WTS_CONNECTSTATE_CLASS.WTSActive && sessionInfo.SessionID != consoleSessionId)
                {

                    sessions.Add(new WindowsSession()
                    {
                        Id = sessionInfo.SessionID,
                        Name = sessionInfo.pWinStationName,
                        Type = WindowsSessionType.RDP,
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
        return Marshal.PtrToStringAuto(commandLinePtr) ?? string.Empty;
    }

    public static bool GetCurrentDesktop([NotNullWhen(true)] out string? desktopName)
    {
        desktopName = null;
        var inputDesktop = OpenInputDesktop();
        try
        {
            if (TryGetDesktopName(inputDesktop, out desktopName))
            {
                return true;
            }

            return false;
        }
        finally
        {
            CloseDesktop(inputDesktop);
        }
    }



    public static string GetUsernameFromSessionId(uint sessionId)
    {
        var username = string.Empty;

        if (WTSAPI32.WTSQuerySessionInformation(nint.Zero, sessionId, WTSAPI32.WTS_INFO_CLASS.WTSUserName, out var buffer, out var strLen) && strLen > 1)
        {
            username = Marshal.PtrToStringAnsi(buffer);
            WTSAPI32.WTSFreeMemory(buffer);
        }

        return username ?? string.Empty;
    }

    public static nint OpenInputDesktop()
    {
        return User32.OpenInputDesktop(0, true, ACCESS_MASK.GENERIC_ALL);
    }

    public static bool CreateInteractiveSystemProcess(
        string commandLine,
        int targetSessionId,
        bool hiddenWindow,
        out PROCESS_INFORMATION procInfo)
    {
        uint winlogonPid = 0;
        var hUserTokenDup = nint.Zero;
        var hPToken = nint.Zero;
        var hProcess = nint.Zero;

        procInfo = new PROCESS_INFORMATION();

        var dwSessionId = ResolveWindowsSession(targetSessionId);

        // Obtain the process ID of the winlogon process that is running within the currently active session.
        var processes = Process.GetProcessesByName("winlogon");
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
        var sa = new SECURITY_ATTRIBUTES();
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
        var si = new STARTUPINFO();
        si.cb = Marshal.SizeOf(si);
        si.lpDesktop = @"winsta0\" + ResolveDesktopName(dwSessionId);

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
        var result = CreateProcessAsUser(
            hUserTokenDup,
            null,
            commandLine,
            ref sa,
            ref sa,
            false,
            dwCreationFlags,
            nint.Zero,
            null,
            ref si,
            out procInfo);

        // Invalidate the handles.
        Kernel32.CloseHandle(hProcess);
        Kernel32.CloseHandle(hPToken);
        Kernel32.CloseHandle(hUserTokenDup);

        return result;
    }

    public static string ResolveDesktopName(uint targetSessionId)
    {
        var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var logonUiPath = Path.Combine(winDir, "System32", "LogonUI.exe");
        var consentPath = Path.Combine(winDir, "System32", "consent.exe");

        var isLogonScreenVisible = Process
            .GetProcessesByName("LogonUI")
            .Any(x => x.SessionId == targetSessionId && x.MainModule?.FileName.Equals(logonUiPath, StringComparison.OrdinalIgnoreCase) == true);

        var isSecureDesktopVisible = Process
            .GetProcessesByName("consent")
            .Any(x => x.SessionId == targetSessionId && x.MainModule?.FileName.Equals(consentPath, StringComparison.OrdinalIgnoreCase) == true);

        if (isLogonScreenVisible || isSecureDesktopVisible)
        {
            return "Winlogon";
        }

        return "Default";
    }

    public static uint ResolveWindowsSession(int targetSessionId)
    {
        var activeSessions = GetActiveSessions();
        if (activeSessions.Any(x => x.Id == targetSessionId))
        {
            // If exact match is found, return that session.
            return (uint)targetSessionId;
        }

        if (Shlwapi.IsOS(OsType.OS_ANYSERVER))
        {
            // If Windows Server, default to console session.
            return Kernel32.WTSGetActiveConsoleSessionId();
        }

        // If consumer version and there's an RDP session active, return that.
        if (activeSessions.Find(x => x.Type == WindowsSessionType.RDP) is { } rdSession)
        {
            return rdSession.Id;
        }

        // Otherwise, return the console session.
        return Kernel32.WTSGetActiveConsoleSessionId();
    }

    public static void SetMonitorState(MonitorState state)
    {
        SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
    }

    public static MessageBoxResult ShowMessageBox(nint owner,
        string message,
        string caption,
        MessageBoxType messageBoxType)
    {
        return (MessageBoxResult)MessageBox(owner, message, caption, (long)messageBoxType);
    }

    public static bool SwitchToInputDesktop()
    {
        try
        {
            var inputDesktop = OpenInputDesktop();

            try
            {
                if (inputDesktop == nint.Zero)
                {
                    return false;
                }

                return SetThreadDesktop(inputDesktop);
            }
            finally
            {
                CloseDesktop(inputDesktop);
            }
        }
        catch
        {
            return false;
        }
    }

    public static void SetConsoleWindowVisibility(bool isVisible)
    {
        var handle = Kernel32.GetConsoleWindow();

        if (isVisible)
        {
            ShowWindow(handle, (int)SW.SW_SHOW);
        }
        else
        {
            ShowWindow(handle, (int)SW.SW_HIDE);
        }

        Kernel32.CloseHandle(handle);
    }

    public static bool TryGetDesktopName(nint desktopHandle, [NotNullWhen(true)] out string? desktopName)
    {
        var deskBytes = new byte[256];
        if (!GetUserObjectInformationW(desktopHandle, UOI_NAME, deskBytes, 256, out uint lenNeeded))
        {
            desktopName = string.Empty;
            return false;
        }

        desktopName = Encoding.Unicode
            .GetString(deskBytes.Take((int)lenNeeded).ToArray())
            .Replace("\0", "");

        return true;
    }
}
