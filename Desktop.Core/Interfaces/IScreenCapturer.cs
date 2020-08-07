using System;
using System.Collections.Generic;
using System.Drawing;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IScreenCapturer : IDisposable
    {
        event EventHandler<Rectangle> ScreenChanged;

        bool CaptureFullscreen { get; set; }
        Rectangle CurrentScreenBounds { get; }
        string SelectedScreen { get; }

        IEnumerable<string> GetDisplayNames();

        Bitmap GetNextFrame();

        int GetScreenCount();

        int GetSelectedScreenIndex();
        Rectangle GetVirtualScreenBounds();

        void Init();

        void SetSelectedScreen(string displayName);
    }
}
