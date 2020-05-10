import * as HubConnection from "./HubConnection.js";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor.js";
import { WebCommands } from "./Commands/WebCommands.js";
import { CMDCommands } from "./Commands/CMDCommands.js";
import { PSCommands } from "./Commands/PSCommands.js";
import * as Utilities from "./Utilities.js";
import * as DataGrid from "./DataGrid.js";
import { Store } from "./Store.js";
import { UserSettings } from "./UserSettings.js";
import { ApplyInputEventHandlers } from "./InputEventHandlers.js";
import { Sound } from "./Sound.js";
import * as Console from "./Console.js";

var remotely = {
    Commands: {
        "Web": WebCommands,
        "WinPS": PSCommands,
        "PSCore": PSCommands,
        "CMD": CMDCommands
    },
    CommandProcessor: CommandProcessor,
    Console: Console,
    DataGrid: DataGrid,
    UI: UI,
    Utilities: Utilities,
    HubConnection: HubConnection,
    UserSettings: UserSettings,
    Sound: Sound,
    Store: Store,
    Init() {
        UI.ConsoleTextArea.focus();
        ApplyInputEventHandlers();
        HubConnection.Connect();
    }
}

export const Main = remotely;
window["Remotely"] = remotely;