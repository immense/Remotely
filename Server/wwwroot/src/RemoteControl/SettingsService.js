const defaultSettings = {
    streamModeEnabled: false,
    displayName: ""
};
export function GetSettings() {
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
export function SetSettings(settings) {
    localStorage.setItem("Viewer_Settings", JSON.stringify(settings));
}
//# sourceMappingURL=SettingsService.js.map