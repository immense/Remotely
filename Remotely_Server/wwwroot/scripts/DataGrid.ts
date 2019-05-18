import * as UI from "./UI.js";
import { Device } from "./Models/Device.js";
import { Main } from "./Main.js";
import { DeviceGrid } from "./UI.js";
import * as BrowserSockets from "./BrowserSockets.js";

export var DataSource: Array<Device> = new Array<Device>();

export function AddOrUpdateDevices(devices: Array<Device>) {
    devices.forEach(x => {
        AddOrUpdateDevice(x);
    });
}
export function AddOrUpdateDevice(device: Device) {
    var existingIndex = DataSource.findIndex(x => x.ID == device.ID);
    if (existingIndex > -1) {
        DataSource[existingIndex] = device;
    }
    else {
        DataSource.push(device);
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
                    <td>${String(device.IsOnline)
                            .replace("true", "<span class='fa fa-check-circle'></span>")
                            .replace("false", "<span class='fa fa-times'></span>")}</td>
                    <td>${device.DeviceName}</td>
                    <td>${device.CurrentUser}</td>
                    <td>${new Date(device.LastOnline).toLocaleString()}</td>
                    <td>${device.Platform}</td>
                    <td>${device.OSDescription}</td>
                    <td>${Math.round(device.FreeStorage * 100)}%</td>
                    <td>${device.TotalStorage.toLocaleString()}</td>
                    <td>${Math.round(device.FreeMemory * 100)}%</td>
                    <td>${device.TotalMemory.toLocaleString()}</td>
                    <td><input type="text" class="device-tag" value="${device.Tags}" /></td>`;


    (recordRow.querySelector(".device-tag") as HTMLInputElement).onchange = (ev) => {
        var newTag = (ev.currentTarget as HTMLInputElement).value;
        DataSource.find(x => x.ID == device.ID).Tags = newTag;
        var deviceTagInput = document.getElementById(device.ID).querySelector(".device-tag") as HTMLInputElement;
        deviceTagInput.value = newTag;
        deviceTagInput.setAttribute("value", newTag);
        BrowserSockets.Connection.invoke("UpdateTags", device.ID, newTag);
    };
    UpdateDeviceCounts();
}
export function GetSelectedDevices(): Device[] {
    var devices = new Array<Device>();
    DeviceGrid.querySelectorAll(".row-selected").forEach(row => {
        devices.push(DataSource.find(x => x.ID == row.id));
    });
    return devices;
};
export function ClearAllData() {
    DataSource.splice(0, DataSource.length);
    UI.DeviceGrid.querySelectorAll(".record-row").forEach(row => {
        row.remove();
    });
    UpdateDeviceCounts();
}
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
                UI.AddConsoleOutput("It looks like you don't have the Remotely service installed on any devices.  You can get the install script from the Client Downloads page.");
            }
            else {

            }
            AddOrUpdateDevices(devices);
        }
        else {
            UI.ShowModal("Request Failure", "Failed to retrieve device data.  Please refresh your connection or contact support.");
        }
    }
    xhr.send();
}
export function ToggleSelectAll() {
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
    UI.DevicesSelectedCount.innerText = UI.DeviceGrid.querySelectorAll(".row-selected").length.toString();
    UI.OnlineDevicesCount.innerText = DataSource.filter(x => x.IsOnline).length.toString();
    UI.TotalDevicesCount.innerText = DataSource.length.toString();
    if (
        DataSource.some(x =>
        x.IsOnline == false &&
        document.getElementById(x.ID).classList.contains("row-selected"))
    ) {
        UI.AddConsoleOutput(`Your selection contains offline computers.  Your commands will only be sent to those that are online.`);
    }
}