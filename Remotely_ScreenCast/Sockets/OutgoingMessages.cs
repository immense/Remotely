using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCapture.Sockets
{
    public class OutgoingMessages
    {
        public OutgoingMessages(HubConnection hubConnection)
        {
            Connection = hubConnection;
        }

        private HubConnection Connection { get; }
        public async Task SendScreenSize(int width, int height)
        {
            await Connection.SendAsync("ScreenSize", width, height);
        }

        public async Task SendScreenCapture(byte[] captureBytes)
        {
            await Connection.SendAsync("ScreenCapture", captureBytes);
        }

        internal async Task SendScreenCount(int primaryScreenIndex, int screenCount, string requesterID)
        {
            await Connection.SendAsync("SendScreenCountToBrowser", primaryScreenIndex, screenCount, requesterID);
        }
    }
}
