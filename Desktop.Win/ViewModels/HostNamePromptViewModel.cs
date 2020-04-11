namespace Remotely.Desktop.Win.ViewModels
{
    public class HostNamePromptViewModel : ViewModelBase
    {
        private string host;

        public static HostNamePromptViewModel Current { get; private set; }
        public HostNamePromptViewModel()
        {
            Current = this;
        }
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
