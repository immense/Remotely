using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using Remotely.Desktop.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Utilities;
using System.Threading;
using Remotely.Desktop.Core.Models;

namespace Remotely.Desktop.Core.Services
{
    public class CasterSocket
    {
        public CasterSocket(
            IKeyboardMouseInput keyboardMouseInput,
            IScreenCaster screenCastService,
            IAudioCapturer audioCapturer,
            IFileTransferService fileDownloadService,
            IClipboardService clipboardService)
        {
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ScreenCaster = screenCastService;
            FileDownloadService = fileDownloadService;
            ClipboardService = clipboardService;
        }

        public HubConnection Connection { get; private set; }
        public bool IsConnected => Connection?.State == HubConnectionState.Connected;
        private IAudioCapturer AudioCapturer { get; }
        private IClipboardService ClipboardService { get; }
        private IFileTransferService FileDownloadService { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        private IScreenCaster ScreenCaster { get; }
        public async Task<bool> Connect(string host)
        {
            try
            {
                if (Connection != null)
                {
                    await Connection.StopAsync();
                    await Connection.DisposeAsync();
                }
                Connection = new HubConnectionBuilder()
                    .WithUrl($"{host}/CasterHub")
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .Build();

                ApplyConnectionHandlers();

                await Connection.StartAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public async Task Disconnect()
        {
            await Connection.StopAsync();
            await Connection.DisposeAsync();
        }

        public async Task DisconnectAllViewers()
        {
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            foreach (var viewer in conductor.Viewers.Values.ToList())
            {
                await DisconnectViewer(viewer, true);
            }
        }

        public async Task<IceServerModel[]> GetIceServers()
        {
            return await Connection.InvokeAsync<IceServerModel[]>("GetIceServers");
        }

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

        public async Task SendAudioSample(byte[] buffer, string viewerID)
        {
            await Connection.SendAsync("SendAudioSample", buffer, viewerID);
        }

        public async Task SendClipboardText(string clipboardText, string viewerID)
        {
            await Connection.SendAsync("SendClipboardText", clipboardText, viewerID);
        }

        public async Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            await Connection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
        }

        public async Task SendConnectionRequestDenied(string viewerID)
        {
            await Connection.SendAsync("SendConnectionRequestDenied", viewerID);
        }

        public async Task SendCtrlAltDel()
        {
            await Connection.SendAsync("CtrlAltDel");
        }

        public async Task SendCursorChange(CursorInfo cursor, string viewerID)
        {
            await Connection.SendAsync("SendCursorChange", cursor, viewerID);
        }

        public async Task SendDeviceInfo(string serviceID, string machineName, string deviceID)
        {
            await Connection.SendAsync("ReceiveDeviceInfo", serviceID, machineName, deviceID);
        }

        public async Task SendIceCandidateToBrowser(string candidate, int sdpMlineIndex, string sdpMid, string viewerConnectionID)
        {
            await Connection.SendAsync("SendIceCandidateToBrowser", candidate, sdpMlineIndex, sdpMid, viewerConnectionID);
        }

        public async Task SendMachineName(string machineName, string viewerID)
        {
            await Connection.SendAsync("SendMachineName", machineName, viewerID);
        }

        public async Task SendRtcOfferToBrowser(string sdp, string viewerID, IceServerModel[] iceServers)
        {
            await Connection.SendAsync("SendRtcOfferToBrowser", sdp, viewerID, iceServers);
        }

        public async Task SendScreenCapture(byte[] imageBytes, string viewerID, int left, int top, int width, int height, int imageQuality)
        {
            for (var i = 0; i < imageBytes.Length; i += 50_000)
            {
                await Connection.SendAsync("SendScreenCapture",
                    imageBytes.Skip(i).Take(50_000).ToArray(),
                    viewerID,
                    left,
                    top,
                    width,
                    height,
                    imageQuality,
                    false);
            }
            await Connection.SendAsync("SendScreenCapture", Array.Empty<byte>(),
                viewerID,
                left,
                top,
                width,
                height,
                imageQuality,
                true);
        }

        public async Task SendScreenData(string selectedScreen, string[] displayNames, string viewerID)
        {
            await Connection.SendAsync("SendScreenDataToBrowser", selectedScreen, displayNames, viewerID);
        }

        public async Task SendScreenSize(int width, int height, string viewerID)
        {
            await Connection.SendAsync("SendScreenSize", width, height, viewerID);
        }

        public async Task DisconnectViewer(Viewer viewer, bool notifyViewer)
        {
            viewer.DisconnectRequested = true;
            viewer.Dispose();
            await Connection.SendAsync("DisconnectViewer", viewer.ViewerConnectionID, notifyViewer);
        }
        public async Task SendWindowsSessions(List<WindowsSession> windowsSessions, string viewerID)
        {
            await Connection.SendAsync("SendWindowsSessions", windowsSessions, viewerID);
        }

        private void ApplyConnectionHandlers()
        {
            // TODO: Remove circular dependencies and the need for static IServiceProvider instance
            // by emitting these events so other services can listen for them.
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            Connection.Closed += (ex) =>
            {
                Logger.Write($"Connection closed.  Error: {ex?.Message}");
                return Task.CompletedTask;
            };

            Connection.On("ReceiveIceCandidate", (string candidate, int sdpMlineIndex, string sdpMid, string viewerID) =>
            {
                try
                {
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                    {
                        viewer.RtcSession.AddIceCandidate(sdpMid, sdpMlineIndex, candidate);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

            });

            Connection.On("ReceiveRtcAnswer", async (string sdp, string viewerID) =>
            {
                try
                {
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                    {
                        await viewer.RtcSession.SetRemoteDescription("answer", sdp);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("ClipboardTransfer", (string transferText, bool typeText, string viewerID) =>
            {
                try
                {
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                    {
                        if (typeText)
                        {
                            KeyboardMouseInput.SendText(transferText, viewer);
                        }
                        else
                        {
                            ClipboardService.SetText(transferText);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("CtrlAltDel", async (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    await SendCtrlAltDel();
                }
            });

            Connection.On("Disconnect", async (string reason) =>
            {
                Logger.Write($"Disconnecting caster socket.  Reason: {reason}");
                await DisconnectAllViewers();
            });

            Connection.On("GetScreenCast", (string viewerID, string requesterName, bool notifyUser) =>
            {
                try
                {
                    ScreenCaster.BeginScreenCasting(new ScreenCastRequest()
                    { 
                        NotifyUser = notifyUser,
                        ViewerID = viewerID, 
                        RequesterName = requesterName 
                    });
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("GetWindowsSessions", async (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    await viewer.SendWindowsSessions();
                }
            });

            Connection.On("RequestScreenCast", (string viewerID, string requesterName, bool notifyUser) =>
            {
                conductor.InvokeScreenCastRequested(new ScreenCastRequest() 
                { 
                    NotifyUser = notifyUser,
                    ViewerID = viewerID, 
                    RequesterName = requesterName 
                });
            });

            Connection.On("KeyDown", (string key, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendKeyDown(key, viewer);
                }
            });

            Connection.On("KeyUp", (string key, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendKeyUp(key, viewer);
                }
            });

            Connection.On("KeyPress", async (string key, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendKeyDown(key, viewer);
                    await Task.Delay(1);
                    KeyboardMouseInput.SendKeyUp(key, viewer);
                }
            });

            Connection.On("MouseMove", (double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendMouseMove(percentX, percentY, viewer);
                }
            });

            Connection.On("MouseDown", (int button, double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
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
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
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
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendMouseWheel(-(int)deltaY, viewer);
                }
            });
            Connection.On("SetKeyStatesUp", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SetKeyStatesUp();
                }
            });
            Connection.On("ViewerDisconnected", async (string viewerID) =>
            {
                await Connection.SendAsync("DisconnectViewer", viewerID, false);
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.DisconnectRequested = true;
                    viewer.Dispose();
                }
                conductor.InvokeViewerRemoved(viewerID);

            });
            Connection.On("FrameReceived", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (viewer.PendingSentFrames.TryDequeue(out _))
                        {
                            break;
                        }
                    }
                }
            });

            Connection.On("SelectScreen", (string displayName, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.Capturer.SetSelectedScreen(displayName);
                }
            });

            Connection.On("QualityChange", (int qualityLevel, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.ImageQuality = qualityLevel;
                }
            });

            Connection.On("AutoQualityAdjust", (bool isOn, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.AutoAdjustQuality = isOn;
                }
            });

            Connection.On("ReceiveFile", async (byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile) =>
            {
                await FileDownloadService.ReceiveFile(buffer, fileName, messageId, endOfFile, startOfFile);
            });

            Connection.On("ToggleAudio", (bool toggleOn, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    AudioCapturer.ToggleAudio(toggleOn);
                }
            });
            Connection.On("ToggleBlockInput", (bool toggleOn, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.ToggleBlockInput(toggleOn);
                }
            });

            Connection.On("ToggleWebRtcVideo", (bool toggleOn, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.ToggleWebRtcVideo(toggleOn);
                }
            });

            Connection.On("TouchDown", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendLeftMouseDown(point.X, point.Y);
                }
            });
            Connection.On("LongPress", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendRightMouseDown(point.X, point.Y);
                    //Win32Interop.SendRightMouseUp(point.X, point.Y);
                }
            });
            Connection.On("TouchMove", (double moveX, double moveY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendMouseMove(point.X + moveX, point.Y + moveY);
                }
            });
            Connection.On("TouchUp", (string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    //User32.GetCursorPos(out var point);
                    //Win32Interop.SendLeftMouseUp(point.X, point.Y);
                }
            });
            Connection.On("Tap", (double percentX, double percentY, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                {
                    KeyboardMouseInput.SendLeftMouseDown(percentX, percentY, viewer);
                    KeyboardMouseInput.SendLeftMouseUp(percentX, percentY, viewer);
                }
            });

            Connection.On("SharedFileIDs", (List<string> fileIDs) =>
            {
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

            Connection.On("SessionID", (string sessionID) =>
            {
                conductor.InvokeSessionIDChanged(sessionID);
            });
        }
    }
}
