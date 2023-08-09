using Immense.RemoteControl.Shared.Primitives;
using Immense.SimpleMessenger;
using Remotely.Server.Models.Messages;
using System;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface ILoaderService
{
    Task<IDisposable> ShowLoader(string statusMessage);
    void HideLoader();
}

public class LoaderService : ILoaderService
{
    private readonly IMessenger _messenger;

    public LoaderService(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public async Task<IDisposable> ShowLoader(string statusMessage)
    {
        await _messenger.Send(new ShowLoaderMessage(true, statusMessage));
        return new CallbackDisposable(HideLoader);
    }

    public void HideLoader()
    {
        _messenger.Send(new ShowLoaderMessage(false, string.Empty));
    }
}