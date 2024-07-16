using System;
using System.Runtime.InteropServices;

namespace Immense.RemoteControl.Desktop.Shared.Native.Windows;

public static class WTSAPI32
{
    public static nint WTS_CURRENT_SERVER_HANDLE = nint.Zero;

    public enum WTS_CONNECTSTATE_CLASS
    {
        WTSActive,
        WTSConnected,
        WTSConnectQuery,
        WTSShadow,
        WTSDisconnected,
        WTSIdle,
        WTSListen,
        WTSReset,
        WTSDown,
        WTSInit
    }

    public enum WTS_INFO_CLASS
    {
        WTSInitialProgram,
        WTSApplicationName,
        WTSWorkingDirectory,
        WTSOEMId,
        WTSSessionId,
        WTSUserName,
        WTSWinStationName,
        WTSDomainName,
        WTSConnectState,
        WTSClientBuildNumber,
        WTSClientName,
        WTSClientDirectory,
        WTSClientProductId,
        WTSClientHardwareId,
        WTSClientAddress,
        WTSClientDisplay,
        WTSClientProtocolType,
        WTSIdleTime,
        WTSLogonTime,
        WTSIncomingBytes,
        WTSOutgoingBytes,
        WTSIncomingFrames,
        WTSOutgoingFrames,
        WTSClientInfo,
        WTSSessionInfo
    }


    [DllImport("wtsapi32.dll", SetLastError = true)]
    public static extern int WTSEnumerateSessions(
        nint hServer,
        int Reserved,
        int Version,
        ref nint ppSessionInfo,
        ref int pCount);

    [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
    public static extern void WTSFreeMemory(nint memory);

    [DllImport("Wtsapi32.dll")]
    public static extern bool WTSQuerySessionInformation(nint hServer, uint sessionId, WTS_INFO_CLASS wtsInfoClass, out nint ppBuffer, out uint pBytesReturned);

    [DllImport("wtsapi32.dll", SetLastError = true)]
    static extern nint WTSOpenServer(string pServerName);

    [StructLayout(LayoutKind.Sequential)]
    public struct WTS_SESSION_INFO
    {
        public uint SessionID;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pWinStationName;
        public WTS_CONNECTSTATE_CLASS State;
    }
}
