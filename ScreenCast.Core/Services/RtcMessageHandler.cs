using MessagePack;
using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Models;
using Remotely.Shared.Enums;
using Remotely.Shared.Models.RtcDtos;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Services
{
    public interface IRtcMessageHandler
    {
        Task ParseMessage(byte[] message);
    }
    public class RtcMessageHandler : IRtcMessageHandler
    {
        public RtcMessageHandler(Viewer viewer,
            CasterSocket casterSocket,
            IKeyboardMouseInput keyboardMouseInput,
            IAudioCapturer audioCapturer,
            IClipboardService clipboardService,
            IFileTransferService fileDownloadService)
        {
            Viewer = viewer;
            CasterSocket = casterSocket;
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ClipboardService = clipboardService;
            FileDownloadService = fileDownloadService;
        }

        private IAudioCapturer AudioCapturer { get; }
        private CasterSocket CasterSocket { get; }
        private IClipboardService ClipboardService { get; }
        private IFileTransferService FileDownloadService { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        private Viewer Viewer { get; }
        public async Task ParseMessage(byte[] message)
        {
            try
            {
                var baseDto = MessagePackSerializer.Deserialize<BinaryDtoBase>(message);

                switch (baseDto.DtoType)
                {
                    case BinaryDtoType.MouseMove:
                    case BinaryDtoType.MouseDown:
                    case BinaryDtoType.MouseUp:
                    case BinaryDtoType.Tap:
                    case BinaryDtoType.MouseWheel:
                    case BinaryDtoType.KeyDown:
                    case BinaryDtoType.KeyUp:
                    case BinaryDtoType.CtrlAltDel:
                    case BinaryDtoType.ToggleBlockInput:
                    case BinaryDtoType.ClipboardTransfer:
                    case BinaryDtoType.KeyPress:
                    case BinaryDtoType.SetKeyStatesUp:
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
                    case BinaryDtoType.SelectScreen:
                        SelectScreen(message);
                        break;
                    case BinaryDtoType.MouseMove:
                        MouseMove(message);
                        break;
                    case BinaryDtoType.MouseDown:
                        MouseDown(message);
                        break;
                    case BinaryDtoType.MouseUp:
                        MouseUp(message);
                        break;
                    case BinaryDtoType.Tap:
                        Tap(message);
                        break;
                    case BinaryDtoType.MouseWheel:
                        MouseWheel(message);
                        break;
                    case BinaryDtoType.KeyDown:
                        KeyDown(message);
                        break;
                    case BinaryDtoType.KeyUp:
                        KeyUp(message);
                        break;
                    case BinaryDtoType.CtrlAltDel:
                        await CasterSocket.SendCtrlAltDel();
                        break;
                    case BinaryDtoType.AutoQualityAdjust:
                        SetAutoQualityAdjust(message);
                        break;
                    case BinaryDtoType.ToggleAudio:
                        ToggleAudio(message);
                        break;
                    case BinaryDtoType.ToggleBlockInput:
                        ToggleBlockInput(message);
                        break;
                    case BinaryDtoType.ClipboardTransfer:
                        ClipboardTransfer(message);
                        break;
                    case BinaryDtoType.KeyPress:
                        await KeyPress(message);
                        break;
                    case BinaryDtoType.QualityChange:
                        QualityChange(message);
                        break;
                    case BinaryDtoType.File:
                        await DownloadFile(message);
                        break;
                    case BinaryDtoType.WindowsSessions:
                        await GetWindowsSessions();
                        break;
                    case BinaryDtoType.SetKeyStatesUp:
                        SetKeyStatesUp();
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

        private void ClipboardTransfer(byte[] message)
        {
            var dto = MessagePackSerializer.Deserialize<ClipboardTransferDto>(message);
            if (dto.TypeText)
            {
                KeyboardMouseInput.SendText(dto.Text, Viewer);
            }
            else
            {
                ClipboardService.SetText(dto.Text);
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
    }
}
