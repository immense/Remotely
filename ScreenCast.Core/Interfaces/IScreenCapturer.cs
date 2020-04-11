using System;
using System.Collections.Generic;
using System.Drawing;

namespace Remotely.ScreenCast.Core.Interfaces
{
    public interface IScreenCapturer : IDisposable
    {
        event EventHandler<Rectangle> ScreenChanged;

        bool CaptureFullscreen { get; set; }
        Bitmap CurrentFrame { get; set; }
        Rectangle CurrentScreenBounds { get; }
        Bitmap PreviousFrame { get; set; }
        string SelectedScreen { get; }

        IEnumerable<string> GetDisplayNames();

        void GetNextFrame();

        int GetScreenCount();

        int GetSelectedScreenIndex();
        Rectangle GetVirtualScreenBounds();

        void Init();

        void SetSelectedScreen(string displayName);
    }
}
