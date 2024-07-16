using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Immense.RemoteControl.Desktop.UI.Controls.Dialogs;
using Immense.RemoteControl.Shared.Models;
using Remotely.Shared.Entities;
using System.Diagnostics;
using System.IO;

namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;

public class FakeBrandedViewModelBase : IBrandedViewModelBase
{
    private readonly BrandingInfo _brandingInfo;
    private Bitmap? _icon;

    public FakeBrandedViewModelBase()
    {
        _brandingInfo = new BrandingInfo();
        _icon = GetBitmapImageIcon(_brandingInfo);
    }
    public Bitmap? Icon
    {
        get => _icon;
        set => _icon = value;
    }
    public string ProductName { get; set; } = "Test Product";
    public WindowIcon? WindowIcon { get; set; }

    public Task ApplyBranding()
    {
        return Task.CompletedTask;
    }

    private Bitmap? GetBitmapImageIcon(BrandingInfo bi)
    {
        try
        {
            using var imageStream = typeof(Shared.Services.AppState)
                .Assembly
                .GetManifestResourceStream("Immense.RemoteControl.Desktop.Shared.Assets.DefaultIcon.png") ?? new MemoryStream();

            return new Bitmap(imageStream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
}
