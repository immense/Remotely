using Immense.RemoteControl.Shared.Models;
using Remotely.Shared.Entities;

namespace Immense.RemoteControl.Desktop.Shared.Abstractions;

public interface IBrandingProvider
{
    BrandingInfo CurrentBranding { get; }
    Task Initialize();
    void SetBrandingInfo(BrandingInfo brandingInfo);
}
