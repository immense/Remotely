using Remotely.Shared.Models;
using Remotely.ScreenCast.Core.Enums;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Remotely.Shared.Utilities;

namespace Remotely.ScreenCast.Core
{
    public class Conductor
    {
        public event EventHandler<ScreenCastRequest> ScreenCastRequested;

        public event EventHandler<string> SessionIDChanged;

        public event EventHandler<Viewer> ViewerAdded;

        public event EventHandler<string> ViewerRemoved;

        public Dictionary<string, string> ArgDict { get; set; }
        public string DeviceID { get; private set; }
        public string Host { get; private set; }
        public AppMode Mode { get; private set; }
        public string OrganizationName { get; private set; }
        public string RequesterID { get; private set; }
        public string ServiceID { get; private set; }
        public ConcurrentDictionary<string, Viewer> Viewers { get; } = new ConcurrentDictionary<string, Viewer>();

        public void InvokeScreenCastRequested(ScreenCastRequest viewerIdAndRequesterName)
        {
            ScreenCastRequested?.Invoke(null, viewerIdAndRequesterName);
        }

        public void InvokeSessionIDChanged(string sessionID)
        {
            SessionIDChanged?.Invoke(null, sessionID);
        }

        public void InvokeViewerAdded(Viewer viewer)
        {
            ViewerAdded?.Invoke(null, viewer);
        }

        public void InvokeViewerRemoved(string viewerID)
        {
            ViewerRemoved?.Invoke(null, viewerID);
        }

        public void ProcessArgs(string[] args)
        {
            ArgDict = new Dictionary<string, string>();
            for (var i = 0; i < args.Length; i += 2)
            {
                try
                {
                    var key = args?[i];
                    if (key != null)
                    {
                        if (!key.Contains("-"))
                        {
                            Logger.Write("Command line arguments are invalid.");
                            i -= 1;
                            continue;
                        }
                        key = key.Trim().Replace("-", "").ToLower();
                        if (i + 1 == args.Length)
                        {
                            ArgDict.Add(key, "true");
                            continue;
                        }
                        var value = args[i + 1];
                        if (value != null)
                        {
                            if (value.StartsWith("-"))
                            {
                                ArgDict.Add(key, "true");
                                i -= 1;
                            }
                            else
                            {
                                ArgDict.Add(key, args[i + 1].Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

            }

            Mode = (AppMode)Enum.Parse(typeof(AppMode), ArgDict["mode"], true);

            if (ArgDict.TryGetValue("host", out var host))
            {
                Host = host;
            }
            if (ArgDict.TryGetValue("requester", out var requester))
            {
                RequesterID = requester.Trim('"');
            }
            if (ArgDict.TryGetValue("serviceid", out var serviceID))
            {
                ServiceID = serviceID.Trim('"');
            }
            if (ArgDict.TryGetValue("deviceid", out var deviceID))
            {
                DeviceID = deviceID;
            }
            if (ArgDict.TryGetValue("organization", out var orgName))
            {
                OrganizationName = orgName;
            }
        }
    }
}
