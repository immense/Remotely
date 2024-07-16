using Immense.RemoteControl.Desktop.Shared.Enums;
using Immense.RemoteControl.Desktop.Shared.Services;

namespace Immense.RemoteControl.Desktop.Shared.Abstractions;

public interface IKeyboardMouseInput
{
    void Init();
    void SendKeyDown(string key);
    void SendKeyUp(string key);
    void SendMouseMove(double percentX, double percentY, IViewer viewer);
    void SendMouseWheel(int deltaY);
    void SendText(string transferText);
    void ToggleBlockInput(bool toggleOn);
    void SetKeyStatesUp();
    void SendMouseButtonAction(int button, ButtonAction buttonAction, double percentX, double percentY, IViewer viewer);
}
