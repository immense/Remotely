using System;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IClipboardService
    {
        event EventHandler<string> ClipboardTextChanged;

        void BeginWatching();

        void SetText(string clipboardText);
    }
}
