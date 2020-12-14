import * as HubConnection from "./HubConnection";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor";
import { WebCommands } from "./Commands/WebCommands";
import { CMDCommands } from "./Commands/CMDCommands";
import { PSCommands } from "./Commands/PSCommands";
import * as Utilities from "../Shared/Utilities";
import * as DataGrid from "./DataGrid";
import { UserSettings } from "./UserSettings";
import { ApplyInputEventHandlers } from "./InputEventHandlers";
import { Sound } from "../Shared/Sound";
import * as Console from "./Console";

export const MainApp = {
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
    Init() {
        UI.ConsoleTextArea.focus();
        ApplyInputEventHandlers();
        HubConnection.Connect();
        document.querySelector(".loading-wheel").remove();
        document.querySelector(".work-area").classList.remove("hidden");
    }
}