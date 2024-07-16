using Remotely.Shared.Enums;

namespace Remotely.Desktop.Shared.Messages;

public class WindowsSessionSwitchedMessage
{
    public WindowsSessionSwitchedMessage(SessionSwitchReasonEx reason, int sessionId)
    {
        Reason = reason;
        SessionId = sessionId;
    }

    public SessionSwitchReasonEx Reason { get; }
    public int SessionId { get; }
}
