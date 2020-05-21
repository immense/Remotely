using Remotely.ScreenCast.Core.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Remotely.ScreenCast.Core.Services
{
    public class IdleTimer
    {
        public IdleTimer(Conductor conductor)
        {
            ViewerList = conductor.Viewers;
        }

        public ConcurrentDictionary<string, Viewer> ViewerList { get; }

        public DateTimeOffset ViewersLastSeen { get; private set; } = DateTimeOffset.Now;

        private Timer Timer { get; set; }

        public void Start()
        {
            Timer?.Dispose();
            Timer = new Timer(100);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ViewerList.Count > 0)
            {
                ViewersLastSeen = DateTimeOffset.Now;
            }
            else if (DateTimeOffset.Now - ViewersLastSeen > TimeSpan.FromSeconds(30))
            {
                Logger.Write("No viewers connected after 30 seconds.  Shutting down.");
                Environment.Exit(0);
            }
        }
    }
}
