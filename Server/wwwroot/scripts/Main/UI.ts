import * as Utilities from "../Shared/Utilities.js";


export var CommandCompletionDiv = document.querySelector("#commandCompletionDiv") as HTMLDivElement;
export var CommandInfoDiv = document.querySelector("#commandInfoDiv") as HTMLDivElement;
export var CommandModeSelect = document.querySelector("#commandModeSelect") as HTMLSelectElement;
export var ConsoleOutputDiv = document.querySelector("#consoleOutputDiv") as HTMLDivElement;
export var ConsoleTextArea = document.querySelector("#consoleTextArea") as HTMLTextAreaElement;
export var DeviceGrid = document.querySelector("#deviceGrid") as HTMLTableElement;
export var DevicesSelectedCount = document.querySelector("#devicesSelectedSpan") as HTMLSpanElement;
export var OnlineDevicesCount = document.querySelector("#onlineDevicesSpan") as HTMLSpanElement;
export var TotalDevicesCount = document.querySelector("#totalDevicesSpan") as HTMLSpanElement;
export var MeasurementCanvas = document.createElement("canvas");
export var MeasurementContext = MeasurementCanvas.getContext("2d");
export var TabContentWrapper = document.getElementById("tabContentWrapper") as HTMLDivElement;
export var ConsoleFrame = document.getElementById("consoleFrame") as HTMLDivElement;
export var ConsoleTab = document.getElementById("consoleTab") as HTMLAnchorElement;
export var ConsoleAlert = document.getElementById("consoleAlert") as HTMLAnchorElement;
export var DeviceGroupSelect = document.getElementById("deviceGroupSelect") as HTMLSelectElement;
export var GridFilter = document.getElementById("gridFilter") as HTMLInputElement;
export var AlertsButton = document.getElementById("alertsButton") as HTMLButtonElement;
export var CloseAlertsButton = document.getElementById("closeAlertsFrameButton") as HTMLButtonElement;
export var AlertsFrame = document.getElementById("alertsFrame") as HTMLDivElement;
export var AlertsCount = document.getElementById("alertsCount") as HTMLSpanElement;
export var ToastsWrapper = document.getElementById("toastsWrapper") as HTMLDivElement;