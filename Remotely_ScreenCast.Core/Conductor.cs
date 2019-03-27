using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely_ScreenCast.Core.Enums;
using Remotely_ScreenCast.Core.Input;
using Remotely_ScreenCast.Core.Models;
using Remotely_ScreenCast.Core.Sockets;
using Remotely_ScreenCast.Core.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Core
{
    public class Conductor
    {
        public AppMode Mode { get; private set; }
        public string RequesterID { get; private set; }
        public string ServiceID { get; private set; }
        public string Host { get; private set; }
        public HubConnection Connection { get; private set; }
        public OutgoingMessages OutgoingMessages { get; private set; }
        public string CurrentDesktopName { get; set; }
        public ConcurrentDictionary<string, Viewer> Viewers { get; } = new ConcurrentDictionary<string, Viewer>();
        public Dictionary<string, string> ArgDict { get; set; }

        public void SetMessageHandlers(IKeyboardMouseInput keyboardMouse)
        {
            OutgoingMessages = new OutgoingMessages(Connection);

            MessageHandlers.ApplyConnectionHandlers(Connection, this, keyboardMouse);
        }

        public Task Connect()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl($"{Host}/RCDeviceHub")
                .AddMessagePackProtocol()
                .Build();

            return Connection.StartAsync();
        }

        public void StartWaitForViewerTimer()
        {
            var timer = new System.Timers.Timer(10000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, arg) =>
            {
                // Shut down if no viewers have connected within 10 seconds.
                if (Viewers.Count == 0)
                {
                    Logger.Write("No viewers connected after 10 seconds.  Shutting down.");
                    Environment.Exit(0);
                }
            };
            timer.Start();
        }

        public void ProcessArgs(string[] args)
        {
            ArgDict = new Dictionary<string, string>();

            for (var i = 0; i < args.Length; i += 2)
            {
                var key = args?[i];
                if (key != null)
                {
                    key = key.Trim().Replace("-", "").ToLower();
                    var value = args?[i + 1];
                    if (value != null)
                    {
                        ArgDict[key] = args[i + 1].Trim();
                    }
                }

            }

            Mode = (AppMode)Enum.Parse(typeof(AppMode), ArgDict["mode"]);
            Host = ArgDict["host"];

            if (Mode == AppMode.Unattended)
            {
                RequesterID = ArgDict["requester"];
                CurrentDesktopName = ArgDict["desktop"];
                ServiceID = ArgDict["serviceid"];
            }

        }
        public EventHandler<string> SessionIDChanged { get; set; }
        public EventHandler<string> ViewerRemoved { get; set; }
        public EventHandler<Viewer> ViewerAdded { get; set; }
        public EventHandler<Tuple<string, string>> ScreenCastRequested { get; set; }
        public EventHandler<Tuple<string, string>> ScreenCastInitiated { get; set; }
    }
}
