import * as BrowserSockets from "./BrowserSockets.js";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor.js";
import { WebCommands } from "./Commands/WebCommands.js";
import { CMDCommands } from "./Commands/CMDCommands.js";
import { PSCoreCommands } from "./Commands/PSCoreCommands.js";
import * as Utilities from "./Utilities.js";
import * as DataGrid from "./DataGrid.js";
import { Store } from "./Store.js";
import { UserSettings } from "./UserSettings.js";
import { WinPSCommands } from "./Commands/WinPSCommands.js";
import { ApplyInputEventHandlers } from "./InputEventHandlers.js";
import { Sound } from "./Sound.js";
import * as Console from "./Console.js";
var remotely = {
    Commands: {
        "Web": WebCommands,
        "WinPS": WinPSCommands,
        "PSCore": PSCoreCommands,
        "CMD": CMDCommands
    },
    CommandProcessor: CommandProcessor,
    Console: Console,
    DataGrid: DataGrid,
    UI: UI,
    Utilities: Utilities,
    Sockets: BrowserSockets,
    UserSettings: UserSettings,
    Sound: Sound,
    Store: Store,
    Init() {
        UI.ConsoleTextArea.focus();
        ApplyInputEventHandlers();
        BrowserSockets.Connect();
    }
};
export const Main = remotely;
window["Remotely"] = remotely;
//# sourceMappingURL=Main.js.map