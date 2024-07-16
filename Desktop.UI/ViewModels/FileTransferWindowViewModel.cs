using Avalonia.Threading;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using Immense.RemoteControl.Desktop.Shared.Reactive;

namespace Immense.RemoteControl.Desktop.UI.ViewModels;

public interface IFileTransferWindowViewModel : IBrandedViewModelBase
{
    ObservableCollection<FileUpload> FileUploads { get; }
    ICommand OpenFileUploadDialogCommand { get; }
    ICommand RemoveFileUploadCommand { get; }
    string ViewerConnectionId { get; set; }
    string ViewerName { get; set; }

    void RemoveFileUpload(FileUpload? fileUpload);
    Task UploadFile(string filePath);
}

public class FileTransferWindowViewModel : BrandedViewModelBase, IFileTransferWindowViewModel
{
    private readonly IFileTransferService _fileTransferService;
    private readonly IViewer _viewer;

    public FileTransferWindowViewModel(
       IViewer viewer,
       IBrandingProvider brandingProvider,
       IUiDispatcher dispatcher,
       IFileTransferService fileTransferService,
       ILogger<FileTransferWindowViewModel> logger)
       : base(brandingProvider, dispatcher, logger)
    {
        _viewer = viewer;
        _fileTransferService = fileTransferService;
        ViewerName = viewer.Name;
        ViewerConnectionId = viewer.ViewerConnectionId;

        OpenFileUploadDialogCommand = new AsyncRelayCommand<FileTransferWindow>(OpenFileUploadDialog);
        RemoveFileUploadCommand = new RelayCommand<FileUpload>(RemoveFileUpload);
    }

    public ObservableCollection<FileUpload> FileUploads { get; } = new ObservableCollection<FileUpload>();

    public ICommand OpenFileUploadDialogCommand { get; }

    public ICommand RemoveFileUploadCommand { get; }

    public string ViewerConnectionId
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public string ViewerName
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public void RemoveFileUpload(FileUpload? fileUpload)
    {
        if (fileUpload is null)
        {
            return;
        }
        FileUploads.Remove(fileUpload);
        fileUpload.CancellationTokenSource.Cancel();
    }

    public async Task UploadFile(string filePath)
    {
        var fileUpload = new FileUpload()
        {
            FilePath = filePath
        };

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            FileUploads.Add(fileUpload);
        });

        await _fileTransferService.UploadFile(
            fileUpload,
            _viewer,
            async progress =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    fileUpload.PercentProgress = progress;
                });
            },
            fileUpload.CancellationTokenSource.Token);
    }

    private async Task OpenFileUploadDialog(FileTransferWindow? window)
    {
        if (window is null)
        {
            return;
        }

        var initialDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (!Directory.Exists(initialDir))
        {
            initialDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "RemoteControl")).FullName;
        }

        var startFolder = await window.StorageProvider.TryGetFolderFromPathAsync(new Uri(initialDir));
        var result = await window.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Upload File via Remotely",
            AllowMultiple = true,
            SuggestedStartLocation = startFolder
        });

        if (result?.Any() != true)
        {
            return;
        }
        foreach (var file in result)
        {
            await UploadFile($"{file.Path.LocalPath}");
        }
    }
}
