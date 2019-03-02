using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Sockets
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

        public async Task SendScreenCapture(byte[] captureBytes, string viewerID, DateTime captureTime)
        {
            await Connection.SendAsync("SendScreenCapture", captureBytes, viewerID, captureTime);
        }

        internal async Task SendScreenCount(int primaryScreenIndex, int screenCount, string viewerID)
        {
            await Connection.SendAsync("SendScreenCountToBrowser", primaryScreenIndex, screenCount, viewerID);
        }

        public async Task NotifyRequesterUnattendedReady(string requesterID)
        {
            await Connection.SendAsync("NotifyRequesterUnattendedReady", requesterID);
        }

        public async Task SendCursorChange(int cursor, List<string> viewerIDs)
        {
            await Connection.SendAsync("SendCursorChange", cursor, viewerIDs);
        }
    }
}
