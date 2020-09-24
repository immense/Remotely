using Avalonia;
using Avalonia.Markup.Xaml;

namespace Remotely.Desktop.Linux
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            //{
            //    var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            //    if (conductor.Mode == Core.Enums.AppMode.Normal)
            //    {
            //        desktop.MainWindow = new MainWindow
            //        {
            //            DataContext = new MainWindowViewModel(),
            //        };
            //    }
            //}

            base.OnFrameworkInitializationCompleted();
        }
    }
}
