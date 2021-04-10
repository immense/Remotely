using Avalonia.Controls;
using Avalonia.Threading;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Linux.Views;

namespace Remotely.Desktop.Linux.Services
{
    public class SessionIndicatorLinux : ISessionIndicator
    {
        public void Show()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var indicatorWindow = new SessionIndicatorWindow();
                indicatorWindow.Show();
            });
        }
    }
}
