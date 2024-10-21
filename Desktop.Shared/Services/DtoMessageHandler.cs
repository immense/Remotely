using Remotely.Desktop.Shared.Abstractions;
using Remotely.Desktop.Shared.Enums;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models.Dtos;
using MessagePack;
using Microsoft.Extensions.Logging;
using Remotely.Desktop.Native.Windows;

namespace Remotely.Desktop.Shared.Services;

public interface IDtoMessageHandler
{
    Task ParseMessage(IViewer viewer, byte[] message);
}
public class DtoMessageHandler : IDtoMessageHandler
{
    private readonly IAudioCapturer _audioCapturer;

    private readonly IClipboardService _clipboardService;

    private readonly IFileTransferService _fileTransferService;

    private readonly IKeyboardMouseInput _keyboardMouseInput;

    private readonly ILogger<DtoMessageHandler> _logger;


    public DtoMessageHandler(
        IKeyboardMouseInput keyboardMouseInput,
        IAudioCapturer audioCapturer,
        IClipboardService clipboardService,
        IFileTransferService fileTransferService,
        ILogger<DtoMessageHandler> logger)
    {
        _keyboardMouseInput = keyboardMouseInput;
        _audioCapturer = audioCapturer;
        _clipboardService = clipboardService;
        _fileTransferService = fileTransferService;
        _logger = logger;
    }

    public async Task ParseMessage(IViewer viewer, byte[] message)
    {
        try
        {
            var wrapper = MessagePackSerializer.Deserialize<DtoWrapper>(message);

            switch (wrapper.DtoType)
            {
                case DtoType.MouseMove:
                case DtoType.MouseDown:
                case DtoType.MouseUp:
                case DtoType.Tap:
                case DtoType.MouseWheel:
                case DtoType.KeyDown:
                case DtoType.KeyUp:
                case DtoType.CtrlAltDel:
                case DtoType.ToggleBlockInput:
                case DtoType.TextTransfer:
                case DtoType.KeyPress:
                case DtoType.SetKeyStatesUp:
                    {
                        if (!viewer.HasControl)
                        {
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }

            switch (wrapper.DtoType)
            {
                case DtoType.SelectScreen:
                    SelectScreen(wrapper, viewer);
                    break;
                case DtoType.MouseMove:
                    MouseMove(wrapper, viewer);
                    break;
                case DtoType.MouseDown:
                    MouseDown(wrapper, viewer);
                    break;
                case DtoType.MouseUp:
                    MouseUp(wrapper, viewer);
                    break;
                case DtoType.Tap:
                    Tap(wrapper, viewer);
                    break;
                case DtoType.MouseWheel:
                    MouseWheel(wrapper);
                    break;
                case DtoType.KeyDown:
                    KeyDown(wrapper);
                    break;
                case DtoType.KeyUp:
                    KeyUp(wrapper);
                    break;
                case DtoType.CtrlAltDel:
                    CtrlAltDel();
                    break;
                case DtoType.ToggleAudio:
                    ToggleAudio(wrapper);
                    break;
                case DtoType.ToggleBlockInput:
                    ToggleBlockInput(wrapper);
                    break;
                case DtoType.TextTransfer:
                    await TransferText(wrapper);
                    break;
                case DtoType.KeyPress:
                    await KeyPress(wrapper);
                    break;
                case DtoType.File:
                    await DownloadFile(wrapper);
                    break;
                case DtoType.WindowsSessions:
                    await GetWindowsSessions(viewer);
                    break;
                case DtoType.SetKeyStatesUp:
                    SetKeyStatesUp();
                    break;
                case DtoType.FrameReceived:
                    HandleFrameReceived(wrapper, viewer);
                    break;
                case DtoType.OpenFileTransferWindow:
                    OpenFileTransferWindow(viewer);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while parsing message.");
        }
    }

    private async Task TransferText(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<TextTransferDto>(wrapper, out var dto))
        {
            return;
        }

        if (dto!.TypeText)
        {
            _keyboardMouseInput.SendText(dto.Text);
        }
        else
        {
            await _clipboardService.SetText(dto.Text);
        }
    }

    private void CtrlAltDel()
    {
        if (OperatingSystem.IsWindows())
        {
            // Might as well try both.
            User32.SendSAS(AsUser: false);
            User32.SendSAS(true);
        }
    }

    private async Task DownloadFile(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<FileDto>(wrapper, out var dto))
        {
            return;
        }

        await _fileTransferService.ReceiveFile(dto!.Buffer,
            dto.FileName,
            dto.MessageId,
            dto.EndOfFile,
            dto.StartOfFile);
    }

    private async Task GetWindowsSessions(IViewer viewer)
    {
        await viewer.SendWindowsSessions();
    }

    private void HandleFrameReceived(DtoWrapper wrapper, IViewer viewer)
    {
        if (!DtoChunker.TryComplete<FrameReceivedDto>(wrapper, out var dto))
        {
            return;
        }
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(dto.Timestamp);
        viewer.SetLastFrameReceived(timestamp.ToLocalTime());
    }
    private void KeyDown(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<KeyDownDto>(wrapper, out var dto))
        {
            return;
        }
        
        if (dto?.Key is null)
        {
            _logger.LogWarning("Key input is empty.");
            return;
        }
        _keyboardMouseInput.SendKeyDown(dto.Key);
    }

    private async Task KeyPress(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<KeyPressDto>(wrapper, out var dto))
        {
            return;
        }

        if (dto?.Key is null)
        {
            _logger.LogWarning("Key input is empty.");
            return;
        }

        _keyboardMouseInput.SendKeyDown(dto.Key);
        await Task.Delay(1);
        _keyboardMouseInput.SendKeyUp(dto.Key);
    }

    private void KeyUp(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<KeyUpDto>(wrapper, out var dto))
        {
            return;
        }
        
        if (dto?.Key is null)
        {
            _logger.LogWarning("Key input is empty.");
            return;
        }
        _keyboardMouseInput.SendKeyUp(dto.Key);
    }

    private void MouseDown(DtoWrapper wrapper, IViewer viewer)
    {
        if (!DtoChunker.TryComplete<MouseDownDto>(wrapper, out var dto))
        {
            return;
        }

        _keyboardMouseInput.SendMouseButtonAction(dto!.Button, ButtonAction.Down, dto.PercentX, dto.PercentY, viewer);
    }

    private void MouseMove(DtoWrapper wrapper, IViewer viewer)
    {
        if (!DtoChunker.TryComplete<MouseMoveDto>(wrapper, out var dto))
        {
            return;
        }
        
        _keyboardMouseInput.SendMouseMove(dto!.PercentX, dto.PercentY, viewer);
    }

    private void MouseUp(DtoWrapper wrapper, IViewer viewer)
    {
        if (!DtoChunker.TryComplete<MouseUpDto>(wrapper, out var dto))
        {
            return;
        }

        _keyboardMouseInput.SendMouseButtonAction(dto!.Button, ButtonAction.Up, dto.PercentX, dto.PercentY, viewer);
    }

    private void MouseWheel(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<MouseWheelDto>(wrapper, out var dto))
        {
            return;
        }

        _keyboardMouseInput.SendMouseWheel(-(int)dto!.DeltaY);
    }

    private void OpenFileTransferWindow(IViewer viewer)
    {
        _fileTransferService.OpenFileTransferWindow(viewer);
    }

    private void SelectScreen(DtoWrapper wrapper, IViewer viewer)
    {
        if (!DtoChunker.TryComplete<SelectScreenDto>(wrapper, out var dto))
        {
            return;
        }

        viewer.Capturer.SetSelectedScreen(dto!.DisplayName);
    }

    private void SetKeyStatesUp()
    {
        _keyboardMouseInput.SetKeyStatesUp();
    }

    private void Tap(DtoWrapper wrapper, IViewer viewer)
    {
        if (!DtoChunker.TryComplete<TapDto>(wrapper, out var dto))
        {
            return;
        }

        _keyboardMouseInput.SendMouseButtonAction(0, ButtonAction.Down, dto!.PercentX, dto.PercentY, viewer);
        _keyboardMouseInput.SendMouseButtonAction(0, ButtonAction.Up, dto.PercentX, dto.PercentY, viewer);
    }

    private void ToggleAudio(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<ToggleAudioDto>(wrapper, out var dto))
        {
            return;
        }

        _audioCapturer.ToggleAudio(dto!.ToggleOn);
    }

    private void ToggleBlockInput(DtoWrapper wrapper)
    {
        if (!DtoChunker.TryComplete<ToggleBlockInputDto>(wrapper, out var dto))
        {
            return;
        }
        _keyboardMouseInput.ToggleBlockInput(dto!.ToggleOn);
    }
}
