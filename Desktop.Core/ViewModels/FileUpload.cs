using System.IO;

namespace Remotely.Desktop.Core.ViewModels
{
    public class FileUpload : ViewModelBase
    {
        private string _filePath;
        private double _percentProgress;

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
