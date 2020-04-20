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

namespace Remotely.ScreenCast.Core.Services
{
    public interface IFileDownloadService
    {
        string GetBaseDirectory();

        Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile);
    }

    public class FileDownloadService : IFileDownloadService
    {
        private static readonly ConcurrentDictionary<string, FileStream> _partialDownloads = new ConcurrentDictionary<string, FileStream>();

        private static readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1);

        private ILogger<FileDownloadService> _logger;

        public FileDownloadService(ILogger<FileDownloadService> logger)
        {
            _logger = logger;
        }

        public string GetBaseDirectory()
        {
            string baseDir;
            var programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (Directory.Exists(programDataPath))
            {
                baseDir = Directory.CreateDirectory(Path.Combine(programDataPath, "Remotely", "Shared")).FullName;
                SetFolderPermissions(baseDir);
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

                    var fs = new FileStream(filePath, FileMode.OpenOrCreate);
                    _partialDownloads.AddOrUpdate(messageId, fs, (k,v) => fs);
                }

                var fileStream = _partialDownloads[messageId];

                if (buffer?.Length > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);

                }

                if (endOfFile)
                {
                    fileStream.Close();
                    _partialDownloads.Remove(messageId, out _);
                    if (EnvironmentHelper.IsWindows)
                    {
                        Process.Start("explorer.exe", baseDir);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving file.");
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private void SetFolderPermissions(string baseDir)
        {
            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var account = (NTAccount)sid.Translate(typeof(NTAccount));
            var aclSections = AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner;
            var ds = new DirectorySecurity(baseDir, aclSections);

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
                new DirectoryInfo(baseDir).SetAccessControl(ds);
            }
        }
    }
}
