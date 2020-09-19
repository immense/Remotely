using Remotely.Desktop.Core.Models;

namespace Remotely.Desktop.Core.Interfaces
{
    public interface IKeyboardMouseInput
    {
        void SendKeyDown(string key);
        void SendKeyUp(string key);
        void SendMouseMove(double percentX, double percentY, Viewer viewer);
        void SendLeftMouseDown(double percentX, double percentY, Viewer viewer);
        void SendLeftMouseUp(double percentX, double percentY, Viewer viewer);
        void SendRightMouseDown(double percentX, double percentY, Viewer viewer);
        void SendRightMouseUp(double percentX, double percentY, Viewer viewer);
        void SendMouseWheel(int deltaY);
        void SendText(string transferText);
        void ToggleBlockInput(bool toggleOn);
        void SetKeyStatesUp();
    }
}
