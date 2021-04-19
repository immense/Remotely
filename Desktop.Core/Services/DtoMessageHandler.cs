using MessagePack;
using Remotely.Desktop.Core.Enums;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Enums;
using Remotely.Shared.Models.RemoteControlDtos;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public interface IDtoMessageHandler
    {
        Task ParseMessage(Viewer viewer, byte[] message);
    }
    public class DtoMessageHandler : IDtoMessageHandler
    {
        public DtoMessageHandler(IKeyboardMouseInput keyboardMouseInput,
            IAudioCapturer audioCapturer,
            IClipboardService clipboardService,
            IFileTransferService fileTransferService)
        {
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ClipboardService = clipboardService;
            FileTransferService = fileTransferService;
        }

        private IAudioCapturer AudioCapturer { get; }
        private IClipboardService ClipboardService { get; }
        private IFileTransferService FileTransferService { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        public async Task ParseMessage(Viewer viewer, byte[] message)
        {
            try
            {
                var baseDto = MessagePackSerializer.Deserialize<BaseDto>(message);

                switch (baseDto.DtoType)
                {
                    case BaseDtoType.MouseMove:
                    case BaseDtoType.MouseDown:
                    case BaseDtoType.MouseUp:
                    case BaseDtoType.Tap:
                    case BaseDtoType.MouseWheel:
                    case BaseDtoType.KeyDown:
                    case BaseDtoType.KeyUp:
                    case BaseDtoType.CtrlAltDel:
                    case BaseDtoType.ToggleBlockInput:
                    case BaseDtoType.ClipboardTransfer:
                    case BaseDtoType.KeyPress:
                    case BaseDtoType.SetKeyStatesUp:
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

                switch (baseDto.DtoType)
                {
                    case BaseDtoType.SelectScreen:
                        SelectScreen(message, viewer);
                        break;
                    case BaseDtoType.MouseMove:
                        MouseMove(message, viewer);
                        break;
                    case BaseDtoType.MouseDown:
                        MouseDown(message, viewer);
                        break;
                    case BaseDtoType.MouseUp:
                        MouseUp(message, viewer);
                        break;
                    case BaseDtoType.Tap:
                        Tap(message, viewer);
                        break;
                    case BaseDtoType.MouseWheel:
                        MouseWheel(message);
                        break;
                    case BaseDtoType.KeyDown:
                        KeyDown(message);
                        break;
                    case BaseDtoType.KeyUp:
                        KeyUp(message);
                        break;
                    case BaseDtoType.CtrlAltDel:
                        await viewer.SendCtrlAltDel();
                        break;
                    case BaseDtoType.ToggleAudio:
                        ToggleAudio(message);
                        break;
                    case BaseDtoType.ToggleBlockInput:
                        ToggleBlockInput(message);
                        break;
                    case BaseDtoType.ToggleWebRtcVideo:
                        ToggleWebRtcVideo(message, viewer);
                        break;
                    case BaseDtoType.ClipboardTransfer:
                        await ClipboardTransfer(message);
                        break;
                    case BaseDtoType.KeyPress:
                        await KeyPress(message);
                        break;
                    case BaseDtoType.File:
                        await DownloadFile(message);
                        break;
                    case BaseDtoType.WindowsSessions:
                        await GetWindowsSessions(viewer);
                        break;
                    case BaseDtoType.SetKeyStatesUp:
                        SetKeyStatesUp();
                        break;
                    case BaseDtoType.FrameReceived:
                        HandleFrameReceived(viewer);
                        break;
                    case BaseDtoType.OpenFileTransferWindow:
                        OpenFileTransferWindow(viewer);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private async Task ClipboardTransfer(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<ClipboardTransferDto>(message);
            if (dto.TypeText)
            {
                KeyboardMouseInput.SendText(dto.Text);
            }
            else
            {
                await ClipboardService.SetText(dto.Text);
            }
        }

        private async Task DownloadFile(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<FileDto>(message);
            await FileTransferService.ReceiveFile(dto.Buffer,
                dto.FileName,
                dto.MessageId,
                dto.EndOfFile,
                dto.StartOfFile);
        }

        private async Task GetWindowsSessions(Viewer viewer)
        {
            await viewer.SendWindowsSessions();
        }

        private void HandleFrameReceived(Viewer viewer)
        {
            while (viewer.PendingSentFrames.Count > 0 &&
                !viewer.IsStalled &&
                viewer.IsConnected)
            {
                if (viewer.PendingSentFrames.TryDequeue(out _))
                {
                    break;
                }
            }
        }

        private void KeyDown(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<KeyDownDto>(message);
            KeyboardMouseInput.SendKeyDown(dto.Key);
        }

        private async Task KeyPress(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<KeyPressDto>(message);
            KeyboardMouseInput.SendKeyDown(dto.Key);
            await Task.Delay(1);
            KeyboardMouseInput.SendKeyUp(dto.Key);
        }

        private void KeyUp(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<KeyUpDto>(message);
            KeyboardMouseInput.SendKeyUp(dto.Key);
        }

        private void MouseDown(byte[] message, Viewer viewer)
        {
            var dto = MessagePackSerializer.Deserialize<MouseDownDto>(message);
            KeyboardMouseInput.SendMouseButtonAction(dto.Button, ButtonAction.Down, dto.PercentX, dto.PercentY, viewer);
        }

        private void MouseMove(byte[] message, Viewer viewer)
        {
            var dto = MessagePackSerializer.Deserialize<MouseMoveDto>(message);
            KeyboardMouseInput.SendMouseMove(dto.PercentX, dto.PercentY, viewer);
        }

        private void MouseUp(byte[] message, Viewer viewer)
        {
            var dto = MessagePackSerializer.Deserialize<MouseUpDto>(message);
            KeyboardMouseInput.SendMouseButtonAction(dto.Button, ButtonAction.Up, dto.PercentX, dto.PercentY, viewer);
        }

        private void MouseWheel(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<MouseWheelDto>(message);
            KeyboardMouseInput.SendMouseWheel(-(int)dto.DeltaY);
        }

        private void OpenFileTransferWindow(Viewer viewer)
        {
            FileTransferService.OpenFileTransferWindow(viewer);
        }

        private void SelectScreen(byte[] message, Viewer viewer)
        {
            var dto = MessagePackSerializer.Deserialize<SelectScreenDto>(message);
            viewer.Capturer.SetSelectedScreen(dto.DisplayName);
        }

        private void SetKeyStatesUp()
        {
            KeyboardMouseInput.SetKeyStatesUp();
        }

        private void Tap(byte[] message, Viewer viewer)
        {
            var dto = MessagePackSerializer.Deserialize<TapDto>(message);
            KeyboardMouseInput.SendMouseButtonAction(0, ButtonAction.Down, dto.PercentX, dto.PercentY, viewer);
            KeyboardMouseInput.SendMouseButtonAction(0, ButtonAction.Up, dto.PercentX, dto.PercentY, viewer);
        }

        private void ToggleAudio(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<ToggleAudioDto>(message);
            AudioCapturer.ToggleAudio(dto.ToggleOn);
        }

        private void ToggleBlockInput(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<ToggleBlockInputDto>(message);
            KeyboardMouseInput.ToggleBlockInput(dto.ToggleOn);
        }

        private void ToggleWebRtcVideo(byte[] message, Viewer viewer)
        {
            var dto = MessagePackSerializer.Deserialize<ToggleWebRtcVideoDto>(message);
            viewer.ToggleWebRtcVideo(dto.ToggleOn);
        }
    }
}
