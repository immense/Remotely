using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Remotely.Server.Components.Pages;
using Remotely.Server.Enums;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Scripts;

[Authorize]
public partial class ScriptSchedules : AuthComponentBase
{
    private readonly List<string> _selectedDeviceGroups = new();

    private readonly List<string> _selectedDevices = new();

    private readonly List<ScriptSchedule> _schedules = new();

    private string _alertMessage = string.Empty;

    private DeviceGroup[] _deviceGroups = Array.Empty<DeviceGroup>();

    private Device[] _devices = Array.Empty<Device>();

    private SavedScript? _selectedScript;

    private ScriptSchedule _selectedSchedule = new()
    {
        Name = string.Empty,
        StartAt = Time.Now 
    };

    [CascadingParameter]
    private ScriptsPage ParentPage { get; set; } = null!;

    [Inject]
    private IDataService DataService { get; set; } = null!;

    [Inject]

    private IJsInterop JsInterop { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    public required ILogger<ScriptSchedules> Logger { get; set; }

    private bool CanModifySchedule
    {
        get
        {
            EnsureUserSet();
            return 
                _selectedSchedule.CreatorId == User.Id ||
                User.IsAdministrator;
        }
    }

    private bool CanDeleteSchedule
    {
        get
        {
            EnsureUserSet();
            return 
                _selectedSchedule.CreatorId == User.Id ||
                User.IsAdministrator;
        }
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

        await RefreshSchedules();
    }

    private void CreateNew()
    {
        _selectedScript = new()
        {
            Name = string.Empty
        };
        _selectedSchedule = new()
        { 
            Name = string.Empty,
            StartAt = Time.Now 
        };
        _selectedDeviceGroups.Clear();
        _selectedDevices.Clear();
    }

    private async Task DeleteSelectedSchedule()
    {
        try
        {
            if (User?.Id != _selectedSchedule.CreatorId)
            {
                ToastService.ShowToast("You can't delete other people's script schedules.", classString: "bg-warning");
                return;
            }

            var result = await JsInterop.Confirm($"Are you sure you want to delete the schedule {_selectedSchedule.Name}?");
            if (result)
            {
                await DataService.DeleteScriptSchedule(_selectedSchedule.Id);
                ToastService.ShowToast("Schedule deleted.");
                _alertMessage = "Schedule deleted.";
                CreateNew();
                await ParentPage.RefreshScripts();
                await RefreshSchedules();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while deleting script schedule.");
            ToastService.ShowToast2("Failed to delete schedule", ToastType.Error);
        }
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

    private async Task OnValidSubmit(EditContext context)
    {
        EnsureUserSet();

        if (_selectedSchedule is null)
        {
            return;
        }

        if (_selectedScript is null)
        {
            ToastService.ShowToast("You must select a script to run.", classString: "bg-warning");
            return;
        }

        if (!CanModifySchedule)
        {
            ToastService.ShowToast("You can't modify other people's schedules.", classString: "bg-warning");
            return;
        }

        if (!_selectedDevices.Any() && !_selectedDeviceGroups.Any())
        {
            ToastService.ShowToast("You must select at least one device or device group.", classString: "bg-warning");
            return;
        }

        _selectedSchedule.SavedScriptId = _selectedScript.Id;

        if (string.IsNullOrWhiteSpace(_selectedSchedule.CreatorId))
        {
            _selectedSchedule.CreatedAt = Time.Now;
            _selectedSchedule.CreatorId = User.Id;
        }

        _selectedSchedule.OrganizationID = User.OrganizationID;
        _selectedSchedule.NextRun = _selectedSchedule.StartAt;

        _selectedSchedule.Devices = _devices.Where(x => _selectedDevices.Contains(x.ID)).ToList();
        _selectedSchedule.DeviceGroups = _deviceGroups.Where(x => _selectedDeviceGroups.Contains(x.ID)).ToList();

        await DataService.AddOrUpdateScriptSchedule(_selectedSchedule);
        CreateNew();
        await RefreshSchedules();
        ToastService.ShowToast("Schedule saved.");
        _alertMessage = "Schedule saved.";
    }

    private async Task RefreshSchedules()
    {
        _schedules.Clear();
        if (User is not null)
        {
            _schedules.AddRange(await DataService.GetScriptSchedules(User.OrganizationID));
        }
    }

    private string GetTableRowClass(ScriptSchedule schedule)
    {
        if (schedule?.Id == _selectedSchedule?.Id)
        {
            return "table-primary";
        }
        return string.Empty;
    }

    private async Task SelectTableRow(ScriptSchedule schedule)
    {
        _selectedSchedule = schedule;
        _selectedDevices.Clear();
        _selectedDeviceGroups.Clear();

        if (schedule?.Devices?.Any() == true)
        {
            _selectedDevices.AddRange(schedule.Devices.Select(x => x.ID));
        }
        if (schedule?.DeviceGroups?.Any() == true)
        {
            _selectedDeviceGroups.AddRange(schedule.DeviceGroups.Select(x => x.ID));
        }

        var result = await DataService.GetSavedScript(_selectedSchedule.SavedScriptId);
        if (result.IsSuccess)
        {
            _selectedScript = result.Value;
        }
    }

    private async Task ScriptSelected(ScriptTreeNode viewModel)
    {
        EnsureUserSet();

        if (viewModel.Script is not null)
        {
            var result = await DataService.GetSavedScript(User.Id, viewModel.Script.Id);
            if (result.IsSuccess)
            {
                _selectedScript = result.Value;
            }

        }
        else
        {
            _selectedScript = null;
        }
    }
}
