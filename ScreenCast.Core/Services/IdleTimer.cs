using Remotely.ScreenCast.Core.Models;
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
            Timer.Elapsed += Timer_Elapsed;
        }

        public ConcurrentDictionary<string, Viewer> ViewerList { get; }

        public DateTimeOffset ViewersLastSeen { get; private set; } = DateTimeOffset.Now;

        private Timer Timer { get; } = new Timer(100);

        public void Start()
        {
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
