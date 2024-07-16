using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.ViewModels;

namespace Immense.RemoteControl.Desktop.Shared.Abstractions;

public interface IFileTransferService
{
    string GetBaseDirectory();

    Task ReceiveFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile);
    void OpenFileTransferWindow(IViewer viewer);
    Task UploadFile(FileUpload file, IViewer viewer, Action<double> progressUpdateCallback, CancellationToken cancelToken);
}
