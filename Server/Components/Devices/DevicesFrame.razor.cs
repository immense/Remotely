using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Remotely.Server.Enums;
using Remotely.Server.Hubs;
using Remotely.Server.Models.Messages;
using Remotely.Server.Services;
using Remotely.Server.Services.Stores;
using Remotely.Shared.Attributes;
using Remotely.Shared.Entities;
using Remotely.Shared.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices;

[Authorize]
public partial class DevicesFrame : AuthComponentBase
{
    private readonly List<Device> _allDevices = new();
    private readonly string _deviceGroupAll = Guid.NewGuid().ToString();
    private readonly string _deviceGroupNone = Guid.NewGuid().ToString();
    private readonly List<DeviceGroup> _deviceGroups = new();
    private readonly List<Device> _devicesForPage = new();
    private readonly SemaphoreSlim _devicesLock = new(1,1);
    private readonly List<Device> _filteredDevices = new();
    private readonly List<PropertyInfo> _sortableProperties = new();
    private int _currentPage = 1;
    private int _devicesPerPage = 25;
    private string? _filter;
    private bool _hideOfflineDevices = true;
    private string? _selectedGroupId;
    private string _selectedSortProperty = "DeviceName";
    private ListSortDirection _sortDirection;

    [Inject]
    private ISelectedCardsStore CardStore { get; init; } = null!;

    [Inject]
    private ITerminalStore TerminalStore { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [Inject]
    private IDataService DataService { get; init; } = null!;

    [Inject]
    private IToastService ToastService { get; init; } = null!;

    private int TotalPages => (int)Math.Max(1, Math.Ceiling((decimal)_filteredDevices.Count / _devicesPerPage));

    private async Task HandleDisplayNotificationMessage(DisplayNotificationMessage message)
    {
        TerminalStore.AddTerminalLine(message.ConsoleText);
        ToastService.ShowToast(message.ToastText, classString: message.ClassName);
        await InvokeAsync(StateHasChanged);
    }

    public async Task Refresh()
    {
        await LoadDevices();
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        EnsureUserSet();

        await Register<DisplayNotificationMessage, string>(
            CircuitConnection.ConnectionId,
            HandleDisplayNotificationMessage);

        await Register<DeviceStateChangedMessage, string>(
            CircuitConnection.ConnectionId,
            HandleDeviceStateChangedMessage);

        await Register<ScriptResultMessage, string>(
            CircuitConnection.ConnectionId,
            HandleScriptResultMessage);

        _deviceGroups.Clear();

        _deviceGroups.AddRange(DataService.GetDeviceGroups(UserName));

        _selectedGroupId = _deviceGroupAll;

        var sortableProperties = typeof(Device)
            .GetProperties()
            .Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SortableAttribute)));

        _sortableProperties.AddRange(sortableProperties);

        await LoadDevices();
    }

    private async Task HandleScriptResultMessage(ScriptResultMessage message)
    {
        await AddScriptResult(message.ScriptResult);
    }

    private async Task HandleDeviceStateChangedMessage(DeviceStateChangedMessage message)
    {
        await _devicesLock.WaitAsync();

        try
        {
            var device = message.Device;

            foreach (var collection in new[] { _allDevices, _devicesForPage })
            {
                var index = collection.FindIndex(x => x.ID == device.ID);
                if (index > -1)
                {
                    collection[index] = device;
                }
            }

            Debouncer.Debounce(TimeSpan.FromSeconds(2), Refresh);
        }
        finally
        {
            _devicesLock.Release();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await FilterDevices();
    }

    private async Task AddScriptResult(ScriptResult result)
    {
        var deviceResult = await DataService.GetDevice(result.DeviceID);
        if (!deviceResult.IsSuccess)
        {
            return;
        }

        TerminalStore.AddTerminalLine($"{deviceResult.Value.DeviceName} @ {result.TimeStamp}", "font-weight-bold");

        var stdOut = result.StandardOutput ?? Array.Empty<string>();
        var stdErr = result.ErrorOutput ?? Array.Empty<string>();

        foreach (var line in stdOut)
        {
            TerminalStore.AddTerminalLine(line, "text-info");
        }
        foreach (var line in stdErr)
        {
            TerminalStore.AddTerminalLine(line, "text-danger");
        }
        TerminalStore.InvokeLinesChanged();
    }

    private async Task ClearSelectedCard()
    {
        await Messenger.Send(
            new DeviceCardStateChangedMessage(string.Empty, DeviceCardState.Normal), 
            CircuitConnection.ConnectionId);
    }

    private async Task FilterDevices()
    {
        await _devicesLock.WaitAsync();
        try
        {
            _filteredDevices.Clear();
            var appendDevices = new List<Device>();

            foreach (var device in _allDevices)
            {
                if (CardStore.SelectedDevices.Contains(device.ID))
                {
                    appendDevices.Add(device);
                }

                if (!device.IsOnline && _hideOfflineDevices)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(_filter) &&
                        device.Alias?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        device.CurrentUser?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        device.DeviceName?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        device.Notes?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        device.Platform?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        device.Tags?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true)
                {
                    continue;
                }

                if (_selectedGroupId == _deviceGroupAll ||
                    _selectedGroupId == device.DeviceGroupID ||
                    (
                        _selectedGroupId == _deviceGroupNone && 
                        string.IsNullOrWhiteSpace(device.DeviceGroupID
                    )))
                {
                    _filteredDevices.Add(device);
                }
            }

            if (!string.IsNullOrWhiteSpace(_selectedSortProperty))
            {
                var direction = _sortDirection == ListSortDirection.Ascending ? 1 : -1;
                _filteredDevices.Sort((a, b) =>
                {
                    if (a.IsOnline != b.IsOnline)
                    {
                        return b.IsOnline.CompareTo(a.IsOnline);
                    }

                    var propInfo = _sortableProperties.Find(x => x.Name == _selectedSortProperty);

                    var valueA = propInfo?.GetValue(a);
                    var valueB = propInfo?.GetValue(b);

                    return Comparer.Default.Compare(valueA, valueB) * direction;
                });
            }

            var skipCount = (_currentPage - 1) * _devicesPerPage;
            var devicesForPage = _filteredDevices
                .Except(appendDevices)
                .Skip(skipCount)
                .Take(_devicesPerPage);

            _devicesForPage.Clear();
            _devicesForPage.AddRange(appendDevices.Concat(devicesForPage));

        }
        finally
        {
            _devicesLock.Release();
        }
    }


    private string GetDisplayName(PropertyInfo propInfo)
    {
        return propInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propInfo.Name;
    }

    private string GetSortIcon()
    {
        return $"oi-sort-{_sortDirection.ToString().ToLower()}";
    }

    private async Task HandleRefreshClicked()
    {
        await Refresh();
        ToastService.ShowToast("Devices refreshed.");
    }

    private async Task LoadDevices()
    {
        EnsureUserSet();

        await _devicesLock.WaitAsync();
        try
        {
            _allDevices.Clear();

            var devices = DataService.GetDevicesForUser(UserName)
                .OrderByDescending(x => x.IsOnline)
                .ToList();

            _allDevices.AddRange(devices);
        }
        finally
        {
            _devicesLock.Release();
        }

        await FilterDevices();
    }
    private void PageDown()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
        }
    }

    private void PageUp()
    {
        if (_currentPage < TotalPages)
        {
            _currentPage++;
        }
    }

    private void SelectAllCards()
    {
        if (CardStore.SelectedDevices.Any())
        {
            CardStore.Clear();
        }
        else
        {
            foreach (var device in _filteredDevices)
            {
                _ = CardStore.Add(device.ID);
            };
        }

        CardStore.InvokeSelectionsChanged();
    }

    private void ToggleSortDirection()
    {
        if (_sortDirection == ListSortDirection.Ascending)
        {
            _sortDirection = ListSortDirection.Descending;
        }
        else
        {
            _sortDirection = ListSortDirection.Ascending;
        }
    }

    private async Task WakeDevices()
    {
        EnsureUserSet();

        var offlineDevices = DataService
           .GetDevicesForUser(UserName)
           .Where(x => !x.IsOnline);

        if (_selectedGroupId == _deviceGroupNone)
        {
            offlineDevices = offlineDevices.Where(x => x.DeviceGroupID is null);
        }

        if (_selectedGroupId != _deviceGroupAll)
        {
            offlineDevices = offlineDevices.Where(x => x.DeviceGroupID == _selectedGroupId);
        }

        var result = await CircuitConnection.WakeDevices(offlineDevices.ToArray());

        if (result.IsSuccess)
        {
            ToastService.ShowToast2(
                $"Wake commands sent to peer devices.", 
                ToastType.Success);
        }
        else
        {
            ToastService.ShowToast2(
                $"Failed to send wake commands.  Reason: {result.Reason}", 
                ToastType.Error);
        }
    }
}
