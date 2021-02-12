using System.Windows;

namespace Remotely.Agent.Installer.Win.Utilities
{
    public static class MessageBoxEx
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
