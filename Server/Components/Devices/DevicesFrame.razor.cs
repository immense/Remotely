using Immense.SimpleMessenger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly SemaphoreSlim _devicesLock = new(1,1);
    private readonly List<Device> _filteredDevices = new();
    private readonly List<Device> _prependedDevices = new();
    private readonly List<PropertyInfo> _sortableProperties = new();
    private int _currentPage = 1;
    private int _devicesPerPage = 25;
    private string? _filter;
    private bool _hideOfflineDevices = true;
    private string _lastFilterState = string.Empty;
    private string? _selectedGroupId;
    private string _selectedSortProperty = "DeviceName";
    private ListSortDirection _sortDirection;

    [Inject]
    private ISelectedCardsStore CardStore { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    private string CurrentFilterState
    {
        get
        {
            return
                $"{_filter}" +
                $"{_selectedGroupId}|" +
                $"{_selectedSortProperty}|" +
                $"{_sortDirection}|" +
                $"{_hideOfflineDevices}|" +
                $"{_currentPage}|" +
                $"{_devicesPerPage}|";
        }
    }
    [Inject]
    private IDataService DataService { get; init; } = null!;

    private Device[] DisplayedDevices => GetDisplayedDevices();

    [Inject]
    private ILogger<DevicesFrame> Logger { get; init; } = null!;

    [Inject]
    private ITerminalStore TerminalStore { get; init; } = null!;

    [Inject]
    private IToastService ToastService { get; init; } = null!;

    private int TotalPages => (int)Math.Max(1, Math.Ceiling((decimal)_filteredDevices.Count / _devicesPerPage));

    public async Task Refresh()
    {
        _lastFilterState = string.Empty;
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

    private void FilterAndSortDevices()
    {
        _filteredDevices.Clear();
        _prependedDevices.Clear();

        foreach (var device in _allDevices)
        {
            if (CardStore.SelectedDevices.Contains(device.ID))
            {
                _prependedDevices.Add(device);
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
    }

    private Device[] GetDisplayedDevices()
    {
        _devicesLock.Wait();
        try
        {
            if (CurrentFilterState != _lastFilterState)
            {
                _lastFilterState = CurrentFilterState;
                FilterAndSortDevices();
            }

            var skipCount = (_currentPage - 1) * _devicesPerPage;
            var devicesForPage = _filteredDevices
                .Except(_prependedDevices)
                .Skip(skipCount)
                .Take(_devicesPerPage);

            return _prependedDevices.Concat(devicesForPage).ToArray();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while filtering devices.");
            ToastService.ShowToast2("Filter devices failed", ToastType.Error);
            return Array.Empty<Device>();
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

    private async Task HandleDeviceStateChangedMessage(object subscriber, DeviceStateChangedMessage message)
    {
        await _devicesLock.WaitAsync();

        try
        {
            var device = message.Device;

            var collections = new[] { _allDevices, _filteredDevices };

            foreach (var collection in collections)
            {
                var index = collection.FindIndex(x => x.ID == device.ID);
                if (index > -1)
                {
                    collection[index] = device;
                }
                else
                {
                    collection.Add(device);
                }
            }

            Debouncer.Debounce(
                   TimeSpan.FromSeconds(2),
                   async () =>
                   {
                       await InvokeAsync(StateHasChanged);
                   });
        }
        finally
        {
            _devicesLock.Release();
        }
    }

    private async Task HandleDisplayNotificationMessage(object subscriber, DisplayNotificationMessage message)
    {
        TerminalStore.AddTerminalLine(message.ConsoleText);
        ToastService.ShowToast(message.ToastText, classString: message.ClassName);
        await InvokeAsync(StateHasChanged);
    }
    private async Task HandleRefreshClicked()
    {
        await Refresh();
        ToastService.ShowToast("Devices refreshed.");
    }

    private async Task HandleScriptResultMessage(object subscriber, ScriptResultMessage message)
    {
        await AddScriptResult(message.ScriptResult);
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
