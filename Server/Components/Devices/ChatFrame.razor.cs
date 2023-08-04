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

public partial class ChatFrame : AuthComponentBase, IAsyncDisposable
{
    private ICollection<ChatSession> _chatSessions = Array.Empty<ChatSession>();

    [Inject]
    private IClientAppState AppState { get; init; } = null!;

    [Inject]
    private IChatSessionCache ChatCache { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [Inject]
    private IMessenger Messenger { get; init; } = null!;

    public async ValueTask DisposeAsync()
    {
        await Messenger.Unregister<ChatSessionsChangedMessage, string>(this, CircuitConnection.ConnectionId);
        await Messenger.Unregister<ChatReceivedMessage, string>(this, CircuitConnection.ConnectionId);
        GC.SuppressFinalize(this);
    }

    protected override async Task OnInitializedAsync()
    {
        _chatSessions = ChatCache.GetAllSessions();
        await Messenger.Register<ChatSessionsChangedMessage, string>(
            this,
            CircuitConnection.ConnectionId,
            HandleChatSessionsChanged);
        await Messenger.Register<ChatReceivedMessage, string>(
            this,
            CircuitConnection.ConnectionId,
            HandleChatMessageReceived);
        await base.OnInitializedAsync();
    }

    private async Task HandleChatMessageReceived(ChatReceivedMessage message)
    {
        if (message.DidDisconnect ||
            ChatCache.ContainsKey(message.DeviceId))
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

        ChatCache.AddOrUpdate(message.DeviceId, newChat, (k, v) => newChat);

        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleChatSessionsChanged(ChatSessionsChangedMessage message)
    {
        _chatSessions = ChatCache.GetAllSessions();
        await InvokeAsync(StateHasChanged);
    }
}
