using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCapture.Utilities;
using Remotely_ScreenCast;
using Remotely_ScreenCast.Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remotely_ScreenCapture.Sockets
{
    public class MessageHandlers
    {
        public static void ApplyConnectionHandlers(HubConnection hubConnection, 
            OutgoingMessages outgoingMessages, 
            ICapturer capturer)
        {
            hubConnection.Closed += (ex) =>
            {
                Logger.Write($"Error: {ex.Message}");
                Environment.Exit(1);
                return Task.CompletedTask;
            };

            hubConnection.On("GetScreenCast", (string viewerID, string requesterName) =>
            {
                outgoingMessages.SendScreenCount(
                   Screen.AllScreens.ToList().IndexOf(Screen.PrimaryScreen),
                   Screen.AllScreens.Length,
                   viewerID).Wait();

                outgoingMessages.SendScreenSize(capturer.CurrentScreenSize.Width, capturer.CurrentScreenSize.Height, viewerID).Wait();

                while (!Program.DisconnectRequested)
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
                            outgoingMessages.SendScreenCapture(img, viewerID).Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write($"Outer Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    }
                }
            });
        }
    }
}
