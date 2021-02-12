using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Services;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class BrandedViewModelBase : ReactiveViewModel
    {
        public BrandedViewModelBase()
        {
            var deviceInit = ServiceContainer.Instance?.GetRequiredService<IDeviceInitService>();

            var brandingInfo = deviceInit?.BrandingInfo ?? new Shared.Models.BrandingInfo();

            ProductName = "Remotely";

            if (!string.IsNullOrWhiteSpace(brandingInfo?.Product))
            {
                ProductName = brandingInfo.Product;
            }

            TitleBackgroundColor = new SolidColorBrush(Color.FromRgb(
                brandingInfo?.TitleBackgroundRed ?? 70,
                brandingInfo?.TitleBackgroundGreen ?? 70,
                brandingInfo?.TitleBackgroundBlue ?? 70));

            TitleForegroundColor = new SolidColorBrush(Color.FromRgb(
               brandingInfo?.TitleForegroundRed ?? 29,
               brandingInfo?.TitleForegroundGreen ?? 144,
               brandingInfo?.TitleForegroundBlue ?? 241));

            TitleButtonForegroundColor = new SolidColorBrush(Color.FromRgb(
               brandingInfo?.ButtonForegroundRed ?? 255,
               brandingInfo?.ButtonForegroundGreen ?? 255,
               brandingInfo?.ButtonForegroundBlue ?? 255));

            if (brandingInfo?.Icon?.Any() == true)
            {
                using var imageStream = new MemoryStream(brandingInfo.Icon);
                Icon = new Bitmap(imageStream);
            }
            else
            {
                using var imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Remotely.Desktop.Linux.Assets.Remotely_Icon.png");
                Icon = new Bitmap(imageStream);
            }

            WindowIcon = new WindowIcon(Icon);
        }

        public Bitmap Icon { get; set; }
        public string ProductName { get; set; }
        public SolidColorBrush TitleBackgroundColor { get; set; }

        public SolidColorBrush TitleButtonForegroundColor { get; set; }

        public SolidColorBrush TitleForegroundColor { get; set; }

        public WindowIcon WindowIcon { get; set; }
    }
}
