using Avalonia.Threading;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Core.ViewModels;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.ViewModels;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Services
{
    public class FileTransferServiceLinux : IFileTransferService
    {
        private static readonly SemaphoreSlim _writeLock = new(1,1);
        private static readonly ConcurrentDictionary<string, FileStream> _partialTransfers =
            new();
        private static readonly ConcurrentDictionary<string, FileTransferWindow> _fileTransferWindows =
            new();
        private static volatile bool _messageBoxPending;

        public string GetBaseDirectory()
        {
            var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (Directory.Exists(desktopDir))
            {
                return desktopDir;
            }

            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Remotely_Shared")).FullName;
        }

        public void OpenFileTransferWindow(Viewer viewer)
        {
            Dispatcher.UIThread.Post(() =>
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

        private async Task ShowTransferComplete()
        {
            // Prevent multiple dialogs from popping up.
            if (!_messageBoxPending)
            {
                _messageBoxPending = true;

                await MessageBox.Show($"File tranfer complete.  Files saved to directory:\n\n{GetBaseDirectory()}",
                    "Tranfer Complete",
                    MessageBoxType.OK);

                _messageBoxPending = false;
            }
        }
    }
}
