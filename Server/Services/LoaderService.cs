using Bitbound.SimpleMessenger;
using Remotely.Server.Hubs;
using Remotely.Server.Models.Messages;

namespace Remotely.Server.Services;

public interface ILoaderService
{
    Task<IDisposable> ShowLoader(string statusMessage);
    void HideLoader();
}

public class LoaderService(IMessenger _messenger, ICircuitConnection _circuitConnection) : ILoaderService
{
    public async Task<IDisposable> ShowLoader(string statusMessage)
    {
        await _messenger.Send(new ShowLoaderMessage(true, statusMessage), _circuitConnection.ConnectionId);
        return new CallbackDisposable(HideLoader);
    }

    public void HideLoader()
    {
        _messenger.Send(new ShowLoaderMessage(false, string.Empty), _circuitConnection.ConnectionId);
    }
}