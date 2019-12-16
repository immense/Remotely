using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Remotely.ScreenCast.Linux.Services
{
    public class LinuxClipboardService : IClipboardService
    {
        public event EventHandler<string> ClipboardTextChanged;

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
