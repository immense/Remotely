namespace Remotely.Server.Models.Messages;

public class ShowLoaderMessage
{
    public ShowLoaderMessage(bool isShown, string statusMessage)
    {
        IsShown = isShown;
        StatusMessage = statusMessage;
    }

    public bool IsShown { get; }
    public string StatusMessage { get; }
}