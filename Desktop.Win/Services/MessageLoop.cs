using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Bitbound.SimpleMessenger;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Remotely.Desktop.Shared.Enums;      // ButtonAction
using Remotely.Desktop.Shared.Messages;   // ButtonActionMessage
using Remotely.Desktop.UI.Services;
using Remotely.Desktop.Win;               // PrivacyOverlay
using Remotely.Shared.Enums;              // SessionEndReasonsEx, SessionSwitchReasonEx

namespace Remotely.Desktop.Win.Services;

public interface IMessageLoop
{
    void StartMessageLoop();
}

[SupportedOSPlatform("windows")]
public class MessageLoop : IMessageLoop
{
    private readonly CancellationToken _exitToken;
    private readonly ILogger<MessageLoop> _logger;
    private readonly IMessenger _messenger;

    private Thread? _messageLoopThread;
    private IDisposable? _buttonActionReg;   // unregister on exit

    public MessageLoop(
        IMessenger messenger,
        IUiDispatcher uiDispatcher,
        ILogger<MessageLoop> logger)
    {
        _messenger = messenger;
        _logger = logger;
        _exitToken = uiDispatcher.ApplicationExitingToken;

        // ⚠️ Your IMessenger.Register<T> requires (object recipient, RegistrationCallback<TMessage>)
        // and the callback returns Task. So we pass "this" and a Task-returning method.
        _buttonActionReg = _messenger.Register<ButtonActionMessage>(this, OnButtonAction);

        // Cleanup on shutdown
        _exitToken.Register(() =>
        {
            try { _buttonActionReg?.Dispose(); } catch { }
            try { PrivacyOverlay.Disable(); } catch { }
        });
    }

    // Registration callback signature: Task(object recipient, TMessage msg)
    private Task OnButtonAction(object _, ButtonActionMessage msg)
    {
        HandleButtonAction(msg);
        return Task.CompletedTask;
    }

    public void StartMessageLoop()
    {
        if (_messageLoopThread is not null)
            throw new InvalidOperationException("Message loop already started.");

        _messageLoopThread = new Thread(() =>
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            while (!_exitToken.IsCancellationRequested)
            {
                try
                {
                    while (GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
                        DispatchMessage(ref msg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in message loop.");
                }
            }

            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        });

        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.Start();
    }

    // ---- Win32 message pump ----
    [DllImport("user32.dll")]
    private static extern bool DispatchMessage([In] ref MSG lpmsg);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    // ---- System event relays ----
    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e) =>
        _messenger.Send(new DisplaySettingsChangedMessage());

    private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
    {
        _logger.LogInformation("Session ending. Reason: {reason}", e.Reason);
        var reason = (SessionEndReasonsEx)e.Reason;
        _messenger.Send(new WindowsSessionEndingMessage(reason));
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        _logger.LogInformation("Session changing. Reason: {reason}", e.Reason);
        var reason = (SessionSwitchReasonEx)(int)e.Reason;
        _messenger.Send(new WindowsSessionSwitchedMessage(reason, Process.GetCurrentProcess().SessionId));
    }

    // ---- Handle viewer toolbar actions (Privacy) ----
    private void HandleButtonAction(ButtonActionMessage msg)
    {
        switch (msg.Action)
        {
            case ButtonAction.PrivacyOn:
                PrivacyOverlay.Enable();
                break;
            case ButtonAction.PrivacyOff:
                PrivacyOverlay.Disable();
                break;
            default:
                break;
        }
    }

    // ---- Win32 structs ----
    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y) { X = x; Y = y; }
    }
}
