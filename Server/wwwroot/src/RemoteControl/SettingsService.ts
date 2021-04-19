import { Settings } from "./Interfaces/Settings.js";

const defaultSettings = {
    streamModeEnabled: false,
    displayName: ""
};


export function GetSettings(): Settings {
    try {
        var settings = localStorage.getItem("Viewer_Settings");
        if (settings) {
            return JSON.parse(settings);
        }
    }
    catch (ex) {
        console.error(ex);
    }

    SetSettings(defaultSettings);
    return defaultSettings;
}

export function SetSettings(settings: Settings) {
    localStorage.setItem("Viewer_Settings", JSON.stringify(settings));
}