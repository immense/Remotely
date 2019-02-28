using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast.Capture;
using Remotely_ScreenCast.Utilities;
using Remotely_ScreenCast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;

namespace Remotely_ScreenCast.Sockets
{
    public class MessageHandlers
    {
        public static void ApplyConnectionHandlers(HubConnection hubConnection, OutgoingMessages outgoingMessages)
        {
            hubConnection.Closed += (ex) =>
            {
                Logger.Write($"Error: {ex.Message}");
                Environment.Exit(1);
                return Task.CompletedTask;
            };

            hubConnection.On("GetScreenCast", (string viewerID, string requesterName) =>
            {
                ScreenCaster.BeginScreenCasting(hubConnection, viewerID, requesterName, outgoingMessages);
            });

            hubConnection.On("KeyDown", (int keyCode, string viewerID) =>
            {
                var viewer = Program.Viewers[viewerID];
                if (viewer.HasControl)
                {
                    Win32Interop.SendKeyDown((User32.VirtualKeyShort)keyCode);
                }
            });

            hubConnection.On("KeyUp", (int keyCode, string viewerID) =>
            {
                var viewer = Program.Viewers[viewerID];
                if (viewer.HasControl)
                {
                    Win32Interop.SendKeyUp((User32.VirtualKeyShort)keyCode);
                }
            });

            hubConnection.On("KeyPress", (int keyCode, string viewerID) =>
            {
                var viewer = Program.Viewers[viewerID];
                if (viewer.HasControl)
                {
                    Win32Interop.SendKeyDown((User32.VirtualKeyShort)keyCode);
                    Win32Interop.SendKeyUp((User32.VirtualKeyShort)keyCode);
                }
            });

            hubConnection.On("MouseMove", (double percentX, double percentY, string viewerID) =>
            {
                var viewer = Program.Viewers[viewerID];
                if (viewer.HasControl)
                {
                    var mousePoint = ScreenCaster.GetAbsoluteScreenCoordinatesFromPercentages(percentX, percentY, viewer.Capturer);
                    Win32Interop.SendMouseMove(mousePoint.Item1, mousePoint.Item2);
                }
            });

            hubConnection.On("ViewerDisconnected", (string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.DisconnectRequested = true;
                    var success = false;
                    while (!success)
                    {
                        success = Program.Viewers.TryRemove(viewerID, out _);
                    }

                    // Close if no one is viewing.
                    if (Program.Viewers.Count == 0)
                    {
                        Environment.Exit(0);
                    }
                }
            });
            hubConnection.On("FrameSkip", (int delayTime, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.NextCaptureDelay = delayTime;
                }
            });
        }
    }
}
