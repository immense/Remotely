using System.Runtime.InteropServices;

namespace Remotely.Shared.Win32
{
    // https://docs.microsoft.com/en-us/windows/win32/api/shlwapi/nf-shlwapi-isos
    public class Shlwapi
    {
        [DllImport("shlwapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsOS(OsType osType);
    }

    public enum OsType
    {
        OS_WINDOWS = 0,
        OS_NT = 1,
        OS_WIN95ORGREATER = 2,
        OS_NT4ORGREATER = 3,
        OS_WIN98ORGREATER = 5,
        OS_WIN98_GOLD = 6,
        OS_WIN2000ORGREATER = 7,
        OS_WIN2000PRO = 8,
        OS_WIN2000SERVER = 9,
        OS_WIN2000ADVSERVER = 10,
        OS_WIN2000DATACENTER = 11,
        OS_WIN2000TERMINAL = 12,
        OS_EMBEDDED = 13,
        OS_TERMINALCLIENT = 14,
        OS_TERMINALREMOTEADMIN = 15,
        OS_WIN95_GOLD = 16,
        OS_MEORGREATER = 17,
        OS_XPORGREATER = 18,
        OS_HOME = 19,
        OS_PROFESSIONAL = 20,
        OS_DATACENTER = 21,
        OS_ADVSERVER = 22,
        OS_SERVER = 23,
        OS_TERMINALSERVER = 24,
        OS_PERSONALTERMINALSERVER = 25,
        OS_FASTUSERSWITCHING = 26,
        OS_WELCOMELOGONUI = 27,
        OS_DOMAINMEMBER = 28,
        OS_ANYSERVER = 29,
        OS_WOW6432 = 30,
        OS_WEBSERVER = 31,
        OS_SMALLBUSINESSSERVER = 32,
        OS_TABLETPC = 33,
        OS_SERVERADMINUI = 34,
        OS_MEDIACENTER = 35,
        OS_APPLIANCE = 36,
    }
}
