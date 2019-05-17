import * as BrowserSockets from "./BrowserSockets.js";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor.js";
import { WebCommands } from "./Commands/WebCommands.js";
import { CMDCommands } from "./Commands/CMDCommands.js";
import { PSCoreCommands } from "./Commands/PSCoreCommands.js";
import { ConsoleCommand } from "./Models/ConsoleCommand.js";
import * as Utilities from "./Utilities.js";
import * as DataGrid from "./DataGrid.js";
import { Store } from "./Store.js";
import { UserSettings } from "./UserSettings.js";
import { WinPSCommands } from "./Commands/WinPSCommands.js";
import { ApplyInputEventHandlers } from "./InputEventHandlers.js";
import { Sound } from "./Sound.js";

var remotely = {
    Commands: {
        "Web": WebCommands,
        "WinPS": WinPSCommands,
        "PSCore": PSCoreCommands,
        "CMD": CMDCommands
    },
    CommandProcessor: CommandProcessor,
    DataGrid: DataGrid,
    UI: UI,
    Utilities: Utilities,
    Sockets: BrowserSockets,
    Storage: Storage,
    UserSettings: UserSettings,
    Sound: Sound,
    Init() {
        UI.ConsoleTextArea.focus();
        ApplyInputEventHandlers();
        BrowserSockets.Connect();
    }
}

export const Main = remotely;
window["Remotely"] = remotely;

window.onload = (ev) => {
    remotely.Init();
    document.querySelector(".loading-frame").remove();
    document.querySelector(".work-area").classList.remove("hidden");
}