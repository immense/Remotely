import * as UI from "./UI.js";
import { Main } from "./Main.js";
import { DeviceGrid } from "./UI.js";
import { AddConsoleOutput } from "./Console.js";
import { CreateChatWindow } from "./Chat.js";
import * as HubConnection from "./HubConnection.js";
export const DataSource = new Array();
export const FilterOptions = new class {
    constructor() {
        this.ShowAllGroups = true;
    }
};
export function AddOrUpdateDevices(devices) {
    devices.sort((a, b) => {
        if (a.IsOnline && !b.IsOnline) {
            return -1;
        }
        else if (b.IsOnline && !a.IsOnline) {
            return 1;
        }
        return a.DeviceName.localeCompare(b.DeviceName, [], { sensitivity: "base" });
    });
    devices.forEach(x => {
        AddOrUpdateDevice(x, false);
    });
    ApplyFilter();
    UpdateDeviceCounts();
}
export function AddOrUpdateDevice(device, sortDevices) {
    var existingIndex = DataSource.findIndex(x => x.ID == device.ID);
    if (existingIndex > -1) {
        DataSource[existingIndex] = device;
    }
    else {
        DataSource.push(device);
    }
    if (sortDevices) {
        var selectedDevices = GetSelectedDevices();
        UI.DeviceGrid.querySelectorAll(".record-row").forEach(row => {
            row.remove();
        });
        AddOrUpdateDevices(DataSource);
        selectedDevices.forEach(x => {
            document.getElementById(x.ID).classList.add("row-selected");
        });
        return;
    }
    var tableBody = document.querySelector("#" + Main.UI.DeviceGrid.id + " tbody");
    var recordRow = document.getElementById(device.ID);
    if (recordRow == null) {
        recordRow = document.createElement("tr");
        recordRow.classList.add("record-row");
        recordRow.id = device.ID;
        tableBody.appendChild(recordRow);
        recordRow.addEventListener("click", (e) => {
            if (!e.ctrlKey && !e.shiftKey) {
                var selectedID = e.currentTarget.id;
                DeviceGrid.querySelectorAll(`.record-row.row-selected:not([id='${selectedID}'])`).forEach(elem => {
                    elem.classList.remove("row-selected");
                });
            }
            e.currentTarget.classList.toggle("row-selected");
            UpdateDeviceCounts();
        });
    }
    recordRow.innerHTML = `
                    <td>${String(device.IsOnline)
        .replace("true", "<span class='fa fa-check-circle'></span>")
        .replace("false", "<span class='fa fa-times'></span>")}</td>
                    <td>${device.DeviceName}</td>
                    <td>${device.Alias || ""}</td>
                    <td>${device.CurrentUser}</td>
                    <td>${new Date(device.LastOnline).toLocaleString()}</td>
                    <td>${device.PublicIP}</td>
                    <td>${device.Platform}</td>
                    <td>${device.OSDescription}</td>
                    <td>${Math.round(device.CpuUtilization * 100)}%</td>
                    <td>${Math.round(device.UsedStorage / device.TotalStorage * 100)}%</td>
                    <td>${device.TotalStorage.toLocaleString()}</td>
                    <td>${Math.round(device.UsedMemory / device.TotalMemory * 100)}%</td>
                    <td>${device.TotalMemory.toLocaleString()}</td>
                    <td style="white-space: nowrap">
                        <i class="fas fa-comment device-chat-button mr-2" title="Chat" style="font-size:1.5em"></i>
                        <i class="fas fa-mouse device-remotecontrol-button mr-2" title="Remote Control" style="font-size:1.5em"></i>
                        <i class="fas fa-edit device-edit-button" title="Edit" style="font-size:1.5em"></i>
                    </td>`;
    recordRow.querySelector(".device-edit-button").onclick = (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        window.open(`${location.origin}/EditDevice?deviceID=${device.ID}`, "_blank");
    };
    recordRow.querySelector(".device-chat-button").onclick = (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        CreateChatWindow(device.ID, device.DeviceName);
    };
    recordRow.querySelector(".device-remotecontrol-button").onclick = (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        AddConsoleOutput("Launching remote control on client device...");
        HubConnection.Connection.invoke("RemoteControl", device.ID);
    };
    UpdateDeviceCounts();
}
export function ApplyFilter() {
    for (var i = 0; i < DataSource.length; i++) {
        var row = document.getElementById(DataSource[i].ID);
        if (FilterOptions.ShowAllGroups ||
            (DataSource[i].DeviceGroupID || "") == (FilterOptions.GroupFilter || "")) {
            if (!FilterOptions.SearchFilter || deviceMatchesFilter(DataSource[i])) {
                row.classList.remove("hidden");
                continue;
            }
        }
        row.classList.add("hidden");
    }
}
export function ClearAllData() {
    DataSource.splice(0, DataSource.length);
    UI.DeviceGrid.querySelectorAll(".record-row").forEach(row => {
        row.remove();
    });
    UpdateDeviceCounts();
}
export function GetSelectedDevices() {
    var devices = new Array();
    DeviceGrid.querySelectorAll(".row-selected").forEach(row => {
        devices.push(DataSource.find(x => x.ID == row.id));
    });
    return devices;
}
;
export function RefreshGrid() {
    ClearAllData();
    var xhr = new XMLHttpRequest();
    xhr.open("get", "/API/Devices");
    xhr.onerror = () => {
        UI.ShowModal("Request Failure", "Failed to retrieve device data.  Please refresh your connection or contact support.");
    };
    xhr.onload = (e) => {
        if (xhr.status == 200) {
            var devices = JSON.parse(xhr.responseText);
            if (devices.length == 0) {
                AddConsoleOutput("It looks like you don't have the Remotely service installed on any devices.  You can get the install script from the Client Downloads page.");
            }
            else {
                AddOrUpdateDevices(devices);
            }
        }
        else {
            UI.ShowModal("Request Failure", "Failed to retrieve device data.  Please refresh your connection or contact support.");
        }
    };
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
        });
    }
    UpdateDeviceCounts();
}
export function UpdateDeviceCounts() {
    UI.DevicesSelectedCount.innerText = UI.DeviceGrid.querySelectorAll(".row-selected").length.toString();
    UI.OnlineDevicesCount.innerText = DataSource.filter(x => x.IsOnline).length.toString();
    UI.TotalDevicesCount.innerText = DataSource.length.toString();
    if (DataSource.some(x => !x.IsOnline &&
        document.getElementById(x.ID) &&
        document.getElementById(x.ID).classList.contains("row-selected"))) {
        AddConsoleOutput(`Your selection contains offline computers.  Your commands will only be sent to those that are online.`);
    }
}
function deviceMatchesFilter(device) {
    for (var key in device) {
        var value = device[key];
        if (!value) {
            continue;
        }
        if (value.toString().toLowerCase().includes(FilterOptions.SearchFilter.toLowerCase())) {
            return true;
        }
    }
    return false;
}
//# sourceMappingURL=DataGrid.js.map