using Remotely.ScreenCast.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Input
{
    public interface IKeyboardMouseInput
    {
        void SendKeyDown(string key, Viewer viewer);
        void SendKeyUp(string key, Viewer viewer);
        uint SendMouseMove(double percentX, double percentY, Viewer viewer);
        uint SendLeftMouseDown(double percentX, double percentY, Viewer viewer);
        uint SendLeftMouseUp(double percentX, double percentY, Viewer viewer);
        uint SendRightMouseDown(double percentX, double percentY, Viewer viewer);
        uint SendRightMouseUp(double percentX, double percentY, Viewer viewer);
        uint SendMouseWheel(int deltaY, Viewer viewer);
    }
}
