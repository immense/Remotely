using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations
{
    public interface IHubEventHandlerEx : IHubEventHandler
    {
        Task<bool> TryWaitForSession(string sessionId, Func<Task> createSessionFunc);
    }

    public class HubEventHandlerEx : IHubEventHandlerEx
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _sessionWaitHandlers = new();

        private readonly ICircuitManager _circuitManager;
        private readonly IHubContext<ServiceHub> _serviceHub;
        private readonly ILogger<HubEventHandlerEx> _logger;

        public HubEventHandlerEx(
            ICircuitManager circuitManager,
            IHubContext<ServiceHub> serviceHub,
            ILogger<HubEventHandlerEx> logger)
        {
            _circuitManager = circuitManager;
            _serviceHub = serviceHub;
            _logger = logger;
        }

        public Task ChangeWindowsSession(RemoteControlSession session, string viewerConnectionId, int targetWindowsSession)
        {
            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            return _serviceHub.Clients
                .Client(ex.ServiceConnectionId)
                .SendAsync("ChangeWindowsSession",
                    viewerConnectionId,
                    ex.UnattendedSessionId,
                    ex.AccessKey,
                    ex.UserConnectionId,
                    ex.RequesterUserName,
                    ex.OrganizationName,
                    ex.OrganizationId,
                    targetWindowsSession);
        }

        public Task InvokeCtrlAltDel(RemoteControlSession session, string viewerConnectionId)
        {
            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            return _serviceHub.Clients.Client(ex.ServiceConnectionId).SendAsync("CtrlAltDel");
        }

        public Task NotifySessionChanged(RemoteControlSession session, SessionSwitchReasonEx reason, int currentSessionId)
        {
            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            switch (reason)
            {
                case SessionSwitchReasonEx.ConsoleDisconnect:
                case SessionSwitchReasonEx.RemoteConnect:
                case SessionSwitchReasonEx.RemoteDisconnect:
                case SessionSwitchReasonEx.SessionLogoff:
                case SessionSwitchReasonEx.SessionLock:
                case SessionSwitchReasonEx.SessionRemoteControl:
                    return _serviceHub.Clients
                      .Client(ex.ServiceConnectionId)
                      .SendAsync("RestartScreenCaster",
                          ex.ViewerList,
                          ex.UnattendedSessionId,
                          ex.AccessKey,
                          ex.UserConnectionId,
                          ex.RequesterUserName,
                          ex.OrganizationName,
                          ex.OrganizationId);
                case SessionSwitchReasonEx.ConsoleConnect:
                case SessionSwitchReasonEx.SessionUnlock:
                case SessionSwitchReasonEx.SessionLogon:
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        public Task NotifyUnattendedSessionReady(RemoteControlSession session, string relativeAccessUrl)
        {
            if (_sessionWaitHandlers.TryGetValue(session.UnattendedSessionId, out var waitHandle))
            {
                waitHandle.Release();
                return Task.CompletedTask;
            }

            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            return _circuitManager.InvokeOnConnection(
                ex.UserConnectionId,
                CircuitEventName.UnattendedSessionReady,
                session.UnattendedSessionId, 
                session.AccessKey,
                ex.DeviceId,
                ex.ViewOnly);
        }

        public Task RestartScreenCaster(RemoteControlSession session, HashSet<string> viewerList)
        {

            if (session is not RemoteControlSessionEx ex)
            {
                _logger.LogError("Event should have been for RemoteControlSessionEx.");
                return Task.CompletedTask;
            }

            return _serviceHub.Clients
                     .Client(ex.ServiceConnectionId)
                     .SendAsync("RestartScreenCaster",
                            viewerList,
                            ex.UnattendedSessionId,
                            ex.AccessKey,
                            ex.UserConnectionId,
                            ex.RequesterUserName,
                            ex.OrganizationName,
                            ex.OrganizationId);
        }

        public async Task<bool> TryWaitForSession(string sessionId, Func<Task> createSessionFunc)
        {
            try
            {
                var waitHandle = _sessionWaitHandlers.AddOrUpdate(sessionId, new SemaphoreSlim(0, 1), (k, v) =>
                {
                    v.Release();
                    return new SemaphoreSlim(0, 1);
                });

                await createSessionFunc();

                return waitHandle.Wait(TimeSpan.FromSeconds(30));
            }
            finally
            {
                if (_sessionWaitHandlers.TryRemove(sessionId, out var result))
                {
                    result.Dispose();
                }
            }
        }

    }
}
