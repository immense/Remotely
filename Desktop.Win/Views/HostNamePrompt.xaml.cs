using Remotely.Desktop.Win.ViewModels;
using System.Windows;

namespace Remotely.Desktop.Win.Views
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

        public HostNamePromptViewModel ViewModel => DataContext as HostNamePromptViewModel;

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
