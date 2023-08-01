using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remotely.Server.Enums;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Attributes;
using Remotely.Shared.Entities;
using Remotely.Shared.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices;

[Authorize]
public partial class DevicesFrame : AuthComponentBase, IDisposable
{
    private readonly List<Device> _allDevices = new();
    private readonly string _deviceGroupAll = Guid.NewGuid().ToString();
    private readonly string _deviceGroupNone = Guid.NewGuid().ToString();
    private readonly List<DeviceGroup> _deviceGroups = new();
    private readonly List<Device> _devicesForPage = new();
    private readonly object _devicesLock = new();
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
    private IClientAppState AppState { get; init; } = null!;

    [Inject]
    private ICircuitConnection CircuitConnection { get; init; } = null!;

    [Inject]
    private IDataService DataService { get; init; } = null!;

    [Inject]
    private IJsInterop JsInterop { get; init; } = null!;

    [Inject]
    private IToastService ToastService { get; init; } = null!;

    private int TotalPages => (int)Math.Max(1, Math.Ceiling((decimal)_filteredDevices.Count / _devicesPerPage));

    public void Dispose()
    {
        CircuitConnection.MessageReceived -= CircuitConnection_MessageReceived;
        AppState.PropertyChanged -= AppState_PropertyChanged;
        GC.SuppressFinalize(this);
    }

    public void Refresh()
    {
        LoadDevices();
        InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        EnsureUserSet();

        CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
        AppState.PropertyChanged += AppState_PropertyChanged;

        _deviceGroups.Clear();

        _deviceGroups.AddRange(DataService.GetDeviceGroups(UserName));

        _selectedGroupId = _deviceGroupAll;

        var sortableProperties = typeof(Device)
            .GetProperties()
            .Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SortableAttribute)));

        _sortableProperties.AddRange(sortableProperties);

        LoadDevices();
    }

    protected override bool ShouldRender()
    {
        var shouldRender = base.ShouldRender();
        if (shouldRender)
        {
            FilterDevices();
        }
        return shouldRender;
    }

    private async Task AddScriptResult(ScriptResult result)
    {
        var deviceResult = await DataService.GetDevice(result.DeviceID);
        if (!deviceResult.IsSuccess)
        {
            return;
        }

        AppState.AddTerminalLine($"{deviceResult.Value.DeviceName} @ {result.TimeStamp}", "font-weight-bold");

        var stdOut = result.StandardOutput ?? Array.Empty<string>();
        var stdErr = result.ErrorOutput ?? Array.Empty<string>();

        foreach (var line in stdOut)
        {
            AppState.AddTerminalLine(line, "text-info");
        }
        foreach (var line in stdErr)
        {
            AppState.AddTerminalLine(line, "text-danger");
        }
        AppState.InvokePropertyChanged(nameof(AppState.TerminalLines));
    }

    private void AppState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppState.DevicesFrameFocusedCardState) ||
            e.PropertyName == nameof(AppState.DevicesFrameFocusedDevice) ||
            e.PropertyName == nameof(AppState.DevicesFrameSelectedDevices))
        {
            InvokeAsync(StateHasChanged);
        }
    }

    private async void CircuitConnection_MessageReceived(object? sender, CircuitEvent args)
    {
        switch (args.EventName)
        {
            case CircuitEventName.DeviceUpdate:
            case CircuitEventName.DeviceWentOffline:
                {
                    if (args.Params?.FirstOrDefault() is Device device)
                    {
                        lock (_devicesLock)
                        {
                            var index = _allDevices.FindIndex(x => x.ID == device.ID);
                            if (index > -1)
                            {
                                _allDevices[index] = device;
                            }

                            index = _devicesForPage.FindIndex(x => x.ID == device.ID);
                            if (index > -1)
                            {
                                _devicesForPage[index] = device;
                            }
                        }
                        Debouncer.Debounce(TimeSpan.FromSeconds(2), Refresh);
                    }
                    break;
                }
            case CircuitEventName.DisplayMessage:
                {
                    var terminalMessage = (string)args.Params[0];
                    var toastMessage = (string)args.Params[1];
                    var className = (string)args.Params[2];
                    AppState.AddTerminalLine(terminalMessage);
                    ToastService.ShowToast(toastMessage, classString: className);
                    await InvokeAsync(StateHasChanged);
                }
                break;
            case CircuitEventName.ScriptResult:
                {
                    if (args.Params[0] is ScriptResult result)
                    {
                        await AddScriptResult(result);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void ClearSelectedCard()
    {
        AppState.DevicesFrameFocusedDevice = string.Empty;
        AppState.DevicesFrameFocusedCardState = DeviceCardState.Normal;
    }

    private void FilterDevices()
    {
        lock (_devicesLock)
        {
            _filteredDevices.Clear();
            var appendDevices = new List<Device>();

            foreach (var device in _allDevices)
            {
                if (AppState.DevicesFrameSelectedDevices.Contains(device.ID))
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

    }


    private string GetDisplayName(PropertyInfo propInfo)
    {
        return propInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propInfo.Name;
    }

    private string GetSortIcon()
    {
        return $"oi-sort-{_sortDirection.ToString().ToLower()}";
    }

    private void HandleRefreshClicked()
    {
        Refresh();
        ToastService.ShowToast("Devices refreshed.");
    }

    private void LoadDevices()
    {
        EnsureUserSet();

        lock (_devicesLock)
        {
            _allDevices.Clear();

            var devices = DataService.GetDevicesForUser(UserName)
                .OrderByDescending(x => x.IsOnline)
                .ToList();

            _allDevices.AddRange(devices);
        }

        FilterDevices();
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
        if (AppState.DevicesFrameSelectedDevices.Any())
        {
            AppState.DevicesFrameSelectedDevices.Clear();
        }
        else
        {
            foreach (var device in _filteredDevices)
            {
                if (!AppState.DevicesFrameSelectedDevices.Contains(device.ID))
                {
                    AppState.DevicesFrameSelectedDevices.Add(device.ID);
                }
            };
        }

        AppState.InvokePropertyChanged(nameof(AppState.DevicesFrameSelectedDevices));
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
