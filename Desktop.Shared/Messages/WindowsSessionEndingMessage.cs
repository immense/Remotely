using Immense.RemoteControl.Shared.Enums;

namespace Immense.RemoteControl.Desktop.Shared.Messages;

public class WindowsSessionEndingMessage
{
    public WindowsSessionEndingMessage(SessionEndReasonsEx reason)
    {
        Reason = reason;
    }

    public SessionEndReasonsEx Reason { get; }
}
