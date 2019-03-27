using Microsoft.AspNetCore.SignalR.Client;
using Remotely_ScreenCast.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Core.Sockets
{
    public class OutgoingMessages
    {
        public OutgoingMessages(HubConnection hubConnection)
        {
            Connection = hubConnection;
        }

        private HubConnection Connection { get; }
        public async Task SendScreenSize(int width, int height, string viewerID)
        {
            await Connection.SendAsync("SendScreenSize", width, height, viewerID);
        }

        public async Task SendScreenCapture(byte[] captureBytes, string viewerID, int left, int top, int width, int height, DateTime captureTime)
        {
            await Connection.SendAsync("SendScreenCapture", captureBytes, viewerID, left, top, width, height, captureTime);
        }

        public async Task SendScreenCount(int primaryScreenIndex, int screenCount, string viewerID)
        {
            await Connection.SendAsync("SendScreenCountToBrowser", primaryScreenIndex, screenCount, viewerID);
        }

        public async Task NotifyRequesterUnattendedReady(string requesterID)
        {
            await Connection.SendAsync("NotifyRequesterUnattendedReady", requesterID);
        }

        public async Task SendCursorChange(CursorInfo cursor, List<string> viewerIDs)
        {
            await Connection.SendAsync("SendCursorChange", cursor, viewerIDs);
        }

        public async Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
        {
            await Connection.SendAsync("NotifyViewersRelaunchedScreenCasterReady", viewerIDs);
        }

        public async Task SendDeviceInfo(string serviceID, string machineName)
        {
            await Connection.SendAsync("ReceiveDeviceInfo", serviceID, machineName);
        }

        public async Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            await Connection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
        }

        public async Task GetSessionID()
        {
            await Connection.SendAsync("GetSessionID");
        }

        public async Task SendViewerRemoved(string viewerID)
        {
            await Connection.SendAsync("SendViewerRemoved", viewerID);
        }
    }
}
