using Remotely_ScreenCast.Core.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely_ScreenCast.Linux.Input
{
    public class X11Input : IKeyboardMouseInput
    {
        public void SendKeyDown(string key)
        {
            
        }

        public void SendKeyUp(string key)
        {
            
        }

        public uint SendLeftMouseDown(double percentX, double percentY)
        {
            return 0;
        }

        public uint SendLeftMouseUp(double percentX, double percentY)
        {
            return 0;
        }

        public uint SendMouseMove(double percentX, double percentY)
        {
            return 0;
        }

        public uint SendMouseWheel(int deltaY)
        {
            return 0;
        }

        public uint SendRightMouseDown(double percentX, double percentY)
        {
            return 0;
        }

        public uint SendRightMouseUp(double percentX, double percentY)
        {
            return 0;
        }
    }
}
