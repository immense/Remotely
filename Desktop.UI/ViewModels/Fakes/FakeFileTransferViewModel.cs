using Immense.RemoteControl.Desktop.Shared.Reactive;
using Immense.RemoteControl.Desktop.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;

public class FakeFileTransferViewModel : FakeBrandedViewModelBase, IFileTransferWindowViewModel
{
    public ObservableCollection<FileUpload> FileUploads { get; } = new();

    public string ViewerConnectionId { get; set; } = string.Empty;
    public string ViewerName { get; set; } = string.Empty;

    public ICommand OpenFileUploadDialogCommand { get; } = new RelayCommand(() => { });

    public ICommand RemoveFileUploadCommand { get; } = new RelayCommand(() => { });

    public Task OpenFileUploadDialog()
    {
        return Task.CompletedTask;
    }

    public void RemoveFileUpload(FileUpload? fileUpload)
    {

    }

    public Task UploadFile(string filePath)
    {
        return Task.CompletedTask;
    }
}
