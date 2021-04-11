using System.IO;
using System.Threading;

namespace Remotely.Desktop.Core.ViewModels
{
    public class FileUpload : ViewModelBase
    {
        private string _filePath;
        private double _percentProgress;

        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        public string DisplayName => Path.GetFileName(FilePath);

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                FirePropertyChanged();
            }
        }
        public double PercentProgress
        {
            get
            {
                return _percentProgress;
            }
            set
            {
                _percentProgress = value;
                FirePropertyChanged();
            }
        }
    }
}
