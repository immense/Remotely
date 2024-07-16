using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Immense.RemoteControl.Desktop.UI.Services;

// Normally, I'd use a view model locator.  But enough view models require a factory pattern
// that I thought it more consistent to put them all here.
public interface IViewModelFactory
{
    IChatWindowViewModel CreateChatWindowViewModel(string organizationName, StreamWriter streamWriter);
    IFileTransferWindowViewModel CreateFileTransferWindowViewModel(IViewer viewer);
    IHostNamePromptViewModel CreateHostNamePromptViewModel();
    IPromptForAccessWindowViewModel CreatePromptForAccessViewModel(string requesterName, string organizationName);
}

internal class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IChatWindowViewModel CreateChatWindowViewModel(string organizationName, StreamWriter streamWriter)
    {
        var branding = _serviceProvider.GetRequiredService<IBrandingProvider>();
        var dispatcher = _serviceProvider.GetRequiredService<IUiDispatcher>();
        var logger = _serviceProvider.GetRequiredService<ILogger<ChatWindowViewModel>>();
        return new ChatWindowViewModel(streamWriter, organizationName, branding, dispatcher, logger);
    }

    public IFileTransferWindowViewModel CreateFileTransferWindowViewModel(IViewer viewer)
    {
        var brandingProvider = _serviceProvider.GetRequiredService<IBrandingProvider>();
        var dispatcher = _serviceProvider.GetRequiredService<IUiDispatcher>();
        var logger = _serviceProvider.GetRequiredService<ILogger<FileTransferWindowViewModel>>();
        var fileTransfer = _serviceProvider.GetRequiredService<IFileTransferService>();
        return new FileTransferWindowViewModel(viewer, brandingProvider, dispatcher, fileTransfer, logger);
    }

    public IPromptForAccessWindowViewModel CreatePromptForAccessViewModel(string requesterName, string organizationName)
    {
        var brandingProvider = _serviceProvider.GetRequiredService<IBrandingProvider>();
        var dispatcher = _serviceProvider.GetRequiredService<IUiDispatcher>();
        var logger = _serviceProvider.GetRequiredService<ILogger<PromptForAccessWindowViewModel>>();
        return new PromptForAccessWindowViewModel(requesterName, organizationName, brandingProvider, dispatcher, logger);
    }

    public IHostNamePromptViewModel CreateHostNamePromptViewModel()
    {
        var brandingProvider = _serviceProvider.GetRequiredService<IBrandingProvider>();
        var dispatcher = _serviceProvider.GetRequiredService<IUiDispatcher>();
        var logger = _serviceProvider.GetRequiredService<ILogger<HostNamePromptViewModel>>();
        return new HostNamePromptViewModel(brandingProvider, dispatcher, logger);
    }
}
