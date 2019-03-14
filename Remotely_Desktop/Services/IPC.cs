using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely_Desktop.Services
{
    public class IPC
    {
        internal void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }

        internal void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            MessageBox.Show(e.Data, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
