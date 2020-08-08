using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface ICursorIconWatcher
    {
        event EventHandler<CursorInfo> OnChange;

        CursorInfo GetCurrentCursor();
    }

}
