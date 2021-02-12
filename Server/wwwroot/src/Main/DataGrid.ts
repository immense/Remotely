import { DeviceGrid, DevicesSelectedCount, OnlineDevicesCount, TotalDevicesCount, TotalPagesSpan, DeviceGridBody, CurrentPageInput } from "./UI.js";
import { AddConsoleOutput } from "./Console.js";
import { CreateChatWindow } from "./Chat.js";
import { BrowserHubConnection } from "./BrowserHubConnection.js"
import { ShowModal } from "../Shared/UI.js";
import { Device } from "../Shared/Models/Device.js";
import { EncodeForHTML } from "../Shared/Utilities.js";

export const DataSource: Array<Device> = new Array<Device>();
export const FilteredDevices: Array<Device> = new Array<Device>();

export const GridState = new class {
    GroupFilter: string = "";
    HideOffline: boolean = true;
    SearchFilter: string = "";
    ShowAllGroups: boolean = true;
    CurrentPage: number = 1;
    TotalPages: number = 1;
    readonly RowsPerPage: number = 100;
};


export function AddOrUpdateDevices(devices: Array<Device>) {
    DataSource.splice(0);
    FilteredDevices.splice(0);
    GridState.CurrentPage = 1;

    devices.sort((a, b) => {
        return a.DeviceName && a.DeviceName.localeCompare(b.DeviceName, [], { sensitivity: "base" });
    });

    devices.forEach(x => {
        AddOrUpdateDevice(x, false);
    });

    UpdateDeviceCounts();
    RenderDeviceRows();
}

export function AddOrUpdateDevice(device: Device, rerenderGrid: boolean) {
    var existingIndex = DataSource.findIndex(x => x.ID == device.ID);
    if (existingIndex > -1) {
        DataSource[existingIndex] = device;
    }
    else {
        DataSource.push(device);
    }

    ApplyFilterToDevice(device);

    if (rerenderGrid) {
        UpdateDeviceCounts();
        RenderDeviceRows();
    }
}

export function ApplyFilterToAll() {
    FilteredDevices.splice(0);
    GridState.CurrentPage = 1;

    for (var i = 0; i < DataSource.length; i++) {
        ApplyFilterToDevice(DataSource[i]);
    }

    UpdateDeviceCounts();
    RenderDeviceRows();
}

export function ApplyFilterToDevice(device: Device) {
    var existingDevice = FilteredDevices.findIndex(x => x.ID == device.ID);

    if (shouldDeviceBeShown(device)) {
        if (existingDevice > -1) {
            FilteredDevices[existingDevice] = device;
        }
        else {
            FilteredDevices.push(device);
        }
    }
    else if (existingDevice > -1)  {
        FilteredDevices.splice(existingDevice, 1);
    }
}

export function RenderDeviceRows() {
    var selectedDevices = GetSelectedDevices();

    DeviceGridBody.innerHTML = "";
    var startCurrentDevices = (GridState.CurrentPage - 1) * GridState.RowsPerPage;
    var endCurrentDevices = startCurrentDevices + GridState.RowsPerPage;

    var currentPageDevices = FilteredDevices.slice(startCurrentDevices, endCurrentDevices);

    for (var i = 0; i < currentPageDevices.length; i++) {
        let device = currentPageDevices[i];
        let recordRow = document.getElementById(device.ID) as HTMLTableRowElement;

        if (recordRow == null) {
            recordRow = document.createElement("tr");
            recordRow.classList.add("record-row");
            if (selectedDevices.some(x => x.ID == device.ID)) {
                recordRow.classList.add("row-selected");
            }
            recordRow.id = device.ID;
            DeviceGridBody.appendChild(recordRow);
            recordRow.addEventListener("click", (e) => {
                if (!e.ctrlKey && !e.shiftKey) {
                    var selectedID = (e.currentTarget as HTMLElement).id;
                    DeviceGrid.querySelectorAll(`.record-row.row-selected:not([id='${selectedID}'])`).forEach(elem => {
                        elem.classList.remove("row-selected");
                    });
                }
                (e.currentTarget as HTMLElement).classList.toggle("row-selected");
                UpdateDeviceCounts();
            });
        }

        recordRow.innerHTML = `
                    <td>
                        ${
                            device.IsOnline ?
                                "<span class='fa fa-check-circle'></span>" :
                                "<span class='fa fa-times'></span>"
                        }
                    </td>
                    <td>${EncodeForHTML(device.DeviceName)}</td>
                    <td>${EncodeForHTML(device.Alias) || ""}</td>
                    <td>${EncodeForHTML(device.CurrentUser)}</td>
                    <td>${new Date(device.LastOnline).toLocaleString()}</td>
                    <td>${EncodeForHTML(device.PublicIP)}</td>
                    <td>${EncodeForHTML(device.Platform)}</td>
                    <td>${EncodeForHTML(device.OSDescription)}</td>
                    <td>${Math.round(device.CpuUtilization * 100)}%</td>
                    <td>${Math.round(device.UsedStorage / device.TotalStorage * 100)}%</td>
                    <td>${EncodeForHTML(device.TotalStorage.toLocaleString())}</td>
                    <td>${Math.round(device.UsedMemory / device.TotalMemory * 100)}%</td>
                    <td>${EncodeForHTML(device.TotalMemory.toLocaleString())}</td>
                    <td style="white-space: nowrap">
                        <div class="dropdown">
                          <button
                                id="device-actions-dropdown-${device.ID}" 
                                class="btn btn-primary dropdown-toggle" 
                                type="button"
                                data-toggle="dropdown" 
                                aria-haspopup="true" aria-expanded="false">
                            Actions
                          </button>
                          <div class="dropdown-menu" aria-labelledby="device-actions-dropdown-${device.ID}">
                            <a class="dropdown-item device-chat-button" href="#">
                                <i class="fas fa-comment" title="Chat"></i>
                                Chat
                            </a>
                            <a class="dropdown-item device-remotecontrol-button" href="#">
                                <i class="fas fa-mouse" title="Remote Control"></i>
                                Remote Control
                            </a>
                            <a class="dropdown-item device-viewonly-button" href="#">
                                <i class="fas fa-eye" title="View Only"></i>
                                View Only
                            </a>
                            <a class="dropdown-item device-edit-button" href="#">
                                <i class="fas fa-edit" title="Edit"></i>
                                Edit
                            </a>
                            <a class="dropdown-item device-uninstall-button" href="#">
                                <i class="fas fa-times" title="Remove"></i>
                                Uninstall
                            </a>
                          </div>
                        </div>
                    </td>`;


        (recordRow.querySelector(".device-edit-button") as HTMLButtonElement).onclick = (ev) => {
            window.open(`${location.origin}/EditDevice?deviceID=${device.ID}`, "_blank");
        };
        (recordRow.querySelector(".device-chat-button") as HTMLButtonElement).onclick = (ev) => {
            CreateChatWindow(device.ID, EncodeForHTML(device.DeviceName));
        };
        (recordRow.querySelector(".device-remotecontrol-button") as HTMLButtonElement).onclick = (ev) => {
            AddConsoleOutput("Launching remote control on client device...");
            BrowserHubConnection.StartRemoteControl(device.ID, false);
        };
        (recordRow.querySelector(".device-viewonly-button") as HTMLButtonElement).onclick = (ev) => {
            AddConsoleOutput("Launching remote control on client device...");
            BrowserHubConnection.StartRemoteControl(device.ID, true);
        };
        (recordRow.querySelector(".device-uninstall-button") as HTMLButtonElement).onclick = (ev) => {
            if (confirm("Are you sure you want to uninstall the agent from this device?")) {
                BrowserHubConnection.Connection.invoke("UninstallAgents", [ device.ID ]);
            }
        };
    }
}


export function GetSelectedDevices(): Device[] {
    var selectedIds = new Array<string>();
    DeviceGrid.querySelectorAll(".row-selected").forEach(row => {
        selectedIds.push(row.id);
    });
    return DataSource.filter(x => selectedIds.includes(x.ID));
};

export function GoToCurrentPage() {
    var newPage = Number(CurrentPageInput.value);

    if (Number.isInteger(newPage) &&
        newPage > 0 &&
        newPage < GridState.TotalPages) {
        GridState.CurrentPage = newPage;
        UpdateDeviceCounts();
        RenderDeviceRows();
    }
    else {
        CurrentPageInput.value = String(GridState.CurrentPage);
    }
}

export function PageDown() {
    if (GridState.CurrentPage > 1) {
        GridState.CurrentPage--;
        UpdateDeviceCounts();
        RenderDeviceRows();
    }
}

export function PageUp() {
    if (GridState.CurrentPage < GridState.TotalPages) {
        GridState.CurrentPage++;
        UpdateDeviceCounts();
        RenderDeviceRows();
    }
}

export function RefreshGrid() {
    DataSource.splice(0);
    DeviceGridBody.innerHTML = "";
    UpdateDeviceCounts();
    var xhr = new XMLHttpRequest();
    xhr.open("get", "/API/Devices");
    xhr.onerror = () => {
        ShowModal("Request Failure", "Failed to retrieve device data.  Please refresh your connection or contact support.");
    };
    xhr.onload = () => {
        if (xhr.status == 200) {
            var devices = JSON.parse(xhr.responseText) as Device[];
            if (devices.length == 0) {
                AddConsoleOutput("It looks like you don't have the Remotely service installed on any devices.  You can get the install script from the Client Downloads page.");
            }
            else {
                AddOrUpdateDevices(devices);
            }
        }
        else {
            ShowModal("Request Failure", "Failed to retrieve device data.  Please refresh your connection or contact support.");
        }
    }
    xhr.send();
}
export function ToggleSelectAll() {
    var hiddenRows = DeviceGrid.querySelectorAll(".row-selected.hidden.row-selected");
    hiddenRows.forEach(x => {
        x.classList.remove("row-selected");
    });

    var currentlySelected = DeviceGrid.querySelectorAll(".row-selected:not(.hidden)");
    if (currentlySelected.length > 0) {
        currentlySelected.forEach(elem => {
            elem.classList.remove("row-selected");
        });
    }
    else {
        DeviceGrid.querySelectorAll(".record-row:not(.hidden)").forEach(elem => {
            elem.classList.add("row-selected");
        })
    }
    UpdateDeviceCounts();
}
export function UpdateDeviceCounts() {
    GridState.TotalPages = Math.ceil(FilteredDevices.length / GridState.RowsPerPage);
    TotalPagesSpan.innerHTML = String(GridState.TotalPages);
    CurrentPageInput.value = String(GridState.CurrentPage);
    DevicesSelectedCount.innerText = DeviceGrid.querySelectorAll(".row-selected").length.toString();
    OnlineDevicesCount.innerText = FilteredDevices.filter(x => x.IsOnline).length.toString();
    TotalDevicesCount.innerText = FilteredDevices.length.toString();
    if (
        FilteredDevices.some(x =>
            !x.IsOnline &&
            document.getElementById(x.ID) &&
            document.getElementById(x.ID).classList.contains("row-selected"))
    ) {
        AddConsoleOutput(`Your selection contains offline computers.  Your commands will only be sent to those that are online.`);
    }
}

function shouldDeviceBeShown(device: Device) {

    if (GridState.HideOffline && !device.IsOnline) {
        return false;
    }

    if (!GridState.ShowAllGroups &&
        (device.DeviceGroupID || "") != (GridState.GroupFilter || "")) {
        return false;
    }


    if (!GridState.SearchFilter) {
        return true;
    }

    for (var key in device) {
        var value = device[key];
        if (!value) {
            continue;
        }
        if (value.toString().toLowerCase().includes(GridState.SearchFilter.toLowerCase())) {
            return true;
        }
    }
    return false;
}