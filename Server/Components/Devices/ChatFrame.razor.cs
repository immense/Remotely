using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Remotely.Server.Hubs;
using Remotely.Server.Models.Messages;
using Remotely.Server.Services.Stores;
using Remotely.Shared.Entities;
using Remotely.Shared.Enums;
using Remotely.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices;

public partial class ChatFrame : AuthComponentBase
{
    private ICollection<ChatSession> _chatSessions = Array.Empty<ChatSession>();

    [Inject]
    private ISelectedCardsStore CardStore { get; init; } = null!;

    [Inject]
    private IChatSessionStore ChatCache { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _chatSessions = ChatCache.GetAllSessions();
        await Register<ChatSessionsChangedMessage, string>(
            CircuitConnection.ConnectionId,
            HandleChatSessionsChanged);
        await Register<ChatReceivedMessage, string>(
            CircuitConnection.ConnectionId,
            HandleChatMessageReceived);

        await base.OnInitializedAsync();
    }

    private async Task HandleChatMessageReceived(object subscriber, ChatReceivedMessage message)
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

        newChat.ChatHistory.Enqueue(new ChatHistoryItem()
        {
            Message = message.MessageText,
            Origin = ChatHistoryItemOrigin.Device
        });

        ChatCache.AddOrUpdate(message.DeviceId, newChat, (k, v) => newChat);

        await Messenger.Send(new ChatSessionsChangedMessage(), CircuitConnection.ConnectionId);
    }

    private async Task HandleChatSessionsChanged(object subscriber, ChatSessionsChangedMessage message)
    {
        _chatSessions = ChatCache.GetAllSessions();
        await InvokeAsync(StateHasChanged);
    }
}
