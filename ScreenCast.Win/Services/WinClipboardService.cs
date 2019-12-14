using Remotely.ScreenCast.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remotely.ScreenCast.Win.Services
{
    public class WinClipboardService : IClipboardService
    {
        public void SetText(string clipboardText)
        {
            var thread = new Thread(() =>
            {
                Clipboard.SetText(clipboardText);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
