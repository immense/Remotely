using Immense.RemoteControl.Server.Extensions;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Server.Services;
using Immense.RemoteControl.Shared.Enums;

namespace Immense.RemoteControl.Server.Abstractions;

/// <summary>
/// Contains functionality that needs to be implemented outside of the remote control process.
/// This service will be registered as a singleton within <see cref="RemoteControlServerBuilder"/>.
/// </summary>
public interface IHubEventHandler
{
    /// <summary>
    /// This is called when a viewer has selected a different Windows session.  A new remote control
    /// process should be started in that session.  The viewer's connection ID should be passed into the
    /// new process using the --viewers argument, and they'll be automatically signaled when the new
    /// session is ready.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="viewerConnectionId"></param>
    /// <param name="targetWindowsSession"></param>
    /// <returns></returns>
    Task ChangeWindowsSession(RemoteControlSession session, string viewerConnectionId, int targetWindowsSession);

    /// <summary>
    /// <para>
    ///     This is called when the viewer invokes "Ctrl+Alt+Del".  On the desktop end, it calls the
    ///     native function SendSAS (https://docs.microsoft.com/en-us/windows/win32/api/sas/nf-sas-sendsas).
    /// </para>
    /// <para>
    ///     This can (usually?) only be called  from a Windows service, so this event can be routed to one.
    ///     The desktop process will also try to call it.  I don't believe there are any side effects from
    ///     calling it multiple times, even if they both succeed.
    /// </para>
    /// </summary>
    /// <param name="session"></param>
    /// <param name="viewerConnectionId"></param>
    /// <returns></returns>
    Task InvokeCtrlAltDel(RemoteControlSession session, string viewerConnectionId);

    /// <summary>
    /// This is called when a new session is added to the <see cref="IRemoteControlSessionCache"/>.
    /// </summary>
    /// <param name="sessionInfo"></param>
    /// <returns></returns>
    Task NotifyDesktopSessionAdded(RemoteControlSession sessionInfo);

    /// <summary>
    /// This is called when all viewers have left a remote control session
    /// and the session is removed from the <see cref="IRemoteControlSessionCache"/>.
    /// </summary>
    /// <param name="sessionInfo"></param>
    /// <returns></returns>
    Task NotifyDesktopSessionRemoved(RemoteControlSession sessionInfo);

    /// <summary>
    /// This is called when a remote control session ends.  This event may occur
    /// multiple times per session if additional viewers are invited to the same session.
    /// </summary>
    /// <param name="sessionInfo"></param>
    /// <returns></returns>
    Task NotifyRemoteControlEnded(RemoteControlSession sessionInfo);

    /// <summary>
    /// This is called when a remote control session starts.  This event may occur
    /// multiple times per session if additional viewers are invited to the same session.
    /// </summary>
    /// <param name="sessionInfo"></param>
    /// <returns></returns>
    Task NotifyRemoteControlStarted(RemoteControlSession sessionInfo);

    /// <summary>
    /// This is called when the Windows session has changed for an active remote control session.
    /// </summary>
    /// <param name="sessionInfo"></param>
    /// <param name="reason">The type of change that's occurring.</param>
    /// <param name="currentSessionId">The current session ID of the remote control process.</param>
    /// <returns></returns>
    Task NotifySessionChanged(RemoteControlSession session, SessionSwitchReasonEx reason, int currentSessionId);

    /// <summary>
    /// This is called when the remote control session ends unexpectedly from the desktop
    /// side, and the viewer is expecting it to restart automatically.
    /// </summary>
    /// <param name="sessionInfo"></param>
    /// <param name="viewerList">
    ///    This is the list of viewer SignalR connection IDs.  These should be comma-delimited
    ///    and passed into the new remote control process with the --viewer param, and they will
    ///    be signaled to automatically reconnect when the new session is ready.
    /// </param>
    /// <returns></returns>
    Task RestartScreenCaster(RemoteControlSession session);
}
