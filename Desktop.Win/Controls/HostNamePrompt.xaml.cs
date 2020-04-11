using System.Windows;

namespace Remotely.Desktop.Win.Controls
{
    /// <summary>
    /// Interaction logic for HostNamePrompt.xaml
    /// </summary>
    public partial class HostNamePrompt : Window
    {
        public HostNamePrompt()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
