using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Core.ViewModels;
using Remotely.Desktop.Win.ViewModels;
using Remotely.Desktop.Win.Views;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely.Desktop.Win.Services
{
    public class FileTransferServiceWin : IFileTransferService
    {
        private static readonly ConcurrentDictionary<string, FileStream> _partialTransfers =
            new();

        private static readonly ConcurrentDictionary<string, FileTransferWindow> _fileTransferWindows =
            new();

        private static readonly SemaphoreSlim _writeLock = new(1,1);
        private static MessageBoxResult? _result;

        public string GetBaseDirectory()
        {
            var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Directory.CreateDirectory(Path.Combine(programDataPath, "Remotely", "Shared")).FullName;
        }

        public void OpenFileTransferWindow(Viewer viewer)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (_fileTransferWindows.TryGetValue(viewer.ViewerConnectionID, out var window))
                {
                    window.Activate();
                }
                else
                {
                    window = new FileTransferWindow
                    {
                        DataContext = new FileTransferWindowViewModel(viewer, this)
                    };
                    window.Closed += (sender, arg) =>
                    {
                        _fileTransferWindows.Remove(viewer.ViewerConnectionID, out _);
                    };
                    _fileTransferWindows.AddOrUpdate(viewer.ViewerConnectionID, window, (k, v) => window);
                    window.Show();
                }
            });
        }

        public async Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile)
        {
            try
            {
                await _writeLock.WaitAsync();

                var baseDir = GetBaseDirectory();

                SetFileOrFolderPermissions(baseDir);

                if (startOfFile)
                {
                    var filePath = Path.Combine(baseDir, fileName);

                    if (File.Exists(filePath))
                    {
                        var count = 0;
                        var ext = Path.GetExtension(fileName);
                        var fileWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        while (File.Exists(filePath))
                        {
                            filePath = Path.Combine(baseDir, $"{fileWithoutExt}-{count}{ext}");
                            count++;
                        }
                    }

                    File.Create(filePath).Close();
                    SetFileOrFolderPermissions(filePath);
                    var fs = new FileStream(filePath, FileMode.OpenOrCreate);
                    _partialTransfers.AddOrUpdate(messageId, fs, (k, v) => fs);
                }

                var fileStream = _partialTransfers[messageId];

                if (buffer?.Length > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);

                }

                if (endOfFile)
                {
                    fileStream.Close();
                    _partialTransfers.Remove(messageId, out _);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                _writeLock.Release();
                if (endOfFile)
                {
                    await Task.Run(ShowTransferComplete);
                }
            }
        }

        public async Task UploadFile(FileUpload fileUpload, Viewer viewer, CancellationToken cancelToken, Action<double> progressUpdateCallback)
        {
            try
            {
                await viewer.SendFile(fileUpload, cancelToken, progressUpdateCallback);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void SetFileOrFolderPermissions(string path)
        {
            FileSystemSecurity ds;

            var aclSections = AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner;
            if (File.Exists(path))
            {
                ds = new FileSecurity(path, aclSections);
            }
            else if (Directory.Exists(path))
            {
                ds = new DirectorySecurity(path, aclSections);
            }
            else
            {
                return;
            }

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var account = (NTAccount)sid.Translate(typeof(NTAccount));

            var accessAlreadySet = false;

            foreach (FileSystemAccessRule rule in ds.GetAccessRules(true, true, typeof(NTAccount)))
            {
                if (rule.IdentityReference == account &&
                    rule.FileSystemRights.HasFlag(FileSystemRights.Modify) &&
                    rule.AccessControlType == AccessControlType.Allow)
                {
                    accessAlreadySet = true;
                    break;
                }
            }

            if (!accessAlreadySet)
            {
                ds.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.Modify, AccessControlType.Allow));
                if (File.Exists(path))
                {
                    new FileInfo(path).SetAccessControl((FileSecurity)ds);
                }
                else if (Directory.Exists(path))
                {
                    new DirectoryInfo(path).SetAccessControl((DirectorySecurity)ds);
                }
            }
        }

        private void ShowTransferComplete()
        {
            // Prevent multiple dialogs from popping up.
            if (_result is null)
            {
                _result = MessageBox.Show("File transfer complete.  Show folder?",
                    "Transfer Complete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.ServiceNotification);

                if (_result == MessageBoxResult.Yes)
                {
                    Process.Start("explorer.exe", GetBaseDirectory());
                }

                _result = null;
            }
        }
    }
}
