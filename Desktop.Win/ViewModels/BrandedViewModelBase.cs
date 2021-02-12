using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Core.ViewModels;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Remotely.Desktop.Win.ViewModels
{
    public class BrandedViewModelBase : ViewModelBase
    {
        public BrandedViewModelBase()
        {
            var deviceInit = ServiceContainer.Instance?.GetRequiredService<IDeviceInitService>();

            var brandingInfo = deviceInit?.BrandingInfo ?? new BrandingInfo();

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

            Icon = GetBitmapImageIcon(brandingInfo);
        }
        public BitmapImage Icon { get; set; }

        public string ProductName { get; set; }

        public SolidColorBrush TitleBackgroundColor { get; set; }

        public SolidColorBrush TitleButtonForegroundColor { get; set; }

        public SolidColorBrush TitleForegroundColor { get; set; }


        private BitmapImage GetBitmapImageIcon(BrandingInfo bi)
        {
            Stream imageStream;
            if (bi.Icon?.Any() == true)
            {
                imageStream = new MemoryStream(bi.Icon);
            }
            else
            {
                imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Remotely.Desktop.Win.Assets.Remotely_Icon.png");
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = imageStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            imageStream.Close();

            return bitmap;
        }
    }

}
