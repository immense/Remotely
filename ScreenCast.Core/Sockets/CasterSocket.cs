using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Models;
using Remotely.ScreenCast.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotely.ScreenCast.Core.Capture;
using System.Diagnostics;
using System.IO;
using System.Net;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Core.Input;
using Remotely.Shared.Win32;

namespace Remotely.ScreenCast.Core.Sockets
{
    public class CasterSocket
    {
        public CasterSocket(HubConnection hubConnection, Conductor conductor, IKeyboardMouseInput keyboardMouseInput)
        {
            Connection = hubConnection;
            Conductor = conductor;
            KeyboardMouseInput = keyboardMouseInput;
            ApplyConnectionHandlers();
        }

        public Conductor Conductor { get; }
        public IKeyboardMouseInput KeyboardMouseInput { get; }
        private HubConnection Connection { get; }
        public async Task GetSessionID()
        {
            await Connection.SendAsync("GetSessionID");
        }

        public async Task NotifyRequesterUnattendedReady(string requesterID)
        {
            await Connection.SendAsync("NotifyRequesterUnattendedReady", requesterID);
        }

        public async Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
        {
            await Connection.SendAsync("NotifyViewersRelaunchedScreenCasterReady", viewerIDs);
        }

        public async Task SendAudioSample(byte[] buffer, List<string> viewerIDs)
        {
            await Connection.SendAsync("SendAudioSample", buffer, viewerIDs);
        }

        public async Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            await Connection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
        }

        public async Task SendCursorChange(CursorInfo cursor, List<string> viewerIDs)
        {
            await Connection.SendAsync("SendCursorChange", cursor, viewerIDs);
        }

        public async Task SendDeviceInfo(string serviceID, string machineName, string deviceID)
        {
            await Connection.SendAsync("ReceiveDeviceInfo", serviceID, machineName, deviceID);
        }

        public async Task SendMachineName(string machineName, string viewerID)
        {
            await Connection.SendAsync("SendMachineName", machineName, viewerID);
        }
        public async Task SendScreenCapture(byte[] captureBytes, string viewerID, int left, int top, int width, int height, DateTime captureTime)
        {
            await Connection.SendAsync("SendScreenCapture", captureBytes, viewerID, left, top, width, height, captureTime);
        }

        public async Task SendScreenCount(int primaryScreenIndex, int screenCount, string viewerID)
        {
            await Connection.SendAsync("SendScreenCountToBrowser", primaryScreenIndex, screenCount, viewerID);
        }

        public async Task SendScreenSize(int width, int height, string viewerID)
        {
            await Connection.SendAsync("SendScreenSize", width, height, viewerID);
        }

        public async Task SendViewerRemoved(string viewerID)
        {
            await Connection.SendAsync("SendViewerRemoved", viewerID);
        }

        private void ApplyConnectionHandlers()
        {
            Connection.Closed += (ex) =>
            {
                Logger.Write($"Connection closed.  Error: {ex.Message}");
                Environment.Exit(1);
                return Task.CompletedTask;
            };

            Connection.On("ClipboardTransfer", (string transferText, string viewerID) =>
            {
                try
                {
                    if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                    {
                        Conductor.InvokeClipboardTransfer(transferText);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("CtrlAltDel", async (string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    await Connection.InvokeAsync("CtrlAltDel");
                }
            });

            Connection.On("GetScreenCast", (string viewerID, string requesterName) =>
            {
                try
                {
                    Conductor.InvokeScreenCastInitiated(new ScreenCastRequest() { ViewerID = viewerID, RequesterName = requesterName });
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("RequestScreenCast", (string viewerID, string requesterName) =>
            {
                Conductor.InvokeScreenCastRequested(new ScreenCastRequest() { ViewerID = viewerID, RequesterName = requesterName });
            });

            Connection.On("KeyDown", (string key, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendKeyDown(key, viewer);
                }
            });

            Connection.On("KeyUp", (string key, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendKeyUp(key, viewer);
                }
            });

            Connection.On("KeyPress", async (string key, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendKeyDown(key, viewer);
                    await Task.Delay(1);
                    KeyboardMouseInput.SendKeyUp(key, viewer);
                }
            });

            Connection.On("MouseMove", (double percentX, double percentY, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendMouseMove(percentX, percentY, viewer);
                }
            });

            Connection.On("MouseDown", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    if (button == 0)
                    {
                        KeyboardMouseInput.SendLeftMouseDown(percentX, percentY, viewer);
                    }
                    else if (button == 2)
                    {
                        KeyboardMouseInput.SendRightMouseDown(percentX, percentY, viewer);
                    }
                }
            });

            Connection.On("MouseUp", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    if (button == 0)
                    {
                        KeyboardMouseInput.SendLeftMouseUp(percentX, percentY, viewer);
                    }
                    else if (button == 2)
                    {
                        KeyboardMouseInput.SendRightMouseUp(percentX, percentY, viewer);
                    }
                }
            });

            Connection.On("MouseWheel", (double deltaX, double deltaY, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendMouseWheel(-(int)deltaY, viewer);
                }
            });

            Connection.On("ViewerDisconnected", async (string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.DisconnectRequested = true;
                }
                await Connection.InvokeAsync("ViewerDisconnected", viewerID);
                Conductor.InvokeViewerRemoved(viewerID);

            });
            Connection.On("LatencyUpdate", (double latency, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.PendingFrames--;
                    viewer.Latency = latency;
                }
            });

            Connection.On("SelectScreen", (int screenIndex, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.Capturer.SetSelectedScreen(screenIndex);
                }
            });

            Connection.On("QualityChange", (int qualityLevel, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.ImageQuality = qualityLevel;
                }
            });

            Connection.On("ToggleAudio", (bool toggleOn, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    Conductor.InvokeAudioToggled(toggleOn);
                }
            });


            Connection.On("TouchDown", (string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendLeftMouseDown(point.X, point.Y);
                }
            });
            Connection.On("LongPress", (string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendRightMouseDown(point.X, point.Y);
                    //Win32Interop.SendRightMouseUp(point.X, point.Y);
                }
            });
            Connection.On("TouchMove", (double moveX, double moveY, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendMouseMove(point.X + moveX, point.Y + moveY);
                }
            });
            Connection.On("TouchUp", (string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendLeftMouseUp(point.X, point.Y);
                }
            });
            Connection.On("Tap", (double percentX, double percentY, string viewerID) =>
            {
                if (Conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendLeftMouseDown(percentX, percentY, viewer);
                    KeyboardMouseInput.SendLeftMouseUp(percentX, percentY, viewer);
                }
            });
            Connection.On("SharedFileIDs", (List<string> fileIDs) =>
            {
                fileIDs.ForEach(id =>
                {
                    var url = $"{Conductor.Host}/API/FileSharing/{id}";
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

            Connection.On("SessionID", (string sessionID) =>
            {
                Conductor.InvokeSessionIDChanged(sessionID);
            });
        }
    }
}
