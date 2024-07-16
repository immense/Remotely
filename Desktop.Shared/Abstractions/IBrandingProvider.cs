using Remotely.Shared.Models;
using Remotely.Shared.Entities;

namespace Remotely.Desktop.Shared.Abstractions;

public interface IBrandingProvider
{
    BrandingInfo CurrentBranding { get; }
    Task Initialize();
    void SetBrandingInfo(BrandingInfo brandingInfo);
}
