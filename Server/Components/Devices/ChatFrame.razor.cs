using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Remotely.Server.Hubs;
using Remotely.Server.Models.Messages;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Enums;
using Remotely.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices;

public partial class ChatFrame : AuthComponentBase, IDisposable
{

    [Inject]
    private IClientAppState AppState { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [Inject]
    private IMessenger Messenger { get; init; } = null!;

    public void Dispose()
    {
        AppState.PropertyChanged -= AppState_PropertyChanged;
        Messenger.Unregister<ChatReceivedMessage, string>(this, CircuitConnection.ConnectionId);
        GC.SuppressFinalize(this);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        AppState.PropertyChanged += AppState_PropertyChanged;
        await Messenger.Register<ChatReceivedMessage, string>(
            this,
            CircuitConnection.ConnectionId,
            HandleChatMessageReceived);
    }

    private async Task HandleChatMessageReceived(ChatReceivedMessage message)
    {
        if (AppState.DevicesFrameChatSessions.Exists(x => x.DeviceId == message.DeviceId) ||
            message.DidDisconnect)
        {
            return;
        }

        var newChat = new ChatSession()
        {
            DeviceId = message.DeviceId,
            DeviceName = message.DeviceName,
            IsExpanded = true
        };

        newChat.ChatHistory.Add(new ChatHistoryItem()
        {
            Message = message.MessageText,
            Origin = ChatHistoryItemOrigin.Device
        });

        AppState.DevicesFrameChatSessions.Add(newChat);

        await InvokeAsync(StateHasChanged);
    }

    private void AppState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppState.DevicesFrameChatSessions))
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
