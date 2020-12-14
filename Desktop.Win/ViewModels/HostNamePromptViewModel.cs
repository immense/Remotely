using Remotely.Desktop.Core.ViewModels;

namespace Remotely.Desktop.Win.ViewModels
{
    public class HostNamePromptViewModel : ViewModelBase
    {
        private string host;

        public string Host
        {
            get => host;
            set
            {
                host = value;
                FirePropertyChanged("Host");
            }
        }
    }
}
