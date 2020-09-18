using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using Remotely.Shared.Enums;
using Remotely.Shared.Models.RemoteControlDtos;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
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
            IFileTransferService fileDownloadService)
        {
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ClipboardService = clipboardService;
            FileDownloadService = fileDownloadService;
        }

        private IAudioCapturer AudioCapturer { get; }
        private IClipboardService ClipboardService { get; }
        private IFileTransferService FileDownloadService { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        private Viewer Viewer { get; set; }
        public async Task ParseMessage(Viewer viewer, byte[] message)
        {
            try
            {
                // TODO: Remove property and pass viewer into methods.
                Viewer = viewer;
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
                            if (!Viewer.HasControl)
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
                        SelectScreen(message);
                        break;
                    case BaseDtoType.MouseMove:
                        MouseMove(message);
                        break;
                    case BaseDtoType.MouseDown:
                        MouseDown(message);
                        break;
                    case BaseDtoType.MouseUp:
                        MouseUp(message);
                        break;
                    case BaseDtoType.Tap:
                        Tap(message);
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
                        await Viewer.SendCtrlAltDel();
                        break;
                    case BaseDtoType.AutoQualityAdjust:
                        SetAutoQualityAdjust(message);
                        break;
                    case BaseDtoType.ToggleAudio:
                        ToggleAudio(message);
                        break;
                    case BaseDtoType.ToggleBlockInput:
                        ToggleBlockInput(message);
                        break;
                    case BaseDtoType.ToggleWebRtcVideo:
                        ToggleWebRtcVideo(message);
                        break;
                    case BaseDtoType.ClipboardTransfer:
                        await ClipboardTransfer(message);
                        break;
                    case BaseDtoType.KeyPress:
                        await KeyPress(message);
                        break;
                    case BaseDtoType.QualityChange:
                        QualityChange(message);
                        break;
                    case BaseDtoType.File:
                        await DownloadFile(message);
                        break;
                    case BaseDtoType.WindowsSessions:
                        await GetWindowsSessions();
                        break;
                    case BaseDtoType.SetKeyStatesUp:
                        SetKeyStatesUp();
                        break;
                    case BaseDtoType.FrameReceived:
                        HandleFrameReceived();
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
                KeyboardMouseInput.SendText(dto.Text, Viewer);
            }
            else
            {
                await ClipboardService.SetText(dto.Text);
            }
        }

        private async Task DownloadFile(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<FileDto>(message);
            await FileDownloadService.ReceiveFile(dto.Buffer,
                dto.FileName,
                dto.MessageId,
                dto.EndOfFile,
                dto.StartOfFile);
        }

        private async Task GetWindowsSessions()
        {
            await Viewer.SendWindowsSessions();
        }

        private void HandleFrameReceived()
        {
            for (int i = 0; i < 5; i++)
            {
                if (Viewer.PendingSentFrames.TryDequeue(out _))
                {
                    break;
                }
            }
        }

        private void KeyDown(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<KeyDownDto>(message);
            KeyboardMouseInput.SendKeyDown(dto.Key, Viewer);
        }

        private async Task KeyPress(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<KeyPressDto>(message);
            KeyboardMouseInput.SendKeyDown(dto.Key, Viewer);
            await Task.Delay(1);
            KeyboardMouseInput.SendKeyUp(dto.Key, Viewer);
        }

        private void KeyUp(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<KeyUpDto>(message);
            KeyboardMouseInput.SendKeyUp(dto.Key, Viewer);
        }

        private void MouseDown(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<MouseDownDto>(message);
            if (dto.Button == 0)
            {
                KeyboardMouseInput.SendLeftMouseDown(dto.PercentX, dto.PercentY, Viewer);
            }
            else if (dto.Button == 2)
            {
                KeyboardMouseInput.SendRightMouseDown(dto.PercentX, dto.PercentY, Viewer);
            }
        }

        private void MouseMove(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<MouseMoveDto>(message);
            KeyboardMouseInput.SendMouseMove(dto.PercentX, dto.PercentY, Viewer);
        }

        private void MouseUp(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<MouseUpDto>(message);
            if (dto.Button == 0)
            {
                KeyboardMouseInput.SendLeftMouseUp(dto.PercentX, dto.PercentY, Viewer);
            }
            else if (dto.Button == 2)
            {
                KeyboardMouseInput.SendRightMouseUp(dto.PercentX, dto.PercentY, Viewer);
            }
        }

        private void MouseWheel(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<MouseWheelDto>(message);
            KeyboardMouseInput.SendMouseWheel(-(int)dto.DeltaY, Viewer);
        }

        private void QualityChange(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<QualityChangeDto>(message);
            Viewer.ImageQuality = dto.QualityLevel;
        }

        private void SelectScreen(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<SelectScreenDto>(message);
            Viewer.Capturer.SetSelectedScreen(dto.DisplayName);
        }

        private void SetAutoQualityAdjust(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<AutoQualityAdjustDto>(message);
            Viewer.AutoAdjustQuality = dto.IsOn;
        }

        private void SetKeyStatesUp()
        {
            KeyboardMouseInput.SetKeyStatesUp();
        }

        private void Tap(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<TapDto>(message);
            KeyboardMouseInput.SendLeftMouseDown(dto.PercentX, dto.PercentY, Viewer);
            KeyboardMouseInput.SendLeftMouseUp(dto.PercentX, dto.PercentY, Viewer);
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

        private void ToggleWebRtcVideo(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<ToggleWebRtcVideoDto>(message);
            Viewer.ToggleWebRtcVideo(dto.ToggleOn);
        }
    }
}
