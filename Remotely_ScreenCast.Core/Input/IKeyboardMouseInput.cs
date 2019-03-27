using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Core.Input
{
    public interface IKeyboardMouseInput
    {
        void SendKeyDown(string key);
        void SendKeyUp(string key);
        uint SendMouseMove(double percentX, double percentY);
        uint SendLeftMouseDown(double percentX, double percentY);
        uint SendLeftMouseUp(double percentX, double percentY);
        uint SendRightMouseDown(double percentX, double percentY);
        uint SendRightMouseUp(double percentX, double percentY);
        uint SendMouseWheel(int deltaY);
    }
}
