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
            throw new NotImplementedException();
        }

        public void SendKeyUp(string key)
        {
            throw new NotImplementedException();
        }

        public uint SendLeftMouseDown(double percentX, double percentY)
        {
            throw new NotImplementedException();
        }

        public uint SendLeftMouseUp(double percentX, double percentY)
        {
            throw new NotImplementedException();
        }

        public uint SendMouseMove(double percentX, double percentY)
        {
            throw new NotImplementedException();
        }

        public uint SendMouseWheel(int deltaY)
        {
            throw new NotImplementedException();
        }

        public uint SendRightMouseDown(double percentX, double percentY)
        {
            throw new NotImplementedException();
        }

        public uint SendRightMouseUp(double percentX, double percentY)
        {
            throw new NotImplementedException();
        }
    }
}
