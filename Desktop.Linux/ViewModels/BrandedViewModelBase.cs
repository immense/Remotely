using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Services;
using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class BrandedViewModelBase : ReactiveViewModel
    {
        public BrandedViewModelBase()
        {
            DeviceInitService = ServiceContainer.Instance?.GetRequiredService<IDeviceInitService>();

            ApplyBranding();
        }

        public Bitmap Icon { get; set; }
        public string ProductName { get; set; }
        public SolidColorBrush TitleBackgroundColor { get; set; }
        public SolidColorBrush TitleButtonForegroundColor { get; set; }
        public SolidColorBrush TitleForegroundColor { get; set; }
        public WindowIcon WindowIcon { get; set; }

        protected IDeviceInitService DeviceInitService { get; }

        public void ApplyBranding()
        {
            try
            {
                var brandingInfo = DeviceInitService?.BrandingInfo ?? new Shared.Models.BrandingInfo();

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

                this.RaisePropertyChanged(nameof(ProductName));
                this.RaisePropertyChanged(nameof(TitleBackgroundColor));
                this.RaisePropertyChanged(nameof(TitleForegroundColor));
                this.RaisePropertyChanged(nameof(TitleButtonForegroundColor));
                this.RaisePropertyChanged(nameof(WindowIcon));
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error applying branding.");
            }
        }
    }
}
