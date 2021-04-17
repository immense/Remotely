using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remotely.Server.Data;
using Remotely.Server.Enums;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Attributes;
using Remotely.Shared.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Devices
{
    [Authorize]
    public partial class DevicesFrame : AuthComponentBase, IDisposable
    {
        private readonly string _deviceGroupAll = Guid.NewGuid().ToString();
        private readonly string _deviceGroupNone = Guid.NewGuid().ToString();
        private readonly List<DeviceGroup> _deviceGroups = new();
        private readonly List<Device> _filteredDevices = new();
        private readonly ConcurrentDictionary<string, RemoteControlTarget> _remoteControlTargetLookup = new();
        private readonly List<PropertyInfo> _sortableProperties = new();
        private int _currentPage = 1;
        private int _devicesPerPage = 50;
        private string _filter;
        private bool _hideOfflineDevices = true;
        private string _selectedGroupId;
        private string _selectedSortProperty = "DeviceName";
        private ListSortDirection _sortDirection;

        [Inject]
        private IClientAppState AppState { get; set; }

        [Inject]
        private ICircuitConnection CircuitConnection { get; set; }

        [Inject]
        private IDataService DataService { get; set; }

        private IEnumerable<Device> DevicesForPage
        {
            get
            {
                var appendDevices = FilteredDevices.Where(x => AppState.DevicesFrameSelectedDevices.Contains(x.ID));
                var skipCount = (_currentPage - 1) * _devicesPerPage;
                var devicesForPage = FilteredDevices
                    .Except(appendDevices)
                    .Skip(skipCount)
                    .Take(_devicesPerPage);


                return appendDevices.Concat(devicesForPage);
            }
        }

        private List<Device> FilteredDevices
        {
            get
            {
                if (_filteredDevices.Any())
                {
                    return _filteredDevices;
                }

                var devices = DataService.GetDevicesForUser(Username)
                    .OrderByDescending(x => x.IsOnline)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(_selectedSortProperty))
                {
                    var direction = _sortDirection == ListSortDirection.Ascending ? 1 : -1;
                    devices.Sort((a, b) =>
                    {
                        if (a.IsOnline != b.IsOnline)
                        {
                            return b.IsOnline.CompareTo(a.IsOnline);
                        }

                        var propInfo = _sortableProperties.Find(x => x.Name == _selectedSortProperty);

                        var valueA = propInfo.GetValue(a);
                        var valueB = propInfo.GetValue(b);

                        return Comparer.Default.Compare(valueA, valueB) * direction;
                    });
                }
                

                _filteredDevices.AddRange(devices);

                if (_hideOfflineDevices)
                {
                    _filteredDevices.RemoveAll(x => !x.IsOnline);
                }

                if (_selectedGroupId == _deviceGroupNone)
                {
                    _filteredDevices.RemoveAll(x => !string.IsNullOrWhiteSpace(x.DeviceGroupID));
                }
                else if (_selectedGroupId != _deviceGroupAll)
                {
                    _filteredDevices.RemoveAll(x => x.DeviceGroupID != _selectedGroupId);
                }

                if (!string.IsNullOrWhiteSpace(_filter))
                {
                    _filteredDevices.RemoveAll(x =>
                        x.Alias?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        x.CurrentUser?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        x.DeviceName?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        x.Notes?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        x.Platform?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true &&
                        x.Tags?.Contains(_filter, StringComparison.OrdinalIgnoreCase) != true);
                }

                return _filteredDevices;
            }
        }

        [Inject]
        private IJsInterop JsInterop { get; set; }

        [Inject]
        private ILogger<DevicesFrame> Logger { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        private int TotalPages => (int)Math.Ceiling((decimal)FilteredDevices.Count / _devicesPerPage);

        public void Dispose()
        {
            CircuitConnection.MessageReceived -= CircuitConnection_MessageReceived;
            AppState.PropertyChanged -= AppState_PropertyChanged;
            GC.SuppressFinalize(this);
        }

        public void Refresh()
        {
            ClearSelectedCard();
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CircuitConnection.MessageReceived += CircuitConnection_MessageReceived;
            AppState.PropertyChanged += AppState_PropertyChanged;

            _deviceGroups.Clear();

            _deviceGroups.AddRange(DataService.GetDeviceGroups(User.UserName));

            _selectedGroupId = _deviceGroupAll;

            var sortableProperties = typeof(Device)
                .GetProperties()
                .Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(SortableAttribute)));

            _sortableProperties.AddRange(sortableProperties);
        }

        protected override bool ShouldRender()
        {
            // This is here to memo-ize FilteredDevices.
            var shouldRender = base.ShouldRender();
            if (shouldRender)
            {
                _filteredDevices.Clear();
            }
            return shouldRender;
        }

        private void AddScriptResult(ScriptResult result)
        {
            var device = DataService.GetDevice(result.DeviceID);
            AppState.AddTerminalLine($"{device?.DeviceName} @ {result.TimeStamp}", "font-weight-bold");

            foreach (var line in result.StandardOutput)
            {
                AppState.AddTerminalLine(line, "text-info");
            }
            foreach (var line in result.ErrorOutput)
            {
                AppState.AddTerminalLine(line, "text-danger");
            }
            AppState.InvokePropertyChanged(nameof(AppState.TerminalLines));
        }

        private void AppState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.DevicesFrameFocusedCardState) ||
                e.PropertyName == nameof(AppState.DevicesFrameFocusedDevice) ||
                e.PropertyName == nameof(AppState.DevicesFrameSelectedDevices))
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private void CircuitConnection_MessageReceived(object sender, CircuitEvent args)
        {
            switch (args.EventName)
            {
                case CircuitEventName.DeviceUpdate:
                case CircuitEventName.DeviceWentOffline:
                    {
                        InvokeAsync(StateHasChanged);
                    }
                    break;
                case CircuitEventName.DisplayMessage:
                    {
                        var terminalMessage = (string)args.Params[0];
                        var toastMessage = (string)args.Params[1];
                        var className = (string)args.Params[2];
                        AppState.AddTerminalLine(terminalMessage);
                        ToastService.ShowToast(toastMessage, classString: className);
                        InvokeAsync(StateHasChanged);
                    }
                    break;
                case CircuitEventName.ScriptResult:
                    {
                        var result = (ScriptResult)args.Params[0];
                        AddScriptResult(result);
                    }
                    break;
                case CircuitEventName.UnattendedSessionReady:
                    {
                        var casterId = (string)args.Params[0];
                        var deviceId = (string)args.Params[1];
                        if (_remoteControlTargetLookup.TryGetValue(deviceId, out var target))
                        {
                            var serviceId = target.ServiceConnectionId;
                            var viewOnly = target.ViewOnlyMode;
                            JsInterop.OpenWindow($"/RemoteControl?casterID={casterId}&serviceID={serviceId}&viewonly={viewOnly}", "_blank");
                        }
                        else
                        {
                            Logger.LogWarning("Device not found for unattended session: {deviceId}", deviceId);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private void ClearSelectedCard()
        {
            AppState.DevicesFrameFocusedDevice = null;
            AppState.DevicesFrameFocusedCardState = DeviceCardState.Normal;
        }

        private string GetDisplayName(PropertyInfo propInfo)
        {
            return propInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propInfo.Name;
        }

        private string GetSortIcon()
        {
            return $"oi-sort-{_sortDirection.ToString().ToLower()}";
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
                foreach (var device in FilteredDevices)
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
    }
}
