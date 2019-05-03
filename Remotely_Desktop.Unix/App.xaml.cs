using Avalonia;
using Avalonia.Markup.Xaml;

namespace Remotely_Desktop.Unix
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
