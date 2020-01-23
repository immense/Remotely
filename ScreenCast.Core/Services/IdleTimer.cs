using Remotely.ScreenCast.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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

        public DateTime ViewersLastSeen { get; private set; } = DateTime.Now;

        private Timer Timer { get; } = new Timer(100);

        public void Start()
        {
            Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ViewerList.Count > 0)
            {
                ViewersLastSeen = DateTime.Now;
            }
            else if (DateTime.Now - ViewersLastSeen > TimeSpan.FromSeconds(10))
            {
                Logger.Write("No viewers connected after 10 seconds.  Shutting down.");
                Environment.Exit(0);
            }
        }
    }
}
