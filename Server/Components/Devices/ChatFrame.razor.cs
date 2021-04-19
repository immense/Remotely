using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Remotely.Server.Hubs;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices
{
    public partial class ChatFrame : AuthComponentBase, IDisposable
    {

        [Inject]
        private IClientAppState AppState { get; set; }

        [Inject]
        private ICircuitConnection CircuitConnection { get; set; }

        public void Dispose()
        {
            AppState.PropertyChanged -= AppState_PropertyChanged;
            CircuitConnection.MessageReceived -= CircuitConnection_MessageReceived;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AppState.PropertyChanged += AppState_PropertyChanged;
            CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
        }

        private void CircuitConnection_MessageReceived(object sender, Models.CircuitEvent e)
        {
            if (e.EventName == Models.CircuitEventName.ChatReceived)
            {
                var deviceId = (string)e.Params[0];

                if (!AppState.DevicesFrameChatSessions.Exists(x => x.DeviceId == deviceId))
                {
                    var deviceName = (string)e.Params[1];
                    var message = (string)e.Params[2];
                    var disconnected = (bool)e.Params[3];

                    if (disconnected)
                    {
                        return;
                    }

                    var newChat = new ChatSession()
                    {
                        DeviceId = deviceId,
                        DeviceName = deviceName,
                        IsExpanded = true
                    };

                    newChat.ChatHistory.Add(new ChatHistoryItem()
                    {
                        Message = message,
                        Origin = ChatHistoryItemOrigin.Device
                    });

                    AppState.DevicesFrameChatSessions.Add(newChat);

                    InvokeAsync(StateHasChanged);
                }
            }
        }

        private void AppState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.DevicesFrameChatSessions))
            {
                InvokeAsync(StateHasChanged);
            }
        }
    }
}
