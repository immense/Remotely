using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCapture.Capture;
using Remotely_ScreenCapture.Utilities;
using Remotely_ScreenCast;
using Remotely_ScreenCast.Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;

namespace Remotely_ScreenCapture.Sockets
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
                BeginScreenCasting(hubConnection, viewerID, requesterName, outgoingMessages);
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

            hubConnection.On("MouseMove", (decimal percentX, decimal percentY, string viewerID) =>
            {
                var viewer = Program.Viewers[viewerID];
                if (viewer.HasControl)
                {
                    var mousePoint = viewer.Capturer.GetAbsoluteScreenCoordinatesFromPercentages(percentX, percentY);
                    Win32Interop.SendMouseMove(mousePoint.X, mousePoint.Y);
                }
            });

            hubConnection.On("ViewerDisconnected", (string viewerID) =>
            {
                if (Program.Viewers.ContainsKey(viewerID))
                {
                    var viewer = Program.Viewers[viewerID];
                    viewer.DisconnectRequested = true;
                    var success = false;
                    while (!success)
                    {
                        success = Program.Viewers.TryRemove(viewerID, out _);
                    }
                }
            });
        }

        private static async void BeginScreenCasting(HubConnection hubConnection, 
                                                        string viewerID, 
                                                        string requesterName,
                                                        OutgoingMessages outgoingMessages)
        {
            ICapturer capturer;
            CaptureMode captureMode;

            try
            {
                capturer = new DXCapture();
                captureMode = CaptureMode.DirectX;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                capturer = new BitBltCapture();
                captureMode = CaptureMode.BitBtl;
            }

            Logger.Write($"Starting screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}. Capture Mode: {captureMode.ToString()}.");

            var viewer = new Models.Viewer()
            {
                Capturer = capturer,
                CurrentScreenIndex = 0,
                DisconnectRequested = false,
                Name = requesterName,
                ViewerConnectionID = viewerID,
                HasControl = Program.Mode == "unattended"
            };


            var success = false;
            while (!success)
            {
                success = Program.Viewers.TryAdd(viewerID, viewer);
            }

            await outgoingMessages.SendScreenCount(
                   Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen),
                   Screen.AllScreens.Length,
                   viewerID);

            await outgoingMessages.SendScreenSize(capturer.CurrentScreenBounds.Width, capturer.CurrentScreenBounds.Height, viewerID);

            capturer.ScreenChanged += async (sender, size) =>
            {
                await outgoingMessages.SendScreenSize(size.Width, size.Height, viewerID);
            };

            while (!viewer.DisconnectRequested)
            {
                try
                {
                    capturer.Capture();
                    var newImage = ImageDiff.GetImageDiff(capturer.CurrentFrame, capturer.PreviousFrame, capturer.CaptureFullscreen);
                    var img = ImageDiff.EncodeBitmap(newImage);
                    if (capturer.CaptureFullscreen)
                    {
                        capturer.CaptureFullscreen = false;
                    }
                    if (img?.Length > 0)
                    {
                        await outgoingMessages.SendScreenCapture(img, viewerID);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write($"Outer Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }

            success = false;
            while (!success)
            {
                Program.Viewers.TryRemove(viewerID, out _);
            }
            Logger.Write($"Ended screen cast.  Requester: {requesterName}. Viewer ID: {viewerID}.");
        }
    }
}
