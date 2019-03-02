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
using System.Net;
using System.IO;
using System.Diagnostics;

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
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    Win32Interop.SendKeyDown((User32.VirtualKeyShort)keyCode);
                }
            });

            hubConnection.On("KeyUp", (int keyCode, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    Win32Interop.SendKeyUp((User32.VirtualKeyShort)keyCode);
                }
            });

            hubConnection.On("KeyPress", (int keyCode, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    
                    Win32Interop.SendKeyDown((User32.VirtualKeyShort)keyCode);
                    Win32Interop.SendKeyUp((User32.VirtualKeyShort)keyCode);
                }
            });

            hubConnection.On("MouseMove", (double percentX, double percentY, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var mousePoint = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    Win32Interop.SendMouseMove(mousePoint.Item1, mousePoint.Item2);
                }
            });

            hubConnection.On("MouseDown", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var mousePoint = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    if (button == 0)
                    {
                        Win32Interop.SendLeftMouseDown((int)mousePoint.Item1, (int)mousePoint.Item2);
                    }
                    else if (button == 2)
                    {
                        Win32Interop.SendRightMouseDown((int)mousePoint.Item1, (int)mousePoint.Item2);
                    }
                }
            });

            hubConnection.On("MouseUp", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    var mousePoint = ScreenCaster.GetAbsolutePercentFromRelativePercent(percentX, percentY, viewer.Capturer);
                    if (button == 0)
                    {
                        Win32Interop.SendLeftMouseUp((int)mousePoint.Item1, (int)mousePoint.Item2);
                    }
                    else if (button == 2)
                    {
                        Win32Interop.SendRightMouseUp((int)mousePoint.Item1, (int)mousePoint.Item2);
                    }
                }
            });

            hubConnection.On("MouseWheel", (double deltaX, double deltaY, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    Win32Interop.SendMouseWheel(-(int)deltaY);
                }
            });

            hubConnection.On("ViewerDisconnected", async (string viewerID) =>
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
                await hubConnection.InvokeAsync("ViewerDisconnected", viewerID);
            });
            hubConnection.On("FrameSkip", (int delayTime, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.NextCaptureDelay = delayTime;
                }
            });

            hubConnection.On("SessionID", (string sessionID) =>
            {
                var formattedSessionID = "";
                for (var i = 0; i < sessionID.Length; i += 3)
                {
                    formattedSessionID += sessionID.Substring(i, 3) + " ";
                }

                // TODO: Send to desktop app.
                formattedSessionID.Trim();
            });

            hubConnection.On("SelectScreen", (int screenIndex, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.Capturer.SelectedScreen = screenIndex;
                }
            });

            hubConnection.On("TouchDown", (string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    User32.GetCursorPos(out var point);
                    Win32Interop.SendLeftMouseDown(point.X, point.Y);
                }
            });
            hubConnection.On("LongPress", (string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    User32.GetCursorPos(out var point);
                    Win32Interop.SendRightMouseDown(point.X, point.Y);
                    Win32Interop.SendRightMouseUp(point.X, point.Y);
                }
            });
            hubConnection.On("TouchMove", (double moveX, double moveY, string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    User32.GetCursorPos(out var point);
                    Win32Interop.SendMouseMove(point.X + moveX, point.Y + moveY);
                }
            });
            hubConnection.On("TouchUp", (string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    User32.GetCursorPos(out var point);
                    Win32Interop.SendLeftMouseUp(point.X, point.Y);
                }
            });
            hubConnection.On("Tap", (string viewerID) =>
            {
                if (Program.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    User32.GetCursorPos(out var point);
                    Win32Interop.SendLeftMouseDown(point.X, point.Y);
                    Win32Interop.SendLeftMouseUp(point.X, point.Y);
                }
            });
            hubConnection.On("SharedFileIDs", (List<string> fileIDs) => {
                fileIDs.ForEach(id =>
                {
                    var url = $"{Program.Host}/API/FileSharing/{id}";
                    var webRequest = WebRequest.CreateHttp(url);
                    var response = webRequest.GetResponse();
                    var contentDisp = response.Headers["Content-Disposition"];
                    var fileName = contentDisp
                        .Split(";".ToCharArray())
                        .FirstOrDefault(x => x.Trim().StartsWith("filename"))
                        .Split("=".ToCharArray())[1];

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
        }
    }
}
