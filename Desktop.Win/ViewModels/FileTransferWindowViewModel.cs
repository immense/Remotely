using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Core.ViewModels;
using Remotely.Desktop.Win.Services;
using Remotely.Shared.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Remotely.Desktop.Win.ViewModels
{
    public class FileTransferWindowViewModel : BrandedViewModelBase
    {
        private readonly IFileTransferService _fileTransferService;
        private readonly Viewer _viewer;
        private string _viewerConnectionId;
        private string _viewerName;

        public FileTransferWindowViewModel() { }

        public FileTransferWindowViewModel(
            Viewer viewer,
            IFileTransferService fileTransferService)
        {
            _fileTransferService = fileTransferService;
            _viewer = viewer;
            ViewerName = viewer.Name;
            ViewerConnectionId = viewer.ViewerConnectionID;
        }

        public ObservableCollection<FileUpload> FileUploads { get; } = new ObservableCollection<FileUpload>();

        public ICommand OpenFileUploadDialog => new Executor(async (param) =>
        {
            // Change initial directory so it doesn't open in %userprofile% path
            // for SYSTEM account.
            var rootDir = Path.GetPathRoot(Environment.SystemDirectory);
            var userDir = Path.Combine(rootDir,
                "Users",
                Win32Interop.GetUsernameFromSessionId((uint)Process.GetCurrentProcess().SessionId));

            var ofd = new OpenFileDialog()
            {
                Title = "Upload File via Remotely",
                Multiselect = true,
                CheckFileExists = true,
                InitialDirectory = Directory.Exists(userDir) ? userDir : rootDir
            };

            try
            {
                // The OpenFileDialog throws an error if SYSTEM doesn't have a Desktop folder.
                var desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
                Directory.CreateDirectory(desktop);
            }
            catch { }

            var result = ofd.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            foreach (var file in ofd.FileNames)
            {
                if (File.Exists(file))
                {
                    await UploadFile(file);
                }
            }
        });

        public string ViewerConnectionId
        {
            get
            {
                return _viewerConnectionId;
            }
            set
            {
                _viewerConnectionId = value;
                FirePropertyChanged();
            }
        }

        public string ViewerName
        {
            get
            {
                return _viewerName;
            }
            set
            {
                _viewerName = value;
                FirePropertyChanged();
            }
        }

        public async Task UploadFile(string filePath)
        {
            var fileUpload = new FileUpload()
            {
                FilePath = filePath
            };

            App.Current.Dispatcher.Invoke(() =>
            {
                FileUploads.Add(fileUpload);
            });

            await _fileTransferService.UploadFile(fileUpload, _viewer, fileUpload.CancellationTokenSource.Token, (double progress) =>
            {
                App.Current.Dispatcher.Invoke(() => fileUpload.PercentProgress = progress);
            });
        }

        public ICommand RemoveFileUpload => new Executor((param) =>
        {
            if (param is FileUpload fileUpload)
            {
                FileUploads.Remove(fileUpload);
                fileUpload.CancellationTokenSource.Cancel();
            }
        });
    }
}