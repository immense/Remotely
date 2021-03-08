using Remotely.Shared.Utilities;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

namespace Remotely.Desktop.Win.Models
{
    public class DirectXOutput : IDisposable
    {
        public DirectXOutput(Adapter1 adapter,
            SharpDX.Direct3D11.Device device,
            OutputDuplication outputDuplication,
            Texture2D texture2D)
        {
            Adapter = adapter;
            Device = device;
            OutputDuplication = outputDuplication;
            Texture2D = texture2D;
        }

        public Adapter1 Adapter { get; }
        public SharpDX.Direct3D11.Device Device { get; }
        public OutputDuplication OutputDuplication { get; }
        public Texture2D Texture2D { get; }

        public void Dispose()
        {
            Disposer.TryDisposeAll(new IDisposable[]
            {
                Adapter,
                Device,
                OutputDuplication,
                Texture2D
            });
            GC.SuppressFinalize(this);
        }
    }
}
