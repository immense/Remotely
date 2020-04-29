using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace Remotely.ScreenCast.Linux.Services
{
    public class ClipboardServiceLinux : IClipboardService
    {
#pragma warning disable
        public event EventHandler<string> ClipboardTextChanged;
#pragma warning restore

        public void BeginWatching()
        {
            // Not implemented.
        }

        public void SetText(string clipboardText)
        {
            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, clipboardText);
            try
            {
                var psi = new ProcessStartInfo("bash", $"-c \"cat {tempPath} | xclip -i -selection clipboard\"")
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var proc = Process.Start(psi);
                proc.WaitForExit();

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                File.Delete(tempPath);
            }
        }
    }
}
