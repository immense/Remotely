using Remotely.Desktop.Core.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Remotely.Desktop.Core
{
    public class Conductor
    {
        public event EventHandler<ScreenCastRequest> ScreenCastRequested;

        public event EventHandler<string> SessionIDChanged;

        public event EventHandler<Services.Viewer> ViewerAdded;

        public event EventHandler<string> ViewerRemoved;

        public Dictionary<string, string> ArgDict { get; set; }
        public string DeviceID { get; private set; }
        public string Host { get; private set; }
        public AppMode Mode { get; private set; }
        public string OrganizationId { get; private set; }
        public string OrganizationName { get; private set; }
        public string RequesterID { get; private set; }
        public string ServiceID { get; private set; }
        public ConcurrentDictionary<string, Services.Viewer> Viewers { get; } = new ConcurrentDictionary<string, Services.Viewer>();

        public void InvokeScreenCastRequested(ScreenCastRequest viewerIdAndRequesterName)
        {
            ScreenCastRequested?.Invoke(null, viewerIdAndRequesterName);
        }

        public void InvokeSessionIDChanged(string sessionID)
        {
            SessionIDChanged?.Invoke(null, sessionID);
        }

        public void InvokeViewerAdded(Services.Viewer viewer)
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
                            Logger.Write($"Command line arguments are invalid.  Key: {key}");
                            i -= 1;
                            continue;
                        }

                        key = key.Trim().Replace("-", "").ToLower();

                        ArgDict.Add(key, args[i + 1].Trim());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

            }

            if (ArgDict.TryGetValue("mode", out var mode))
            {
                Mode = (AppMode)Enum.Parse(typeof(AppMode), mode, true);
            }
            else
            {
                Mode = AppMode.Normal;
            }

            if (ArgDict.TryGetValue("host", out var host))
            {
                Host = host;
            }
            if (ArgDict.TryGetValue("requester", out var requester))
            {
                RequesterID = requester;
            }
            if (ArgDict.TryGetValue("serviceid", out var serviceID))
            {
                ServiceID = serviceID;
            }
            if (ArgDict.TryGetValue("deviceid", out var deviceID))
            {
                DeviceID = deviceID;
            }
            if (ArgDict.TryGetValue("organization", out var orgName))
            {
                OrganizationName = orgName;
            }
            if (ArgDict.TryGetValue("orgid", out var orgId))
            {
                OrganizationId = orgId;
            }
        }

        public void UpdateHost(string host)
        {
            Host = host;
        }

        public void UpdateOrganizationId(string organizationId)
        {
            OrganizationId = organizationId;
        }
    }
}
