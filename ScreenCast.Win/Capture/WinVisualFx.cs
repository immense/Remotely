using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Win.Capture
{
    public class WinVisualFx
    {

        private static object PreviousSetting { get; set; }

        public static void RestoreSetting()
        {
            var vfxKey = Registry.CurrentUser.CreateSubKey(@"Software\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true);
            vfxKey?.SetValue("VisualFXSetting", PreviousSetting, RegistryValueKind.DWord);
        }

        public static void SetHighPerformance()
        {
            var vfxKey = Registry.CurrentUser.CreateSubKey(@"Software\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true);
            PreviousSetting = vfxKey?.GetValue("VisualFXSetting");
            vfxKey?.SetValue("VisualFXSetting", 2, RegistryValueKind.DWord);
        }
    }
}
