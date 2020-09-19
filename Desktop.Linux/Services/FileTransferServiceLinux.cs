using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Core.Interfaces;
using Avalonia.Controls;
using Remotely.Desktop.Core.Models;
using Remotely.Desktop.Core.ViewModels;

namespace Remotely.Desktop.Linux.Services
{
    public class FileTransferServiceLinux : IFileTransferService
    {
        private static readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1);
        private static readonly ConcurrentDictionary<string, FileStream> partialTransfers = new ConcurrentDictionary<string, FileStream>();
        private static volatile bool _messageBoxPending;

        public string GetBaseDirectory()
        {
            // TODO: Is this working on Linux?
            var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (Directory.Exists(desktopDir))
            {
                return desktopDir;
            }

            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Remotely_Shared")).FullName;
        }

        public void OpenFileTransferWindow(Viewer viewer)
        {
            // TODO: Create file transfer window.
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
                    partialTransfers.AddOrUpdate(messageId, fs, (k, v) => fs);
                }

                var fileStream = partialTransfers[messageId];

                if (buffer?.Length > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);

                }

                if (endOfFile)
                {
                    fileStream.Close();
                    partialTransfers.Remove(messageId, out _);
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

        public Task UploadFile(FileUpload file, Viewer viewer)
        {
            throw new NotImplementedException();
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
