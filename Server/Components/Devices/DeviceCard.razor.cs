using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Remotely.Server.Enums;
using Remotely.Server.Hubs;
using Remotely.Server.Models.Messages;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using Remotely.Shared.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices;

public partial class DeviceCard : AuthComponentBase, IDisposable
{
    private readonly ConcurrentDictionary<string, double> _fileUploadProgressLookup = new();
    private ElementReference _card;
    private Version _currentVersion = new();
    private Theme _theme;
    private DeviceGroup[] _deviceGroups = Array.Empty<DeviceGroup>();

    [Parameter]
    public Device Device { get; set; } = null!;

    [CascadingParameter]
    public DevicesFrame ParentFrame { get; init; } = null!;


    [Inject]
    private IClientAppState AppState { get; init; } = null!;

    [Inject]
    private IThemeProvider ThemeProvider { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [Inject]
    private IDataService DataService { get; init; } = null!;

    [Inject]
    private IChatSessionCache ChatCache { get; init; } = null!;

    private bool IsExpanded => GetCardState() == DeviceCardState.Expanded;

    private bool IsOutdated =>
        Version.TryParse(Device.AgentVersion, out var result) &&
        result < _currentVersion;

    private bool IsSelected => AppState.DevicesFrameSelectedDevices.Contains(Device.ID);

    [Inject]
    private IJsInterop JsInterop { get; init; } = null!;

    [Inject]
    private IModalService ModalService { get; init; } = null!;

    [Inject]
    private IAgentHubSessionCache ServiceSessionCache { get; init; } = null!;

    [Inject]
    private IToastService ToastService { get; init; } = null!;

    [Inject]
    private IUpgradeService UpgradeService { get; init; } = null!;

    [Inject]
    private IMessenger Messenger { get; init; } = null!;

    public void Dispose()
    {
        AppState.PropertyChanged -= AppState_PropertyChanged;
        Messenger.Unregister<DeviceStateChangedMessage, string>(this, CircuitConnection.ConnectionId);
        GC.SuppressFinalize(this);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        EnsureUserSet();
        _theme = await ThemeProvider.GetEffectiveTheme();
        _currentVersion = UpgradeService.GetCurrentVersion();
        _deviceGroups = DataService.GetDeviceGroups(UserName);
        AppState.PropertyChanged += AppState_PropertyChanged;
        await Messenger.Register<DeviceStateChangedMessage, string>(
            this, 
            CircuitConnection.ConnectionId,
            HandleDeviceStateChanged);
    }

    private async Task HandleDeviceStateChanged(DeviceStateChangedMessage message)
    {
        if (message.Device.ID != Device.ID)
        {
            return;
        }

        // TODO: It would be cool to decorate user-editable properties
        // with a "UserEditable" attribute, then use a source generator
        // to create/update a method that copies property values for
        // those that do not have the attribute.  We could do the same
        // with reflection, but this method is called too frequently,
        // and the performance hit would likely be significant.

        // If the card is expanded, only update the immutable UI
        // elements, so any changes to the form fields aren't lost.
        if (IsExpanded)
        {
            Device.CurrentUser = message.Device.CurrentUser;
            Device.Platform = message.Device.Platform;
            Device.TotalStorage = message.Device.TotalStorage;
            Device.UsedStorage = message.Device.UsedStorage;
            Device.Drives = message.Device.Drives;
            Device.CpuUtilization = message.Device.CpuUtilization;
            Device.TotalMemory = message.Device.TotalMemory;
            Device.UsedMemory = message.Device.UsedMemory;
            Device.AgentVersion = message.Device.AgentVersion;
            Device.LastOnline = message.Device.LastOnline;
            Device.PublicIP = message.Device.PublicIP;
            Device.MacAddresses = message.Device.MacAddresses;
        }
        else
        {
            Device = message.Device;
        }
        await InvokeAsync(StateHasChanged);
    }

    private void AppState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppState.DevicesFrameFocusedCardState) ||
            e.PropertyName == nameof(AppState.DevicesFrameFocusedDevice) ||
            e.PropertyName == nameof(AppState.DevicesFrameSelectedDevices))
        {
            InvokeAsync(StateHasChanged);
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
        EnsureUserSet();
        if (!DataService.DoesUserHaveAccessToDevice(Device.ID, User))
        {
            ToastService.ShowToast("Unauthorized.", classString: "bg-warning");
            return;
        }
       
        await DataService.UpdateDevice(Device.ID,
              Device.Tags,
              Device.Alias,
              Device.DeviceGroupID,
              Device.Notes);

        ToastService.ShowToast("Device settings saved.");

        await CircuitConnection.TriggerHeartbeat(Device.ID);
    }

    private async Task OnFileInputChanged(InputFileChangeEventArgs args)
    {
        EnsureUserSet();

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
        var session = ChatCache.GetOrAdd(Device.ID, key =>
        {
            return new ChatSession()
            {
                DeviceId = key,
                DeviceName = Device.DeviceName,
                IsExpanded = true
            };
        });

        session.IsExpanded = true;
        Messenger.Send(new ChatSessionsChangedMessage(), CircuitConnection.ConnectionId);
    }

    private async Task StartRemoteControl(bool viewOnly)
    {
        if (!ServiceSessionCache.TryGetConnectionId(Device.ID, out _))
        {
            ToastService.ShowToast("Device connection not found", classString: "bg-danger");
            return;
        }

        var result = await CircuitConnection.RemoteControl(Device.ID, viewOnly);
        if (!result.IsSuccess)
        {
            return;
        }

        var session = result.Value;

        if (!await session.WaitForSessionReady(TimeSpan.FromSeconds(20)))
        {
            ToastService.ShowToast("Session failed to start", classString: "bg-danger");
            return;
        }

        JsInterop.OpenWindow(
            $"/RemoteControl/Viewer" +
                $"?mode=Unattended&sessionId={session.UnattendedSessionId}" +
                $"&accessKey={session.AccessKey}" +
                $"&viewonly={viewOnly}", 
            "_blank");
    }

    private void ToggleIsSelected(ChangeEventArgs args)
    {
        if (args.Value is not bool isSelected)
        {
            return;
        }

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
            await ParentFrame.Refresh();
        }
    }

    private async Task WakeDevice()
    {
        var result = await CircuitConnection.WakeDevice(Device);
        if (result.IsSuccess)
        {
            ToastService.ShowToast2(
                $"Wake command sent to peer devices.", 
                ToastType.Success);
        }
        else
        {
            ToastService.ShowToast2(
                $"Wake command failed.  Reason: {result.Reason}", 
                ToastType.Error);
        }
    }
}