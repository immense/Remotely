import { BrowserHubConnection } from "./BrowserHubConnection.js";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor.js";
import { WebCommands } from "./Commands/WebCommands.js";
import { CMDCommands } from "./Commands/CMDCommands.js";
import { PSCommands } from "./Commands/PSCommands.js";
import * as Utilities from "../Shared/Utilities.js";
import * as DataGrid from "./DataGrid.js";
import { UserSettings } from "./UserSettings.js";
import { ApplyInputEventHandlers } from "./InputEventHandlers.js";
import { Sound } from "../Shared/Sound.js";
import * as Console from "./Console.js";

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
    HubConnection: BrowserHubConnection,
    UserSettings: UserSettings,
    Sound: Sound,
    GetMotd() {
        fetch("https://remotely.one/api/motd")
            .then(async response => {
                var content = await response.text();
                if (localStorage["remotely-motd"] == content) {
                    return;
                }

                UI.MotdAlert.innerHTML += content;
                UI.MotdAlert.removeAttribute("hidden");
                UI.MotdAlert.querySelector("button").addEventListener("click", () => {
                    localStorage["remotely-motd"] = content;
                });
            })
            .catch(reason => {
                console.warn(`Unable to retrieve message of the day.  Error: ${reason}`);
            })
    },
    Init() {
        MainApp.GetMotd();
        UI.ConsoleTextArea.focus();
        ApplyInputEventHandlers();
        BrowserHubConnection.Connect();
        document.querySelector(".loading-wheel").remove();
        document.querySelector(".work-area").classList.remove("hidden");
    }
}