using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Installer.Win.Services
{
    public static class ProcessWrapper
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
