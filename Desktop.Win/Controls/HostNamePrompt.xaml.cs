using Remotely.Desktop.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
