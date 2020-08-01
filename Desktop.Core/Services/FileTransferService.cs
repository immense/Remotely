using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Remotely.Shared.Win32;

namespace Remotely.Desktop.Core.Services
{
    public interface IFileTransferService
    {
        string GetBaseDirectory();

        Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile);
    }

    public class FileTransferService : IFileTransferService
    {
        private static readonly ConcurrentDictionary<string, FileStream> partialTransfers = new ConcurrentDictionary<string, FileStream>();

        private static readonly SemaphoreSlim writeLock = new SemaphoreSlim(1);
        private static volatile bool messageBoxPending;

        public string GetBaseDirectory()
        {
            string baseDir;
            var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (Directory.Exists(programDataPath))
            {
                baseDir = Directory.CreateDirectory(Path.Combine(programDataPath, "Remotely", "Shared")).FullName;
                SetFileOrFolderPermissions(baseDir);
            }
            else
            {
                // Use Temp dir if ProgramData doesn't exist (e.g. non-Windows OS).
                baseDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Remotely_Shared")).FullName;
            }
            return baseDir;
        }

        public async Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile)
        {
            try
            {
                await writeLock.WaitAsync();

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
                    SetFileOrFolderPermissions(filePath);
                    var fs = new FileStream(filePath, FileMode.OpenOrCreate);
                    partialTransfers.AddOrUpdate(messageId, fs, (k,v) => fs);
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
                writeLock.Release();
                if (endOfFile)
                {
                    await Task.Run(ShowTransferComplete);
                }
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
            if (!messageBoxPending)
            {
                if (EnvironmentHelper.IsWindows)
                {
                    messageBoxPending = true;
                    var result = Win32Interop.ShowMessageBox(User32.GetDesktopWindow(),
                                    "File transfer complete.  Show folder?",
                                    "Transfer Complete",
                                    User32.MessageBoxType.MB_YESNO | 
                                    User32.MessageBoxType.MB_ICONINFORMATION |
                                    User32.MessageBoxType.MB_TOPMOST |
                                    User32.MessageBoxType.MB_SYSTEMMODAL);

                    if (result == User32.MessageBoxResult.IDYES)
                    {
                        Process.Start("explorer.exe", GetBaseDirectory());
                    }
                    messageBoxPending = false;
                }
            }
        }
    }
}
