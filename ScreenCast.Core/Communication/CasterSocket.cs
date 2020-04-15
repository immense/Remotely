using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Remotely.ScreenCast.Core.Communication
{
    public class CasterSocket
    {
        public CasterSocket(
            IKeyboardMouseInput keyboardMouseInput,
            IScreenCaster screenCastService,
            IAudioCapturer audioCapturer)
        {
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ScreenCaster = screenCastService;
        }

        public HubConnection Connection { get; private set; }
        public bool IsConnected => Connection?.State == HubConnectionState.Connected;
        public IScreenCaster ScreenCaster { get; }

        private IAudioCapturer AudioCapturer { get; }

        private IClipboardService ClipboardService { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        public async Task Connect(string host)
        {
            if (Connection != null)
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }
            Connection = new HubConnectionBuilder()
                .WithUrl($"{host}/RCDeviceHub")
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();

            ApplyConnectionHandlers();
            
            await Connection.StartAsync();
        }

        public async Task Disconnect()
        {
            await Connection.StopAsync();
            await Connection.DisposeAsync();
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

        public async Task SendCtrlAltDel()
        {
            await Connection.SendAsync("CtrlAltDel");
        }

        public async Task SendCursorChange(CursorInfo cursor, List<string> viewerIDs)
        {
            await Connection.SendAsync("SendCursorChange", cursor, viewerIDs);
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

        public async Task SendRtcOfferToBrowser(string sdp, string viewerID)
        {
            await Connection.SendAsync("SendRtcOfferToBrowser", sdp, viewerID);
        }

        public async Task SendScreenCapture(byte[] captureBytes, string viewerID, int left, int top, int width, int height, int imageQuality)
        {
            await Connection.SendAsync("SendScreenCapture", captureBytes, viewerID, left, top, width, height, imageQuality);
        }

        public async Task SendScreenData(string selectedScreen, string[] displayNames, string viewerID)
        {
            await Connection.SendAsync("SendScreenDataToBrowser", selectedScreen, displayNames, viewerID);
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
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                    {
                        viewer.RtcSession.AddIceCandidate(sdpMid, sdpMlineIndex, candidate);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

            });

            Connection.On("ReceiveRtcAnswer", (string sdp, string viewerID) =>
            {
                try
                {
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer) && viewer.HasControl)
                    {
                        viewer.RtcSession.SetRemoteDescription("answer", sdp);
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
                foreach (var viewer in conductor.Viewers.Values.ToList())
                {
                    await Connection.SendAsync("ViewerDisconnected", viewer.ViewerConnectionID);
                    viewer.DisconnectRequested = true;
                }
            });

            Connection.On("GetScreenCast", (string viewerID, string requesterName) =>
            {
                try
                {
                    ScreenCaster.BeginScreenCasting(new ScreenCastRequest() { ViewerID = viewerID, RequesterName = requesterName });
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("RequestScreenCast", (string viewerID, string requesterName) =>
            {
                conductor.InvokeScreenCastRequested(new ScreenCastRequest() { ViewerID = viewerID, RequesterName = requesterName });
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

            Connection.On("ViewerDisconnected", async (string viewerID) =>
            {
                await Connection.SendAsync("ViewerDisconnected", viewerID);
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.DisconnectRequested = true;
                }
                conductor.InvokeViewerRemoved(viewerID);

            });
            Connection.On("FrameReceived", (int bytesReceived, string viewerID) =>
            {
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.WebSocketBuffer = Math.Max(0, viewer.WebSocketBuffer - bytesReceived);
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

            //Connection.On("SharedFileIDs", (List<string> fileIDs) =>
            //{
            //    fileIDs.ForEach(id =>
            //    {
            //        var url = $"{conductor.Host}/API/FileSharing/{id}";
            //        var webRequest = WebRequest.CreateHttp(url);
            //        var response = webRequest.GetResponse();
            //        var contentDisp = response.Headers["Content-Disposition"];
            //        var fileName = contentDisp
            //            .Split(";".ToCharArray())
            //            .FirstOrDefault(x => x.Trim().StartsWith("filename"))
            //            .Split("=".ToCharArray())[1];

            //        var legalChars = fileName.ToCharArray().Where(x => !Path.GetInvalidFileNameChars().Any(y => x == y));

            //        fileName = new string(legalChars.ToArray());

            //        var dirPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "RemotelySharedFiles")).FullName;
            //        var filePath = Path.Combine(dirPath, fileName);
            //        using (var fs = new FileStream(filePath, FileMode.Create))
            //        {
            //            using (var rs = response.GetResponseStream())
            //            {
            //                rs.CopyTo(fs);
            //            }
            //        }
            //        Process.Start("explorer.exe", dirPath);
            //    });
            //});

            Connection.On("SessionID", (string sessionID) =>
            {
                conductor.InvokeSessionIDChanged(sessionID);
            });
        }
    }
}
