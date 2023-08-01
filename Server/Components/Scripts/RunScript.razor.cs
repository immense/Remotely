using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Hubs;
using Remotely.Server.Migrations.PostgreSql;
using Remotely.Server.Migrations.Sqlite;
using Remotely.Server.Migrations.SqlServer;
using Remotely.Server.Pages;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Scripts;

[Authorize]
public partial class RunScript : AuthComponentBase
{
    private readonly List<string> _selectedDeviceGroups = new();

    private readonly List<string> _selectedDevices = new();

    private DeviceGroup[] _deviceGroups = Array.Empty<DeviceGroup>();

    private Device[] _devices = Array.Empty<Device>();

    private bool _runOnNextConnect = true;

    private SavedScript? _selectedScript;

    [Inject]
    private IDataService DataService { get; init; } = null!;

    [Inject]
    private IJsInterop JsInterop { get; init; } = null!;

    [Inject]
    private IToastService ToastService { get; init; } = null!;

    [Inject]
    private IAgentHubSessionCache ServiceSessionCache { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [CascadingParameter]
    private ScriptsPage ParentPage { get; init; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            JsInterop.AutoHeight();
        }
        base.OnAfterRender(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        EnsureUserSet();
        _deviceGroups = DataService.GetDeviceGroups(UserName);
        _devices = DataService
            .GetDevicesForUser(UserName)
            .OrderBy(x => x.DeviceName)
            .ToArray();
    }

    private void DeviceGroupSelectedChanged(ChangeEventArgs args, DeviceGroup deviceGroup)
    {
        if (args.Value is not bool isSelected)
        {
            return;
        }
        
        if (isSelected)
        {
            _selectedDeviceGroups.Add(deviceGroup.ID);
        }
        else
        {
            _selectedDeviceGroups.RemoveAll(x => x == deviceGroup.ID);
        }
    }

    private void DeviceSelectedChanged(ChangeEventArgs args, Device device)
    {
        if (args.Value is not bool isSelected)
        {
            return;
        }

        if (isSelected)
        {
            _selectedDevices.Add(device.ID);
        }
        else
        {
            _selectedDevices.RemoveAll(x => x == device.ID);
        }
    }

    private async Task ExecuteScript()
    {
        EnsureUserSet();

        if (_selectedScript is null)
        {
            ToastService.ShowToast("You must select a script.", classString: "bg-warning");
            return;
        }

        if (!_selectedDeviceGroups.Any() &&
            !_selectedDevices.Any())
        {
            ToastService.ShowToast("You must select at least one device or device group.", classString: "bg-warning");
            return;
        }

        var deviceIdsFromDeviceGroups = _devices
            .Where(x => _selectedDeviceGroups.Contains(x.DeviceGroupID!))
            .Select(x => x.ID);

        var deviceIds = _selectedDevices
            .Concat(deviceIdsFromDeviceGroups)
            .Distinct()
            .ToArray();

        var filteredDevices = DataService.FilterDeviceIdsByUserPermission(deviceIds.ToArray(), User);

        var onlineDevices = ServiceSessionCache.FilterDevicesByOnlineStatus(filteredDevices, true);

        var scriptRun = new ScriptRun()
        {
            OrganizationID = User.OrganizationID,
            RunAt = Time.Now,
            SavedScriptId = _selectedScript.Id,
            RunOnNextConnect = _runOnNextConnect,
            InputType = ScriptInputType.OneTimeScript,
            Initiator = User.UserName
        };

        if (_runOnNextConnect)
        {
            scriptRun.Devices = DataService.GetDevices(filteredDevices);
        }
        else
        {
            scriptRun.Devices = DataService.GetDevices(onlineDevices);
        }

        await DataService.AddScriptRun(scriptRun);

        ToastService.ShowToast($"Created script run for {scriptRun.Devices.Count} devices.");

        await CircuitConnection.RunScript(onlineDevices, _selectedScript.Id, scriptRun.Id, ScriptInputType.OneTimeScript, false);

        ToastService.ShowToast($"Running script immediately on {onlineDevices.Count()} devices.");
    }

    private async Task ScriptSelected(ScriptTreeNode viewModel)
    {
        EnsureUserSet();

        if (viewModel.Script is not null)
        {
            var scriptResult = await DataService.GetSavedScript(User.Id, viewModel.Script.Id);
            if (scriptResult.IsSuccess)
            {
                _selectedScript = scriptResult.Value;
            }
        }
        else
        {
            _selectedScript = null;
        }
    }
}
