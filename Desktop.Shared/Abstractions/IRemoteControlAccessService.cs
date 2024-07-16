using Immense.RemoteControl.Shared.Enums;

namespace Immense.RemoteControl.Desktop.Shared.Abstractions;

public interface IRemoteControlAccessService
{
    bool IsPromptOpen { get; }

    Task<PromptForAccessResult> PromptForAccess(string requesterName, string organizationName);
}
