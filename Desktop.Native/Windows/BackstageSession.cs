namespace Remotely.Desktop.Native.Windows;
public record BackstageSession(nint WindowStation, nint Desktop, ADVAPI32.PROCESS_INFORMATION ProcInfo);