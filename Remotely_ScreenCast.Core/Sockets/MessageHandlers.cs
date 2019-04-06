using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast.Core.Capture;
using Remotely_ScreenCast.Core.Utilities;
using Remotely_ScreenCast.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using Remotely_ScreenCast.Core.Models;
using Remotely_ScreenCast.Core.Input;
using Remotely_Library.Models;

namespace Remotely_ScreenCast.Core.Sockets
{
    public class MessageHandlers
    {
        public static void ApplyConnectionHandlers(HubConnection hubConnection, Conductor conductor, IKeyboardMouseInput keyboardMouse)
        {
            hubConnection.Closed += (ex) =>
            {
                Logger.Write($"Connection closed.  Error: {ex.Message}");
                Environment.Exit(1);
                return Task.CompletedTask;
            };

            hubConnection.On("GetScreenCast", (string viewerID, string requesterName) =>
            {
                try
                {
                    conductor.InvokeScreenCastInitiated(new ScreenCastRequest() { ViewerID = viewerID, RequesterName = requesterName });
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            hubConnection.On("RequestScreenCast", (string viewerID, string requesterName) =>
            {
                conductor.InvokeScreenCastRequested(new ScreenCastRequest() { ViewerID = viewerID, RequesterName = requesterName });
            });

            hubConnection.On("KeyDown", (string key, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    keyboardMouse.SendKeyDown(key, viewer);
                }
            });

            hubConnection.On("KeyUp", (string key, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    keyboardMouse.SendKeyUp(key, viewer);
                }
            });

            hubConnection.On("KeyPress", async (string key, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    keyboardMouse.SendKeyDown(key, viewer);
                    await Task.Delay(1);
                    keyboardMouse.SendKeyUp(key, viewer);
                }
            });

            hubConnection.On("MouseMove", (double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var xyPercents = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    keyboardMouse.SendMouseMove(xyPercents.Item1, xyPercents.Item2, viewer);
                }
            });

            hubConnection.On("MouseDown", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var xyPercents = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    if (button == 0)
                    {
                        keyboardMouse.SendLeftMouseDown(xyPercents.Item1, xyPercents.Item2, viewer);
                    }
                    else if (button == 2)
                    {
                        keyboardMouse.SendRightMouseDown(xyPercents.Item1, xyPercents.Item2, viewer);
                    }
                }
            });

            hubConnection.On("MouseUp", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var xyPercents = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    if (button == 0)
                    {
                        keyboardMouse.SendLeftMouseUp(xyPercents.Item1, xyPercents.Item2, viewer);
                    }
                    else if (button == 2)
                    {
                        keyboardMouse.SendRightMouseUp(xyPercents.Item1, xyPercents.Item2, viewer);
                    }
                }
            });

            hubConnection.On("MouseWheel", (double deltaX, double deltaY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    keyboardMouse.SendMouseWheel(-(int)deltaY, viewer);
                }
            });

            hubConnection.On("ViewerDisconnected", async (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.DisconnectRequested = true;
                }
                await hubConnection.InvokeAsync("ViewerDisconnected", viewerID);
                conductor.InvokeViewerRemoved(viewerID);

            });
            hubConnection.On("LatencyUpdate", (double latency, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.PendingFrames--;
                    viewer.Latency = latency;
                }
            });

            hubConnection.On("SelectScreen", (int screenIndex, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.Capturer.SetSelectedScreen(screenIndex);
                }
            });

            hubConnection.On("QualityChange", (int qualityLevel, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.ImageQuality = qualityLevel;
                }
            });


            hubConnection.On("TouchDown", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendLeftMouseDown(point.X, point.Y);
                }
            });
            hubConnection.On("LongPress", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendRightMouseDown(point.X, point.Y);
                    //Win32Interop.SendRightMouseUp(point.X, point.Y);
                }
            });
            hubConnection.On("TouchMove", (double moveX, double moveY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendMouseMove(point.X + moveX, point.Y + moveY);
                }
            });
            hubConnection.On("TouchUp", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendLeftMouseUp(point.X, point.Y);
                }
            });
            hubConnection.On("Tap", (double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var xyPercents = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    keyboardMouse.SendLeftMouseDown(xyPercents.Item1, xyPercents.Item2, viewer);
                    keyboardMouse.SendLeftMouseUp(xyPercents.Item1, xyPercents.Item2, viewer);
                }
            });
            hubConnection.On("SharedFileIDs", (List<string> fileIDs) => {
                fileIDs.ForEach(id =>
                {
                    var url = $"{conductor.Host}/API/FileSharing/{id}";
                    var webRequest = WebRequest.CreateHttp(url);
                    var response = webRequest.GetResponse();
                    var contentDisp = response.Headers["Content-Disposition"];
                    var fileName = contentDisp
                        .Split(";".ToCharArray())
                        .FirstOrDefault(x => x.Trim().StartsWith("filename"))
                        .Split("=".ToCharArray())[1];

                    var legalChars = fileName.ToCharArray().Where(x => !Path.GetInvalidFileNameChars().Any(y => x == y));

                    fileName = new string(legalChars.ToArray());

                    var dirPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "RemotelySharedFiles")).FullName;
                    var filePath = Path.Combine(dirPath, fileName);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (var rs = response.GetResponseStream())
                        {
                            rs.CopyTo(fs);
                        }
                    }
                    Process.Start("explorer.exe", dirPath);
                });
            });

            hubConnection.On("SessionID", (string sessionID) =>
            {
                conductor.InvokeSessionIDChanged(sessionID);
            });
        }
    }
}
