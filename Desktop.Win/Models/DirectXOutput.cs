using Remotely.Shared.Helpers;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Drawing;

namespace Remotely.Desktop.Win.Models;

public class DirectXOutput : IDisposable
{
    public DirectXOutput(Adapter1 adapter,
        SharpDX.Direct3D11.Device device,
        OutputDuplication outputDuplication,
        Texture2D texture2D,
        DisplayModeRotation rotation)
    {
        Adapter = adapter;
        Device = device;
        OutputDuplication = outputDuplication;
        Texture2D = texture2D;
        Rotation = rotation;
        Bounds = new Rectangle(0, 0, texture2D.Description.Width, texture2D.Description.Height);
    }

    public Adapter1 Adapter { get; }
    public Rectangle Bounds { get; set; }
    public SharpDX.Direct3D11.Device Device { get; }
    public OutputDuplication OutputDuplication { get; }
    public DisplayModeRotation Rotation { get; }
    public Texture2D Texture2D { get; }
    public void Dispose()
    {
        OutputDuplication.ReleaseFrame();
        Disposer.TryDisposeAll(OutputDuplication, Texture2D, Adapter, Device);
        GC.SuppressFinalize(this);
    }
}
