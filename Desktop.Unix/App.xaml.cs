using Avalonia;
using Avalonia.Markup.Xaml;

namespace Remotely.Desktop.Unix
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
