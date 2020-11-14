import { ViewerApp } from "./App.js";
import { Settings } from "./Interfaces/Settings.js";
import { AutoQualityAdjustCheckBox, QualitySlider, UpdateStreamingToggled } from "./UI.js";

const defaultSettings = {
    autoQualityEnabled: true,
    qualityLevel: 60,
    streamModeEnabled: false
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