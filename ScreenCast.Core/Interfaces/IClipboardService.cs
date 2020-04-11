using System;

namespace Remotely.ScreenCast.Core.Interfaces
{
    public interface IClipboardService
    {
        event EventHandler<string> ClipboardTextChanged;

        void BeginWatching();

        void SetText(string clipboardText);
    }
}
