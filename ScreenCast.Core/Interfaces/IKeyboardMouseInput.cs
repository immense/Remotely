using Remotely.ScreenCast.Core.Models;

namespace Remotely.ScreenCast.Core.Interfaces
{
    public interface IKeyboardMouseInput
    {
        void SendKeyDown(string key, Viewer viewer);
        void SendKeyUp(string key, Viewer viewer);
        void SendMouseMove(double percentX, double percentY, Viewer viewer);
        void SendLeftMouseDown(double percentX, double percentY, Viewer viewer);
        void SendLeftMouseUp(double percentX, double percentY, Viewer viewer);
        void SendRightMouseDown(double percentX, double percentY, Viewer viewer);
        void SendRightMouseUp(double percentX, double percentY, Viewer viewer);
        void SendMouseWheel(int deltaY, Viewer viewer);
        void SendText(string transferText, Viewer viewer);
        void ToggleBlockInput(bool toggleOn);
        void SetKeyStatesUp();
    }
}
