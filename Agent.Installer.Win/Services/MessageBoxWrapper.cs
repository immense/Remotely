using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely.Agent.Installer.Win.Services
{
    public class MessageBoxWrapper
    {
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            if (!CommandLineParser.CommandLineArgs.ContainsKey("quiet"))
            {
                return MessageBox.Show(message, caption, messageBoxButton, messageBoxImage);
            }
            return MessageBoxResult.None;
        }    
    }
}
