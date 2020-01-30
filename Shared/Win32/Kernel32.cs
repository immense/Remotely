using System;
using System.Runtime.InteropServices;

namespace Remotely.Shared.Win32
{
    public static class Kernel32
    {
        #region DLL Imports

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll")]
        public static extern bool ProcessIdToSessionId(uint dwProcessId, ref uint pSessionId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetCommandLine();

        #endregion
    }
}
