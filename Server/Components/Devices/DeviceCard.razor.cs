﻿using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Remotely.Server.Auth;
using Remotely.Server.Enums;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices
{
    public partial class DeviceCard : AuthComponentBase, IDisposable
    {
        private readonly ConcurrentDictionary<string, double> _fileUploadProgressLookup = new();
        private ElementReference _card;
        private Theme _theme;
        private Version _currentVersion = new();

        [Parameter]
        public Device Device { get; set; }

        [CascadingParameter]
        public DevicesFrame ParentFrame { get; set; }


        [Inject]
        private IClientAppState AppState { get; set; }

        [Inject]
        private ICircuitConnection CircuitConnection { get; set; }

        [Inject]
        private IServiceHubSessionCache ServiceSessionCache { get; init; }

        [Inject]
        private IUpgradeService UpgradeService { get; init; }

        [Inject]
        private IDataService DataService { get; set; }

        private bool IsExpanded => GetCardState() == DeviceCardState.Expanded;

        private bool IsOutdated =>
            Version.TryParse(Device.AgentVersion, out var result) &&
            result < _currentVersion;

        private bool IsSelected => AppState.DevicesFrameSelectedDevices.Contains(Device.ID);

        [Inject]
        private IJsInterop JsInterop { get; set; }

        [Inject]
        private IModalService ModalService { get; set; }
        [Inject]
        private IToastService ToastService { get; set; }

        public void Dispose()
        {
            AppState.PropertyChanged -= AppState_PropertyChanged;
            CircuitConnection.MessageReceived -= CircuitConnection_MessageReceived;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _theme = await AppState.GetEffectiveTheme();
            _currentVersion = UpgradeService.GetCurrentVersion();
            AppState.PropertyChanged += AppState_PropertyChanged;
            CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
        }

        private void AppState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.DevicesFrameFocusedCardState) ||
                e.PropertyName == nameof(AppState.DevicesFrameFocusedDevice) ||
                e.PropertyName == nameof(AppState.DevicesFrameSelectedDevices))
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private void CircuitConnection_MessageReceived(object sender, CircuitEvent e)
        {
           switch (e.EventName)
            {
                case CircuitEventName.DeviceUpdate:
                case CircuitEventName.DeviceWentOffline:
                    {
                        if (e.Params?.FirstOrDefault() is Device device &&
                            device.ID == Device?.ID)
                        {
                            Device = device;
                            InvokeAsync(StateHasChanged);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        private void ContextMenuOpening(MouseEventArgs args)
        {
            if (GetCardState() == DeviceCardState.Normal)
            {
                JsInterop.OpenWindow($"/device-details/{Device.ID}", "_blank");
            }
        }

        private async Task ExpandCard(MouseEventArgs args)
        {
            if (AppState.DevicesFrameFocusedDevice == Device.ID)
            {
                if (AppState.DevicesFrameFocusedCardState == DeviceCardState.Normal)
                {
                    AppState.DevicesFrameFocusedCardState = DeviceCardState.Expanded;
                }
                return;
            }

            AppState.DevicesFrameFocusedDevice = Device.ID;
            AppState.DevicesFrameFocusedCardState = DeviceCardState.Expanded;
            JsInterop.ScrollToElement(_card);

            await CircuitConnection.TriggerHeartbeat(Device.ID);
        }

        private DeviceCardState GetCardState()
        {
            if (AppState.DevicesFrameFocusedDevice == Device.ID)
            {
                return AppState.DevicesFrameFocusedCardState;
            }

            return DeviceCardState.Normal;
        }

        private string GetCardStateClass(Device device)
        {
            if (AppState.DevicesFrameFocusedDevice == device.ID)
            {
                return AppState.DevicesFrameFocusedCardState.ToString().ToLower();
            }

            return string.Empty;
        }

        private string GetProgressMessage(string key)
        {
            if (_fileUploadProgressLookup.TryGetValue(key, out var value))
            {
                return $"{MathHelper.GetFormattedPercent(value)} - {key}";
            }

            return string.Empty;
        }

        private void HandleHeaderClick()
        {
            if (IsExpanded)
            {
                SetCardStateNormal();
            }
        }
        private async Task HandleValidSubmit()
        {
            DataService.UpdateDevice(Device.ID,
                  Device.Tags,
                  Device.Alias,
                  Device.DeviceGroupID,
                  Device.Notes);

            ToastService.ShowToast("Device settings saved.");

            await CircuitConnection.TriggerHeartbeat(Device.ID);
        }

        private async Task OnFileInputChanged(InputFileChangeEventArgs args)
        {
            ToastService.ShowToast("File upload started.");

            var fileId = await DataService.AddSharedFile(args.File, User.OrganizationID, OnFileInputProgress);

            var transferId = Guid.NewGuid().ToString();

            var result = await CircuitConnection.TransferFileFromBrowserToAgent(Device.ID, transferId, new[] { fileId });

            if (!result)
            {
                ToastService.ShowToast("Device not found.", classString: "bg-warning");
            }
            else
            {
                ToastService.ShowToast("File upload completed.");
            }
        }

        private void OnFileInputProgress(double percentComplete, string fileName)
        {
            if (_fileUploadProgressLookup.TryGetValue(fileName, out var existingValue) &&
                percentComplete < 1 &&
                percentComplete - existingValue < .05)
            {
                // Avoid too frequent of updates.
                return;
            }
            _fileUploadProgressLookup.AddOrUpdate(fileName, percentComplete, (k, v) => percentComplete);
            InvokeAsync(StateHasChanged);
        }
        private void OpenDeviceDetails()
        {
            JsInterop.OpenWindow($"/device-details/{Device.ID}", "_blank");
        }

        private void SetCardStateNormal()
        {
            AppState.DevicesFrameFocusedDevice = null;
            AppState.DevicesFrameFocusedCardState = DeviceCardState.Normal;
        }

        private void ShowAllDisks()
        {
            var disksString = JsonSerializer.Serialize(Device.Drives, JsonSerializerHelper.IndentedOptions);

            void modalBody(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.AddMarkupContent(0, $"<div style='white-space: pre'>{disksString}</div>");
            }

            ModalService.ShowModal($"All Disks for {Device.DeviceName}", modalBody);
        }

        private void StartChat()
        {
            var existingSession = AppState.DevicesFrameChatSessions.FirstOrDefault(x => x.DeviceId == Device.ID);
            if (existingSession is null)
            {
                AppState.DevicesFrameChatSessions.Add(new ChatSession()
                {
                    DeviceId = Device.ID,
                    DeviceName = Device.DeviceName,
                    IsExpanded = true
                });
            }
            else
            {
                existingSession.IsExpanded = true;
            }
            AppState.InvokePropertyChanged(nameof(AppState.DevicesFrameChatSessions));
        }

        private void StartRemoteControl(bool viewOnly)
        {
            if (!ServiceSessionCache.TryGetConnectionId(Device.ID, out var connectionId))
            {
                ToastService.ShowToast("Device connection not found", classString: "bg-danger");
                return;
            }

            CircuitConnection.RemoteControl(Device.ID, viewOnly);
        }

        private void ToggleIsSelected(ChangeEventArgs args)
        {
            var isSelected = (bool)args.Value;
            if (isSelected)
            {
                AppState.DevicesFrameSelectedDevices.Add(Device.ID);
            }
            else
            {
                AppState.DevicesFrameSelectedDevices.Remove(Device.ID);
            }
            AppState.InvokePropertyChanged(nameof(AppState.DevicesFrameSelectedDevices));
        }

        private async Task UninstallAgent()
        {
            var result = await JsInterop.Confirm("Are you sure you want to uninstall this agent?  This is permanent!");
            if (result)
            {
                await CircuitConnection.UninstallAgents(new[] { Device.ID });
                AppState.DevicesFrameFocusedDevice = null;
                AppState.DevicesFrameFocusedCardState = DeviceCardState.Normal;
                ParentFrame.Refresh();
            }
        }
    }
}