using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Remotely.Server.Hubs;
using Remotely.Server.Models.Messages;
using Remotely.Server.Services;
using Remotely.Server.Services.Stores;
using Remotely.Shared.Enums;
using Remotely.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices;

public partial class ChatCard : AuthComponentBase
{
    private ElementReference _chatMessagesWindow;

    private string? _inputText;

    [Parameter]
    [EditorRequired]
    public required ChatSession Session { get; set; }

    [Inject]
    private IChatSessionStore ChatSessionStore { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [Inject]
    private IJsInterop JsInterop { get; init; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        JsInterop.ScrollToEnd(_chatMessagesWindow);
        base.OnAfterRender(firstRender);
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await Register<ChatReceivedMessage, string>(
            CircuitConnection.ConnectionId,
            HandleChatMessageReceived);
    }

    private async Task HandleChatMessageReceived(object subscriber, ChatReceivedMessage message)
    {
        if (message.DeviceId != Session.DeviceId)
        {
            return;
        }

        if (!ChatSessionStore.TryGetSession(message.DeviceId, out var session))
        {
            return;
        }

        if (message.DidDisconnect)
        {
            session.ChatHistory.Enqueue(new ChatHistoryItem()
            {
                Message = $"{Session.DeviceName} disconnected.",
                Origin = ChatHistoryItemOrigin.System
            });
        }
        else
        {
            session.ChatHistory.Enqueue(new ChatHistoryItem()
            {
                Message = message.MessageText,
                Origin = ChatHistoryItemOrigin.Device
            });
        }

        if (!session.IsExpanded)
        {
            session.MissedChats++;
        }

        await InvokeAsync(StateHasChanged);

        JsInterop.ScrollToEnd(_chatMessagesWindow);
    }

    private async Task CloseChatCard()
    {
        await CircuitConnection.SendChat(string.Empty, $"{Session.DeviceId}", true);
        _ = ChatSessionStore.TryRemove($"{Session.DeviceId}", out _);
        var message = new ChatSessionsChangedMessage();
        await Messenger.Send(message, CircuitConnection.ConnectionId);
    }
    private async Task EvaluateInputKeypress(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(_inputText))
            {
                return;
            }

            await CircuitConnection.SendChat(_inputText, $"{Session.DeviceId}");

            Session.ChatHistory.Enqueue(new ChatHistoryItem()
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
