using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Core.Capture
{
    public interface ICapturer : IDisposable
    {
        bool CaptureFullscreen { get; set; }
        Bitmap CurrentFrame { get; set; }
        Rectangle CurrentScreenBounds { get; }
        Bitmap PreviousFrame { get; set; }
        EventHandler<Rectangle> ScreenChanged { get; set; }
        int SelectedScreen { get; set; }
        int GetScreenCount();
        double GetVirtualScreenHeight();
        double GetVirtualScreenWidth();

        void Capture();
        void Init();
    }
}
