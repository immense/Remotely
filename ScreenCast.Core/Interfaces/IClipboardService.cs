using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.ScreenCast.Core.Interfaces
{
    public interface IClipboardService
    {
        event EventHandler<string> ClipboardTextChanged;

        void BeginWatching();

        void SetText(string clipboardText);
    }
}
