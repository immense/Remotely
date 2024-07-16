using Immense.RemoteControl.Shared.Models;

namespace Immense.RemoteControl.Desktop.Shared.Abstractions;

public interface ICursorIconWatcher
{
    [Obsolete("This should be replaced with a message published by IMessenger.")]
    event EventHandler<CursorInfo> OnChange;

    CursorInfo GetCurrentCursor();
}
