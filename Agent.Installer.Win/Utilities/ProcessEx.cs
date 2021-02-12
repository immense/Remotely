using System.Diagnostics;

namespace Remotely.Agent.Installer.Win.Utilities
{
    public static class ProcessEx
    {
        public static Process StartHidden(string filePath, string arguments)
        {
            var psi = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                Arguments = arguments,
                FileName = filePath
            };
            return Process.Start(psi);
        }
    }
}
