using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Installer.Win.Models
{
    public class BrandingInfo
    {
        public string Product { get; set; } = "Remotely";

        public string Icon { get; set; }

        public byte TitleForegroundRed { get; set; } = 29;

        public byte TitleForegroundGreen { get; set; } = 144;

        public byte TitleForegroundBlue { get; set; } = 241;

        public byte TitleBackgroundRed { get; set; } = 70;

        public byte TitleBackgroundGreen { get; set; } = 70;

        public byte TitleBackgroundBlue { get; set; } = 70;

        public byte ButtonForegroundRed { get; set; } = 255;

        public byte ButtonForegroundGreen { get; set; } = 255;

        public byte ButtonForegroundBlue { get; set; } = 255;
    }
}
