using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.ScreenCast.Core.Interfaces
{
    public interface IClipboardService
    {
        void SetText(string clipboardText);
    }
}
