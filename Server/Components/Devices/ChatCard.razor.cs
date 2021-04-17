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
    public partial class ChatCard : AuthComponentBase, IDisposable
    {
        private ElementReference _chatMessagesWindow;

        private string _inputText;

        [Parameter]
        public ChatSession Session { get; set; }
        [Inject]
        private IClientAppState AppState { get; set; }

        [Inject]
        private ICircuitConnection CircuitConnection { get; set; }

        [Inject]
        private IJsInterop JsInterop { get; set; }
        public void Dispose()
        {
            AppState.PropertyChanged -= AppState_PropertyChanged;
            CircuitConnection.MessageReceived -= CircuitConnection_MessageReceived;
            GC.SuppressFinalize(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            JsInterop.ScrollToEnd(_chatMessagesWindow);
            base.OnAfterRender(firstRender);
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AppState.PropertyChanged += AppState_PropertyChanged;
            CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
        }

        private void AppState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Session.SessionId)
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private void CircuitConnection_MessageReceived(object sender, Models.CircuitEvent e)
        {
            if (e.EventName == Models.CircuitEventName.ChatReceived)
            {
                var deviceId = (string)e.Params[0];

                if (deviceId == Session.DeviceId)
                {
                    var deviceName = (string)e.Params[1];
                    var message = (string)e.Params[2];
                    var disconnected = (bool)e.Params[3];

                    var session = AppState.DevicesFrameChatSessions.Find(x => x.DeviceId == deviceId);

                    if (disconnected)
                    {
                        session.ChatHistory.Add(new ChatHistoryItem()
                        {
                            Message = $"{Session.DeviceName} disconnected.",
                            Origin = ChatHistoryItemOrigin.System
                        });
                    }
                    else
                    {
                        session.ChatHistory.Add(new ChatHistoryItem()
                        {
                            Message = message,
                            Origin = ChatHistoryItemOrigin.Device
                        });
                    }

                    if (!session.IsExpanded)
                    {
                        session.MissedChats++;
                    }

                    InvokeAsync(StateHasChanged);

                    JsInterop.ScrollToEnd(_chatMessagesWindow);
                }
            }
        }
        private void CloseChatCard()
        {
            AppState.DevicesFrameChatSessions.RemoveAll(x => x.DeviceId == Session.DeviceId);
            AppState.InvokePropertyChanged(nameof(AppState.DevicesFrameChatSessions));
        }
        private async Task EvaluateInputKeypress(KeyboardEventArgs args)
        {
            if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(_inputText))
                {
                    return;
                }

                await CircuitConnection.SendChat(_inputText, Session.DeviceId);

                Session.ChatHistory.Add(new ChatHistoryItem()
                {
                    Origin = ChatHistoryItemOrigin.Self,
                    Message = _inputText
                });

                _inputText = string.Empty;

                JsInterop.ScrollToEnd(_chatMessagesWindow);
            }
        }

        private void HeaderClicked()
        {
            Session.IsExpanded = !Session.IsExpanded;
            if (Session.IsExpanded)
            {
                Session.MissedChats = 0;
            }
        }
    }
}
