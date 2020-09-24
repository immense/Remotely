using System;

namespace Remotely.Server.Models
{
    public class RemoteControlFrame
    {

        public RemoteControlFrame(byte[] frameBytes,
            int left,
            int top,
            int screenWidth,
            int screenHeight,
            bool endOfFrame,
            string viewerID,
            string machineName,
            DateTimeOffset startTime)
        {
            FrameBytes = frameBytes;
            Left = left;
            Top = top;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            EndOfFrame = endOfFrame;
            ViewerID = viewerID;
            MachineName = machineName;
            StartTime = startTime;
        }
        public bool EndOfFrame { get; private set; }
        public byte[] FrameBytes { get; private set; }
        public int Left { get; private set; }
        public string MachineName { get; private set; }
        public int ScreenHeight { get; private set; }
        public int ScreenWidth { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public int Top { get; private set; }
        public string ViewerID { get; private set; }
    }
}
