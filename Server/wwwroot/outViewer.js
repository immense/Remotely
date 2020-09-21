define("Shared/Utilities", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.RemoveFromArray = exports.FormatScriptOutputArray = exports.FormatScriptOutput = exports.ConvertUInt8ArrayToBase64 = exports.ConvertBase64ToUInt8Array = exports.When = exports.GetDistanceBetween = exports.ParseSearchString = exports.EncodeForHTML = exports.CreateGUID = exports.Split = void 0;
    /**
     * Splits a string into "parts" number of pieces.  Once "parts" number is
     * reached, additional occurrences of the splitter are ignored.
     * @param splitter
     * @param parts
     */
    function Split(originalString, splitter, parts) {
        var returnArray = [];
        var remainingString = originalString;
        for (var i = 1; i < parts; i++) {
            if (remainingString.indexOf(splitter) == -1) {
                break;
            }
            returnArray.push(remainingString.slice(0, remainingString.indexOf(splitter)));
            remainingString = remainingString.slice(remainingString.indexOf(splitter) + splitter.length);
        }
        returnArray.push(remainingString);
        return returnArray;
    }
    exports.Split = Split;
    function CreateGUID() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    exports.CreateGUID = CreateGUID;
    function EncodeForHTML(text) {
        var tempDiv = document.createElement("div");
        tempDiv.innerText = text;
        return tempDiv.innerHTML;
    }
    exports.EncodeForHTML = EncodeForHTML;
    function ParseSearchString() {
        var queryStrings = {};
        var queryArray = location.search.substring(1).split("&");
        queryArray.forEach(value => {
            var keyValue = value.split("=");
            queryStrings[keyValue[0]] = keyValue[1];
        });
        return queryStrings;
    }
    exports.ParseSearchString = ParseSearchString;
    function GetDistanceBetween(fromX, fromY, toX, toY) {
        return Math.sqrt(Math.pow(fromX - toX, 2) +
            Math.pow(fromY - toY, 2));
    }
    exports.GetDistanceBetween = GetDistanceBetween;
    async function When(predicate, pollingTimeMs = 100) {
        return new Promise((resolve, reject) => {
            function checkCondition() {
                if (predicate()) {
                    resolve();
                }
                else {
                    window.setTimeout(() => {
                        checkCondition();
                    }, pollingTimeMs);
                }
            }
            checkCondition();
        });
    }
    exports.When = When;
    function ConvertBase64ToUInt8Array(base64) {
        var binaryString = window.atob(base64);
        var bytes = new Uint8ClampedArray(binaryString.length);
        for (var i = 0; i < binaryString.length; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }
        return bytes;
    }
    exports.ConvertBase64ToUInt8Array = ConvertBase64ToUInt8Array;
    function ConvertUInt8ArrayToBase64(array) {
        var base64String = '';
        for (var i = 0; i < array.byteLength; i++) {
            base64String += String.fromCharCode(array[i]);
        }
        return btoa(base64String);
    }
    exports.ConvertUInt8ArrayToBase64 = ConvertUInt8ArrayToBase64;
    function FormatScriptOutput(output) {
        return EncodeForHTML(output).replace(/ /g, "&nbsp;").replace(/\n/g, "<br>");
    }
    exports.FormatScriptOutput = FormatScriptOutput;
    function FormatScriptOutputArray(output) {
        return output.map(x => EncodeForHTML(x)).join("<br>");
    }
    exports.FormatScriptOutputArray = FormatScriptOutputArray;
    function RemoveFromArray(array, item) {
        var index = array.indexOf(item);
        if (index > -1) {
            array.splice(index, 1);
        }
    }
    exports.RemoveFromArray = RemoveFromArray;
    ;
});
define("Shared/Enums/WindowsSessionType", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.WindowsSessionType = void 0;
    var WindowsSessionType;
    (function (WindowsSessionType) {
        WindowsSessionType[WindowsSessionType["Console"] = 0] = "Console";
        WindowsSessionType[WindowsSessionType["RDP"] = 1] = "RDP";
    })(WindowsSessionType = exports.WindowsSessionType || (exports.WindowsSessionType = {}));
});
define("Shared/Models/WindowsSession", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.WindowsSession = void 0;
    class WindowsSession {
    }
    exports.WindowsSession = WindowsSession;
});
define("RemoteControl/UI", ["require", "exports", "RemoteControl/App", "Shared/Utilities", "Shared/Enums/WindowsSessionType"], function (require, exports, App_js_1, Utilities_js_1, WindowsSessionType_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.UpdateWindowsSessions = exports.UpdateDisplays = exports.UpdateCursor = exports.SetScreenSize = exports.Prompt = exports.GetCurrentViewer = exports.DownloadRecordingButton = exports.RecordSessionButton = exports.WindowsSessionSelect = exports.ConnectionRelayedIcon = exports.ConnectionP2PIcon = exports.TypeClipboardButton = exports.ClipboardTransferButton = exports.ClipboardTransferBar = exports.TouchKeyboardTextArea = exports.CtrlAltDelButton = exports.FileDownloadButton = exports.FileUploadButtton = exports.FileTransferBar = exports.FileTransferButton = exports.InviteButton = exports.KeyboardButton = exports.FileTransferNameSpan = exports.FileTransferProgress = exports.FileTransferInput = exports.DisconnectButton = exports.BlockInputButton = exports.FitToScreenButton = exports.StreamVideoButton = exports.QualityButton = exports.ChangeScreenButton = exports.ViewBar = exports.ActionsBar = exports.AutoQualityAdjustCheckBox = exports.QualitySlider = exports.QualityBar = exports.ScreenSelectBar = exports.ConnectBox = exports.HorizontalBars = exports.Screen2DContext = exports.ScreenViewerWrapper = exports.VideoScreenViewer = exports.ScreenViewer = exports.StatusMessage = exports.RequesterNameInput = exports.ConnectButton = exports.SessionIDInput = exports.MenuFrame = exports.MenuButton = exports.AudioButton = void 0;
    exports.AudioButton = document.getElementById("audioButton");
    exports.MenuButton = document.getElementById("menuButton");
    exports.MenuFrame = document.getElementById("menuFrame");
    exports.SessionIDInput = document.getElementById("sessionIDInput");
    exports.ConnectButton = document.getElementById("connectButton");
    exports.RequesterNameInput = document.getElementById("nameInput");
    exports.StatusMessage = document.getElementById("statusMessage");
    exports.ScreenViewer = document.getElementById("screenViewer");
    exports.VideoScreenViewer = document.getElementById("videoScreenViewer");
    exports.ScreenViewerWrapper = document.getElementById("screenViewerWrapper");
    exports.Screen2DContext = exports.ScreenViewer.getContext("2d");
    exports.HorizontalBars = document.querySelectorAll(".horizontal-button-bar");
    exports.ConnectBox = document.getElementById("connectBox");
    exports.ScreenSelectBar = document.getElementById("screenSelectBar");
    exports.QualityBar = document.getElementById("qualityBar");
    exports.QualitySlider = document.getElementById("qualityRangeInput");
    exports.AutoQualityAdjustCheckBox = document.getElementById("autoAdjustQualityCheckBox");
    exports.ActionsBar = document.getElementById("actionsBar");
    exports.ViewBar = document.getElementById("viewBar");
    exports.ChangeScreenButton = document.getElementById("changeScreenButton");
    exports.QualityButton = document.getElementById("qualityButton");
    exports.StreamVideoButton = document.getElementById("streamVideoButton");
    exports.FitToScreenButton = document.getElementById("fitToScreenButton");
    exports.BlockInputButton = document.getElementById("blockInputButton");
    exports.DisconnectButton = document.getElementById("disconnectButton");
    exports.FileTransferInput = document.getElementById("fileTransferInput");
    exports.FileTransferProgress = document.getElementById("fileTransferProgress");
    exports.FileTransferNameSpan = document.getElementById("fileTransferNameSpan");
    exports.KeyboardButton = document.getElementById("keyboardButton");
    exports.InviteButton = document.getElementById("inviteButton");
    exports.FileTransferButton = document.getElementById("fileTransferButton");
    exports.FileTransferBar = document.getElementById("fileTransferBar");
    exports.FileUploadButtton = document.getElementById("fileUploadButton");
    exports.FileDownloadButton = document.getElementById("fileDownloadButton");
    exports.CtrlAltDelButton = document.getElementById("ctrlAltDelButton");
    exports.TouchKeyboardTextArea = document.getElementById("touchKeyboardTextArea");
    exports.ClipboardTransferBar = document.getElementById("clipboardTransferBar");
    exports.ClipboardTransferButton = document.getElementById("clipboardTransferButton");
    exports.TypeClipboardButton = document.getElementById("typeClipboardButton");
    exports.ConnectionP2PIcon = document.getElementById("connectionP2PIcon");
    exports.ConnectionRelayedIcon = document.getElementById("connectionRelayedIcon");
    exports.WindowsSessionSelect = document.getElementById("windowsSessionSelect");
    exports.RecordSessionButton = document.getElementById("recordSessionButton");
    exports.DownloadRecordingButton = document.getElementById("downloadRecordingButton");
    function GetCurrentViewer() {
        if (exports.ScreenViewer.hasAttribute("hidden")) {
            return exports.VideoScreenViewer;
        }
        return exports.ScreenViewer;
    }
    exports.GetCurrentViewer = GetCurrentViewer;
    function Prompt(promptMessage) {
        return new Promise((resolve, reject) => {
            var modalDiv = document.createElement("div");
            modalDiv.classList.add("modal-prompt");
            var messageDiv = document.createElement("div");
            messageDiv.innerHTML = promptMessage;
            var responseInput = document.createElement("input");
            var buttonsDiv = document.createElement("div");
            buttonsDiv.classList.add("buttons-footer");
            var cancelButton = document.createElement("button");
            cancelButton.innerHTML = "Cancel";
            var okButton = document.createElement("button");
            okButton.innerHTML = "OK";
            buttonsDiv.appendChild(okButton);
            buttonsDiv.appendChild(cancelButton);
            modalDiv.appendChild(messageDiv);
            modalDiv.appendChild(responseInput);
            modalDiv.appendChild(buttonsDiv);
            document.body.appendChild(modalDiv);
            okButton.onclick = () => {
                modalDiv.remove();
                resolve(responseInput.value);
            };
            cancelButton.onclick = () => {
                modalDiv.remove();
                resolve(null);
            };
        });
    }
    exports.Prompt = Prompt;
    function SetScreenSize(width, height) {
        exports.ScreenViewer.width = width;
        exports.ScreenViewer.height = height;
        exports.Screen2DContext.clearRect(0, 0, width, height);
    }
    exports.SetScreenSize = SetScreenSize;
    function UpdateCursor(imageBytes, hotSpotX, hotSpotY, cssOverride) {
        var targetElement = GetCurrentViewer();
        if (cssOverride) {
            targetElement.style.cursor = cssOverride;
        }
        else if (imageBytes.byteLength == 0) {
            targetElement.style.cursor = "default";
        }
        else {
            var base64 = Utilities_js_1.ConvertUInt8ArrayToBase64(imageBytes);
            targetElement.style.cursor = `url('data:image/png;base64,${base64}') ${hotSpotX} ${hotSpotY}, default`;
        }
    }
    exports.UpdateCursor = UpdateCursor;
    function UpdateDisplays(selectedDisplay, displayNames) {
        exports.ScreenSelectBar.innerHTML = "";
        for (let i = 0; i < displayNames.length; i++) {
            var button = document.createElement("button");
            button.innerHTML = `Monitor ${i}`;
            button.classList.add("horizontal-bar-button");
            if (displayNames[i] == selectedDisplay) {
                button.classList.add("toggled");
            }
            exports.ScreenSelectBar.appendChild(button);
            button.onclick = (ev) => {
                App_js_1.ViewerApp.MessageSender.SendSelectScreen(displayNames[i]);
                document.querySelectorAll("#screenSelectBar .horizontal-bar-button").forEach(button => {
                    button.classList.remove("toggled");
                });
                ev.currentTarget.classList.add("toggled");
            };
        }
    }
    exports.UpdateDisplays = UpdateDisplays;
    function UpdateWindowsSessions(windowsSessions) {
        while (exports.WindowsSessionSelect.options.length > 0) {
            exports.WindowsSessionSelect.options.remove(0);
        }
        exports.WindowsSessionSelect.options.add(document.createElement("option"));
        windowsSessions.forEach(x => {
            var sessionType = "";
            if (typeof x.Type == "number") {
                sessionType = x.Type == WindowsSessionType_js_1.WindowsSessionType.Console ? "Console" : "RDP";
            }
            else {
                sessionType = x.Type;
            }
            var option = document.createElement("option");
            option.value = String(x.ID);
            option.text = `${sessionType} (ID: ${x.ID} | User: ${x.Username})`;
            option.title = `${sessionType} Session (ID: ${x.ID} | User: ${x.Username})`;
            exports.WindowsSessionSelect.options.add(option);
        });
    }
    exports.UpdateWindowsSessions = UpdateWindowsSessions;
});
define("Shared/Models/IceServerModel", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.IceServerModel = void 0;
    class IceServerModel {
    }
    exports.IceServerModel = IceServerModel;
});
define("RemoteControl/RtcSession", ["require", "exports", "RemoteControl/UI", "RemoteControl/App"], function (require, exports, UI, App_js_2) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.RtcSession = void 0;
    class RtcSession {
        constructor() {
            this.MessagePack = window['MessagePack'];
        }
        Init(iceServers) {
            this.PeerConnection = new RTCPeerConnection({
                iceServers: iceServers.map(x => {
                    return {
                        urls: x.Url,
                        username: x.TurnUsername,
                        credential: x.TurnPassword,
                        credentialType: "password"
                    };
                })
            });
            this.PeerConnection.ondatachannel = (ev) => {
                console.log("Data channel received.");
                this.DataChannel = ev.channel;
                this.DataChannel.binaryType = "arraybuffer";
                this.DataChannel.onclose = (ev) => {
                    console.log("Data channel closed.");
                    UI.ConnectionP2PIcon.style.display = "none";
                    UI.ConnectionRelayedIcon.style.display = "unset";
                    UI.StreamVideoButton.setAttribute("hidden", "hidden");
                    UI.ScreenViewer.removeAttribute("hidden");
                    UI.QualityButton.removeAttribute("hidden");
                    UI.VideoScreenViewer.setAttribute("hidden", "hidden");
                };
                this.DataChannel.onerror = (ev) => {
                    console.log("Data channel error.", ev.error);
                    UI.ConnectionP2PIcon.style.display = "none";
                    UI.ConnectionRelayedIcon.style.display = "unset";
                    UI.StreamVideoButton.setAttribute("hidden", "hidden");
                    UI.ScreenViewer.removeAttribute("hidden");
                    UI.QualityButton.removeAttribute("hidden");
                    UI.VideoScreenViewer.setAttribute("hidden", "hidden");
                };
                this.DataChannel.onmessage = async (ev) => {
                    var data = ev.data;
                    App_js_2.ViewerApp.DtoMessageHandler.ParseBinaryMessage(data);
                };
                this.DataChannel.onopen = (ev) => {
                    console.log("Data channel opened.");
                    UI.ConnectionP2PIcon.style.display = "unset";
                    UI.ConnectionRelayedIcon.style.display = "none";
                    UI.StreamVideoButton.removeAttribute("hidden");
                };
            };
            this.PeerConnection.onconnectionstatechange = function (ev) {
                console.log("Connection state changed to " + this.connectionState);
            };
            this.PeerConnection.oniceconnectionstatechange = function (ev) {
                console.log("ICE connection state changed to " + this.iceConnectionState);
            };
            this.PeerConnection.onicecandidate = async (ev) => {
                await App_js_2.ViewerApp.ViewerHubConnection.SendIceCandidate(ev.candidate);
            };
            UI.VideoScreenViewer.onloadedmetadata = (ev) => {
                UI.VideoScreenViewer.play();
            };
            this.PeerConnection.ontrack = (event) => {
                if (event.track) {
                    UI.VideoScreenViewer.srcObject = new MediaStream([event.track]);
                }
            };
        }
        Disconnect() {
            this.PeerConnection.close();
        }
        async ReceiveRtcOffer(sdp) {
            await this.PeerConnection.setRemoteDescription({ type: "offer", sdp: sdp });
            await this.PeerConnection.setLocalDescription(await this.PeerConnection.createAnswer());
            await App_js_2.ViewerApp.ViewerHubConnection.SendRtcAnswer(this.PeerConnection.localDescription);
            console.log("Set RTC offer.");
        }
        async ReceiveCandidate(candidate) {
            await this.PeerConnection.addIceCandidate(candidate);
            console.log("Set ICE candidate.");
        }
        SendDto(dto) {
            this.DataChannel.send(this.MessagePack.encode(dto));
        }
    }
    exports.RtcSession = RtcSession;
});
define("Shared/Enums/RemoteControlMode", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.RemoteControlMode = void 0;
    var RemoteControlMode;
    (function (RemoteControlMode) {
        RemoteControlMode[RemoteControlMode["None"] = 0] = "None";
        RemoteControlMode[RemoteControlMode["Unattended"] = 1] = "Unattended";
        RemoteControlMode[RemoteControlMode["Normal"] = 2] = "Normal";
    })(RemoteControlMode = exports.RemoteControlMode || (exports.RemoteControlMode = {}));
});
define("RemoteControl/ClipboardWatcher", ["require", "exports", "RemoteControl/App"], function (require, exports, App_js_3) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ClipboardWatcher = void 0;
    class ClipboardWatcher {
        WatchClipboard() {
            if (navigator.clipboard.readText) {
                this.ClipboardTimer = setInterval(() => {
                    if (this.PauseMonitoring) {
                        return;
                    }
                    if (!document.hasFocus()) {
                        return;
                    }
                    navigator.clipboard.readText().then(newText => {
                        if (this.LastClipboardText != newText) {
                            this.LastClipboardText = newText;
                            App_js_3.ViewerApp.MessageSender.SendClipboardTransfer(newText, false);
                        }
                    });
                }, 100);
            }
        }
        SetClipboardText(text) {
            if (text == this.LastClipboardText) {
                return;
            }
            if (navigator.clipboard.writeText) {
                this.PauseMonitoring = true;
                this.LastClipboardText = text;
                navigator.clipboard.writeText(text);
                this.PauseMonitoring = false;
            }
        }
    }
    exports.ClipboardWatcher = ClipboardWatcher;
});
define("Shared/Enums/BaseDtoType", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.BaseDtoType = void 0;
    var BaseDtoType;
    (function (BaseDtoType) {
        BaseDtoType[BaseDtoType["CaptureFrame"] = 0] = "CaptureFrame";
        BaseDtoType[BaseDtoType["ScreenData"] = 1] = "ScreenData";
        BaseDtoType[BaseDtoType["ScreenSize"] = 2] = "ScreenSize";
        BaseDtoType[BaseDtoType["MachineName"] = 3] = "MachineName";
        BaseDtoType[BaseDtoType["ClipboardText"] = 4] = "ClipboardText";
        BaseDtoType[BaseDtoType["AudioSample"] = 5] = "AudioSample";
        BaseDtoType[BaseDtoType["CursorChange"] = 6] = "CursorChange";
        BaseDtoType[BaseDtoType["SelectScreen"] = 7] = "SelectScreen";
        BaseDtoType[BaseDtoType["MouseMove"] = 8] = "MouseMove";
        BaseDtoType[BaseDtoType["MouseDown"] = 9] = "MouseDown";
        BaseDtoType[BaseDtoType["MouseUp"] = 10] = "MouseUp";
        BaseDtoType[BaseDtoType["Tap"] = 11] = "Tap";
        BaseDtoType[BaseDtoType["MouseWheel"] = 12] = "MouseWheel";
        BaseDtoType[BaseDtoType["KeyDown"] = 13] = "KeyDown";
        BaseDtoType[BaseDtoType["KeyUp"] = 14] = "KeyUp";
        BaseDtoType[BaseDtoType["CtrlAltDel"] = 15] = "CtrlAltDel";
        BaseDtoType[BaseDtoType["AutoQualityAdjust"] = 16] = "AutoQualityAdjust";
        BaseDtoType[BaseDtoType["ToggleAudio"] = 17] = "ToggleAudio";
        BaseDtoType[BaseDtoType["ToggleBlockInput"] = 18] = "ToggleBlockInput";
        BaseDtoType[BaseDtoType["ClipboardTransfer"] = 19] = "ClipboardTransfer";
        BaseDtoType[BaseDtoType["KeyPress"] = 20] = "KeyPress";
        BaseDtoType[BaseDtoType["QualityChange"] = 21] = "QualityChange";
        BaseDtoType[BaseDtoType["File"] = 22] = "File";
        BaseDtoType[BaseDtoType["WindowsSessions"] = 23] = "WindowsSessions";
        BaseDtoType[BaseDtoType["SetKeyStatesUp"] = 24] = "SetKeyStatesUp";
        BaseDtoType[BaseDtoType["FrameReceived"] = 25] = "FrameReceived";
        BaseDtoType[BaseDtoType["ToggleWebRtcVideo"] = 26] = "ToggleWebRtcVideo";
        BaseDtoType[BaseDtoType["OpenFileTransferWindow"] = 27] = "OpenFileTransferWindow";
    })(BaseDtoType = exports.BaseDtoType || (exports.BaseDtoType = {}));
});
define("RemoteControl/BaseDto", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("Shared/UI", ["require", "exports", "Shared/Utilities"], function (require, exports, Utilities_js_2) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ValidateInput = exports.ShowModal = exports.ShowMessage = exports.ToastsWrapper = void 0;
    exports.ToastsWrapper = document.getElementById("toastsWrapper");
    function ShowMessage(message) {
        var messageDiv = document.createElement("div");
        messageDiv.classList.add("toast-message");
        messageDiv.innerHTML = message;
        exports.ToastsWrapper.appendChild(messageDiv);
        window.setTimeout(() => {
            messageDiv.remove();
        }, 5000);
    }
    exports.ShowMessage = ShowMessage;
    function ShowModal(title, modalBodyHtml, buttonsHTML = "", onDismissCallback = null) {
        var modalID = Utilities_js_2.CreateGUID();
        var modalHTML = `<div id="${modalID}" class="modal fade in" tabindex="-1" role="dialog">
          <div class="modal-dialog" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">${title}</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div class="modal-body">
                ${modalBodyHtml}
              </div>
              <div class="modal-footer">
                ${buttonsHTML}
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
              </div>
            </div>
          </div>
        </div>`;
        var wrapperDiv = document.createElement("div");
        wrapperDiv.innerHTML = modalHTML;
        document.body.appendChild(wrapperDiv);
        $("#" + modalID).on("hidden.bs.modal", ev => {
            try {
                if (onDismissCallback) {
                    onDismissCallback();
                }
            }
            finally {
                ev.currentTarget.parentElement.remove();
            }
        });
        $("#" + modalID)["modal"]("show");
        return wrapperDiv;
    }
    exports.ShowModal = ShowModal;
    function ValidateInput(inputElement) {
        if (!inputElement.checkValidity()) {
            $(inputElement)["tooltip"]({
                template: '<div class="tooltip" role="tooltip"><div class="arrow"></div><div class="tooltip-inner text-danger"></div></div>',
                title: inputElement.validationMessage
            });
            $(inputElement)["tooltip"]("show");
            return false;
        }
        else {
            return true;
        }
    }
    exports.ValidateInput = ValidateInput;
});
define("Shared/Sound", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.Sound = void 0;
    exports.Sound = new class {
        constructor() {
            this.SourceNodes = new Array();
        }
        Init() {
            if (this.Context) {
                // Already initialized.
                return;
            }
            if (AudioContext) {
                this.Context = new AudioContext();
            }
            else if (window["webkitAudioContext"]) {
                this.Context = new window["webkitAudioContext"];
            }
            else {
                return;
            }
            this.BackgroundAudio = new Audio();
            this.BackgroundNode = this.Context.createMediaElementSource(this.BackgroundAudio);
            this.BackgroundNode.connect(this.Context.destination);
        }
        Play(buffer) {
            if (!this.Context) {
                return;
            }
            var fr = new FileReader();
            fr.onload = async (ev) => {
                var audioBuffer = await this.Context.decodeAudioData(fr.result);
                var bufferSource = this.Context.createBufferSource();
                bufferSource.buffer = audioBuffer;
                bufferSource.connect(this.Context.destination);
                bufferSource.start();
            };
            fr.readAsArrayBuffer(new Blob([buffer], { 'type': 'audio/wav' }));
        }
        ;
    };
});
define("Shared/Models/Point", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("Shared/Models/CursorInfo", ["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
});
define("RemoteControl/Dtos", ["require", "exports", "Shared/Enums/BaseDtoType"], function (require, exports, BaseDtoType_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.WindowsSessionsDto = exports.ToggleWebRtcVideoDto = exports.ToggleBlockInputDto = exports.ToggleAudioDto = exports.TapDto = exports.SelectScreenDto = exports.QualityChangeDto = exports.MouseWheelDto = exports.MouseUpDto = exports.MouseMoveDto = exports.MouseDownDto = exports.KeyUpDto = exports.KeyPressDto = exports.KeyDownDto = exports.GenericDto = exports.FileDto = exports.CtrlAltDelDto = exports.ClipboardTransferDto = exports.AutoQualityAdjustDto = void 0;
    class AutoQualityAdjustDto {
        constructor(isOn) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.AutoQualityAdjust;
            this.IsOn = isOn;
        }
    }
    exports.AutoQualityAdjustDto = AutoQualityAdjustDto;
    class ClipboardTransferDto {
        constructor(text, typeText) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.ClipboardTransfer;
            this.Text = text;
            this.TypeText = typeText;
        }
    }
    exports.ClipboardTransferDto = ClipboardTransferDto;
    class CtrlAltDelDto {
        constructor() {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.CtrlAltDel;
        }
    }
    exports.CtrlAltDelDto = CtrlAltDelDto;
    class FileDto {
        constructor(buffer, fileName, messageId, endOfFile, startOfFile) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.File;
            this.Buffer = buffer;
            this.FileName = fileName;
            this.MessageId = messageId;
            this.EndOfFile = endOfFile;
            this.StartOfFile = startOfFile;
        }
    }
    exports.FileDto = FileDto;
    class GenericDto {
        constructor(type) {
            this.DtoType = type;
        }
    }
    exports.GenericDto = GenericDto;
    class KeyDownDto {
        constructor(key) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.KeyDown;
            this.Key = key;
        }
    }
    exports.KeyDownDto = KeyDownDto;
    class KeyPressDto {
        constructor(key) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.KeyPress;
            this.Key = key;
        }
    }
    exports.KeyPressDto = KeyPressDto;
    class KeyUpDto {
        constructor(key) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.KeyUp;
            this.Key = key;
        }
    }
    exports.KeyUpDto = KeyUpDto;
    class MouseDownDto {
        constructor(button, percentX, percentY) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.MouseDown;
            this.Button = button;
            this.PercentX = percentX;
            this.PercentY = percentY;
        }
    }
    exports.MouseDownDto = MouseDownDto;
    class MouseMoveDto {
        constructor(percentX, percentY) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.MouseMove;
            this.PercentX = percentX;
            this.PercentY = percentY;
        }
    }
    exports.MouseMoveDto = MouseMoveDto;
    class MouseUpDto {
        constructor(button, percentX, percentY) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.MouseUp;
            this.Button = button;
            this.PercentX = percentX;
            this.PercentY = percentY;
        }
    }
    exports.MouseUpDto = MouseUpDto;
    class MouseWheelDto {
        constructor(deltaX, deltaY) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.MouseWheel;
            this.DeltaX = deltaX;
            this.DeltaY = deltaY;
        }
    }
    exports.MouseWheelDto = MouseWheelDto;
    class QualityChangeDto {
        constructor(qualityLevel) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.QualityChange;
            this.QualityLevel = qualityLevel;
        }
    }
    exports.QualityChangeDto = QualityChangeDto;
    class SelectScreenDto {
        constructor(displayName) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.SelectScreen;
            this.DisplayName = displayName;
        }
    }
    exports.SelectScreenDto = SelectScreenDto;
    class TapDto {
        constructor(percentX, percentY) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.Tap;
            this.PercentX = percentX;
            this.PercentY = percentY;
        }
    }
    exports.TapDto = TapDto;
    class ToggleAudioDto {
        constructor(toggleOn) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.ToggleAudio;
            this.ToggleOn = toggleOn;
        }
    }
    exports.ToggleAudioDto = ToggleAudioDto;
    class ToggleBlockInputDto {
        constructor(toggleOn) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.ToggleBlockInput;
            this.ToggleOn = toggleOn;
        }
    }
    exports.ToggleBlockInputDto = ToggleBlockInputDto;
    class ToggleWebRtcVideoDto {
        constructor(toggleOn) {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.ToggleWebRtcVideo;
            this.ToggleOn = toggleOn;
        }
    }
    exports.ToggleWebRtcVideoDto = ToggleWebRtcVideoDto;
    class WindowsSessionsDto {
        constructor() {
            this.DtoType = BaseDtoType_js_1.BaseDtoType.WindowsSessions;
        }
    }
    exports.WindowsSessionsDto = WindowsSessionsDto;
});
define("RemoteControl/FileTransferService", ["require", "exports", "RemoteControl/UI", "RemoteControl/App", "Shared/UI"], function (require, exports, UI_js_1, App_js_4, UI_js_2) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ReceiveFile = exports.UploadFiles = void 0;
    const PartialDownloads = {};
    async function UploadFiles(fileList) {
        if (!UI_js_1.FileTransferProgress.parentElement.hasAttribute("hidden")) {
            UI_js_1.FileTransferInput.value = null;
            UI_js_2.ShowMessage("File transfer already in progress.");
            return;
        }
        UI_js_2.ShowMessage("File upload started...");
        UI_js_1.FileTransferProgress.value = 0;
        UI_js_1.FileTransferProgress.parentElement.removeAttribute("hidden");
        try {
            for (var i = 0; i < fileList.length; i++) {
                UI_js_1.FileTransferNameSpan.innerHTML = fileList[i].name;
                var buffer = await fileList[i].arrayBuffer();
                await App_js_4.ViewerApp.MessageSender.SendFile(new Uint8Array(buffer), fileList[i].name);
            }
            UI_js_2.ShowMessage("File upload completed.");
        }
        catch (_a) {
            UI_js_2.ShowMessage("File upload failed.");
        }
        UI_js_1.FileTransferInput.value = null;
        UI_js_1.FileTransferProgress.parentElement.setAttribute("hidden", "hidden");
    }
    exports.UploadFiles = UploadFiles;
    async function ReceiveFile(file) {
        if (file.StartOfFile) {
            UI_js_2.ShowMessage(`Downloading file ${file.FileName}...`);
        }
        var partial = PartialDownloads[file.MessageId];
        if (!partial) {
            partial = new Array();
            PartialDownloads[file.MessageId] = partial;
        }
        if (file.Buffer) {
            partial.push(file.Buffer);
        }
        if (file.EndOfFile) {
            var blob = new Blob(partial, { type: 'application/octet-stream' });
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement('a');
            link.style.display = 'none';
            link.href = url;
            link.download = file.FileName;
            document.body.appendChild(link);
            link.click();
            setTimeout(() => {
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            }, 100);
        }
    }
    exports.ReceiveFile = ReceiveFile;
});
define("RemoteControl/DtoMessageHandler", ["require", "exports", "RemoteControl/UI", "Shared/Enums/BaseDtoType", "RemoteControl/App", "Shared/UI", "Shared/Sound", "RemoteControl/FileTransferService"], function (require, exports, UI, BaseDtoType_js_2, App_js_5, UI_js_3, Sound_js_1, FileTransferService_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.DtoMessageHandler = void 0;
    class DtoMessageHandler {
        constructor() {
            this.MessagePack = window['MessagePack'];
            this.PartialCaptureFrames = [];
        }
        ParseBinaryMessage(data) {
            var model = this.MessagePack.decode(data);
            switch (model.DtoType) {
                case BaseDtoType_js_2.BaseDtoType.AudioSample:
                    this.HandleAudioSample(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.CaptureFrame:
                    this.HandleCaptureFrame(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.ClipboardText:
                    this.HandleClipboardText(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.CursorChange:
                    this.HandleCursorChange(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.MachineName:
                    this.HandleMachineName(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.ScreenData:
                    this.HandleScreenData(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.ScreenSize:
                    this.HandleScreenSize(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.WindowsSessions:
                    this.HandleWindowsSessions(model);
                    break;
                case BaseDtoType_js_2.BaseDtoType.File:
                    this.HandleFile(model);
                default:
                    break;
            }
        }
        HandleAudioSample(audioSample) {
            Sound_js_1.Sound.Play(audioSample.Buffer);
        }
        HandleCaptureFrame(captureFrame) {
            if (UI.AutoQualityAdjustCheckBox.checked &&
                Number(UI.QualitySlider.value) != captureFrame.ImageQuality) {
                UI.QualitySlider.value = String(captureFrame.ImageQuality);
            }
            if (captureFrame.EndOfFrame) {
                App_js_5.ViewerApp.MessageSender.SendFrameReceived();
                var url = window.URL.createObjectURL(new Blob(this.PartialCaptureFrames));
                var img = document.createElement("img");
                img.onload = () => {
                    UI.Screen2DContext.drawImage(img, captureFrame.Left, captureFrame.Top, captureFrame.Width, captureFrame.Height);
                    window.URL.revokeObjectURL(url);
                };
                img.src = url;
                this.PartialCaptureFrames = [];
            }
            else {
                this.PartialCaptureFrames.push(captureFrame.ImageBytes);
            }
        }
        HandleClipboardText(clipboardText) {
            App_js_5.ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
            UI_js_3.ShowMessage("Clipboard updated.");
        }
        HandleCursorChange(cursorChange) {
            UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
        }
        HandleFile(file) {
            FileTransferService_js_1.ReceiveFile(file);
        }
        HandleMachineName(machineNameDto) {
            document.title = `${machineNameDto.MachineName} - Remotely Session`;
        }
        HandleScreenData(screenDataDto) {
            UI.UpdateDisplays(screenDataDto.SelectedScreen, screenDataDto.DisplayNames);
        }
        HandleScreenSize(screenSizeDto) {
            UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
        }
        HandleWindowsSessions(windowsSessionsDto) {
            UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
        }
    }
    exports.DtoMessageHandler = DtoMessageHandler;
});
define("RemoteControl/MessageSender", ["require", "exports", "RemoteControl/App", "RemoteControl/Dtos", "Shared/Utilities", "RemoteControl/UI", "Shared/Enums/BaseDtoType", "Shared/Enums/RemoteControlMode"], function (require, exports, App_js_6, Dtos_js_1, Utilities_js_3, UI_js_4, BaseDtoType_js_3, RemoteControlMode_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.MessageSender = void 0;
    class MessageSender {
        GetWindowsSessions() {
            if (App_js_6.ViewerApp.Mode == RemoteControlMode_js_1.RemoteControlMode.Unattended) {
                var dto = new Dtos_js_1.WindowsSessionsDto();
                this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
            }
        }
        ChangeWindowsSession(sessionId) {
            App_js_6.ViewerApp.ViewerHubConnection.ChangeWindowsSession(sessionId);
        }
        SendFrameReceived() {
            var dto = new Dtos_js_1.GenericDto(BaseDtoType_js_3.BaseDtoType.FrameReceived);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendSelectScreen(displayName) {
            var dto = new Dtos_js_1.SelectScreenDto(displayName);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendMouseMove(percentX, percentY) {
            var dto = new Dtos_js_1.MouseMoveDto(percentX, percentY);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendMouseDown(button, percentX, percentY) {
            var dto = new Dtos_js_1.MouseDownDto(button, percentX, percentY);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendMouseUp(button, percentX, percentY) {
            var dto = new Dtos_js_1.MouseUpDto(button, percentX, percentY);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendTap(percentX, percentY) {
            var dto = new Dtos_js_1.TapDto(percentX, percentY);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendMouseWheel(deltaX, deltaY) {
            var dto = new Dtos_js_1.MouseWheelDto(deltaX, deltaY);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendKeyDown(key) {
            var dto = new Dtos_js_1.KeyDownDto(key);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendKeyUp(key) {
            var dto = new Dtos_js_1.KeyUpDto(key);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendKeyPress(key) {
            var dto = new Dtos_js_1.KeyPressDto(key);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendSetKeyStatesUp() {
            var dto = new Dtos_js_1.GenericDto(BaseDtoType_js_3.BaseDtoType.SetKeyStatesUp);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendCtrlAltDel() {
            var dto = new Dtos_js_1.CtrlAltDelDto();
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendOpenFileTransferWindow() {
            var dto = new Dtos_js_1.GenericDto(BaseDtoType_js_3.BaseDtoType.OpenFileTransferWindow);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        async SendFile(buffer, fileName) {
            var messageId = Utilities_js_3.CreateGUID();
            let dto = new Dtos_js_1.FileDto(null, fileName, messageId, false, true);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
            for (var i = 0; i < buffer.byteLength; i += 50000) {
                let dto = new Dtos_js_1.FileDto(buffer.slice(i, i + 50000), fileName, messageId, false, false);
                await this.SendToAgentAsync(async () => {
                    App_js_6.ViewerApp.RtcSession.SendDto(dto);
                    await Utilities_js_3.When(() => App_js_6.ViewerApp.RtcSession.DataChannel.bufferedAmount == 0, 10);
                }, async () => {
                    await App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto);
                });
                if (i > 0) {
                    UI_js_4.FileTransferProgress.value = i / buffer.byteLength;
                }
            }
            dto = new Dtos_js_1.FileDto(null, fileName, messageId, true, false);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendQualityChange(qualityLevel) {
            var dto = new Dtos_js_1.QualityChangeDto(qualityLevel);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendAutoQualityAdjust(isOn) {
            var dto = new Dtos_js_1.AutoQualityAdjustDto(isOn);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendToggleAudio(toggleOn) {
            var dto = new Dtos_js_1.ToggleAudioDto(toggleOn);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        ;
        SendToggleBlockInput(toggleOn) {
            var dto = new Dtos_js_1.ToggleBlockInputDto(toggleOn);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendToggleWebRtcVideo(toggleOn) {
            var dto = new Dtos_js_1.ToggleWebRtcVideoDto(toggleOn);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        SendClipboardTransfer(text, typeText) {
            var dto = new Dtos_js_1.ClipboardTransferDto(text, typeText);
            this.SendToAgent(() => App_js_6.ViewerApp.RtcSession.SendDto(dto), () => App_js_6.ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
        IsWebRtcAvailable() {
            return App_js_6.ViewerApp.RtcSession.DataChannel && App_js_6.ViewerApp.RtcSession.DataChannel.readyState == "open";
        }
        SendToAgent(rtcSend, websocketSend) {
            if (App_js_6.ViewerApp.RtcSession.DataChannel && App_js_6.ViewerApp.RtcSession.DataChannel.readyState == "open") {
                rtcSend();
            }
            else if (App_js_6.ViewerApp.ViewerHubConnection.Connection.connectionStarted) {
                websocketSend();
            }
        }
        async SendToAgentAsync(rtcSend, websocketSend) {
            if (this.IsWebRtcAvailable()) {
                await rtcSend();
            }
            else {
                await websocketSend();
            }
        }
    }
    exports.MessageSender = MessageSender;
});
define("RemoteControl/SessionRecorder", ["require", "exports", "RemoteControl/UI"], function (require, exports, UI_js_5) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.SessionRecorder = void 0;
    class SessionRecorder {
        constructor() {
            this.RecordedData = [];
        }
        Start() {
            if (!window["MediaRecorder"] || !UI_js_5.GetCurrentViewer().captureStream) {
                alert("Session recording isn't supported on this browser.");
                return;
            }
            if (this.Recorder && this.Recorder.state != "inactive") {
                this.Recorder.stop();
            }
            this.RecordedData = [];
            this.Stream = UI_js_5.GetCurrentViewer().captureStream(10);
            var options = { mimeType: 'video/webm' };
            this.Recorder = new window["MediaRecorder"](this.Stream, options);
            this.Recorder.ondataavailable = (event) => {
                if (event.data && event.data.size > 0) {
                    this.RecordedData.push(event.data);
                }
            };
            this.Recorder.start(100);
        }
        Stop() {
            if (!this.Recorder) {
                return;
            }
            this.Recorder.stop();
        }
        DownloadVideo() {
            if (!this.RecordedData || this.RecordedData.length == 0) {
                alert("No video recorded.");
                return;
            }
            if (this.Recorder && this.Recorder.state != "inactive") {
                alert("You must stop recording before you can download.");
                return;
            }
            var currentDate = new Date();
            var dateString = `${currentDate.getFullYear()}-` +
                `${(currentDate.getMonth() + 1).toString().padStart(2, "0")}-` +
                `${currentDate.getDate().toString().padStart(2, "0")} ` +
                `${currentDate.getHours().toString().padStart(2, "0")}.` +
                `${currentDate.getMinutes().toString().padStart(2, "0")}.` +
                `${currentDate.getSeconds().toString().padStart(2, "0")}`;
            var blob = new Blob(this.RecordedData, { type: 'video/webm' });
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement('a');
            link.style.display = 'none';
            link.href = url;
            link.download = `Remote_Session_${dateString}.webm`;
            document.body.appendChild(link);
            link.click();
            setTimeout(() => {
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            }, 100);
        }
    }
    exports.SessionRecorder = SessionRecorder;
});
define("RemoteControl/InputEventHandlers", ["require", "exports", "RemoteControl/UI", "Shared/Sound", "RemoteControl/App", "RemoteControl/FileTransferService", "Shared/Enums/RemoteControlMode", "Shared/Utilities", "Shared/UI"], function (require, exports, UI_js_6, Sound_js_2, App_js_7, FileTransferService_js_2, RemoteControlMode_js_2, Utilities_js_4, UI_js_7) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ApplyInputHandlers = void 0;
    var lastPointerMove = Date.now();
    var isDragging;
    var currentPointerDevice;
    var currentTouchCount;
    var cancelNextViewerClick;
    var isPinchZooming;
    var startPinchPoint1;
    var startPinchPoint2;
    var lastPinchDistance;
    var isMenuButtonDragging;
    var startMenuDraggingY;
    var startLongPressTimeout;
    var lastPinchCenterX;
    var lastPinchCenterY;
    function ApplyInputHandlers() {
        UI_js_6.AudioButton.addEventListener("click", (ev) => {
            UI_js_6.AudioButton.classList.toggle("toggled");
            var toggleOn = UI_js_6.AudioButton.classList.contains("toggled");
            if (toggleOn) {
                Sound_js_2.Sound.Init();
            }
            App_js_7.ViewerApp.MessageSender.SendToggleAudio(toggleOn);
        });
        UI_js_6.ChangeScreenButton.addEventListener("click", (ev) => {
            closeAllHorizontalBars("screenSelectBar");
            UI_js_6.ScreenSelectBar.classList.toggle("open");
        });
        UI_js_6.ClipboardTransferButton.addEventListener("click", (ev) => {
            closeAllHorizontalBars("clipboardTransferBar");
            UI_js_6.ClipboardTransferBar.classList.toggle("open");
        });
        UI_js_6.TypeClipboardButton.addEventListener("click", (ev) => {
            if (!navigator.clipboard.readText) {
                alert("Clipboard access isn't supported on this browser.");
                return;
            }
            navigator.clipboard.readText().then(text => {
                App_js_7.ViewerApp.MessageSender.SendClipboardTransfer(text, true);
                UI_js_7.ShowMessage("Clipboard sent!");
            }, reason => {
                alert("Unable to read clipboard.  Please check your permissions.");
                console.log("Unable to read clipboard.  Reason: " + reason);
            });
        });
        UI_js_6.ConnectButton.addEventListener("click", (ev) => {
            App_js_7.ViewerApp.ConnectToClient();
        });
        UI_js_6.CtrlAltDelButton.addEventListener("click", (ev) => {
            if (!App_js_7.ViewerApp.ServiceID) {
                UI_js_7.ShowMessage("Not available for this session.");
                return;
            }
            closeAllHorizontalBars(null);
            App_js_7.ViewerApp.MessageSender.SendCtrlAltDel();
        });
        UI_js_6.DisconnectButton.addEventListener("click", (ev) => {
            UI_js_6.ConnectButton.removeAttribute("disabled");
            App_js_7.ViewerApp.ViewerHubConnection.Connection.stop();
            if (location.search.includes("fromApi=true")) {
                window.close();
            }
        });
        document.querySelectorAll("#sessionIDInput, #nameInput").forEach(x => {
            x.addEventListener("keypress", (ev) => {
                if (ev.key.toLowerCase() == "enter") {
                    App_js_7.ViewerApp.ConnectToClient();
                }
            });
        });
        UI_js_6.FileTransferButton.addEventListener("click", (ev) => {
            closeAllHorizontalBars(UI_js_6.FileTransferBar.id);
            UI_js_6.FileTransferBar.classList.toggle("open");
        });
        UI_js_6.FileUploadButtton.addEventListener("click", (ev) => {
            UI_js_6.FileTransferInput.click();
        });
        UI_js_6.FileDownloadButton.addEventListener("click", (ev) => {
            App_js_7.ViewerApp.MessageSender.SendOpenFileTransferWindow();
        });
        UI_js_6.FileTransferInput.addEventListener("change", (ev) => {
            FileTransferService_js_2.UploadFiles(UI_js_6.FileTransferInput.files);
        });
        UI_js_6.FitToScreenButton.addEventListener("click", (ev) => {
            UI_js_6.FitToScreenButton.classList.toggle("toggled");
            if (UI_js_6.FitToScreenButton.classList.contains("toggled")) {
                UI_js_6.ScreenViewer.style.removeProperty("max-width");
                UI_js_6.ScreenViewer.style.removeProperty("max-height");
                UI_js_6.VideoScreenViewer.style.removeProperty("max-width");
                UI_js_6.VideoScreenViewer.style.removeProperty("max-height");
            }
            else {
                UI_js_6.ScreenViewer.style.maxWidth = "unset";
                UI_js_6.ScreenViewer.style.maxHeight = "unset";
                UI_js_6.VideoScreenViewer.style.maxWidth = "unset";
                UI_js_6.VideoScreenViewer.style.maxHeight = "unset";
            }
        });
        UI_js_6.BlockInputButton.addEventListener("click", (ev) => {
            UI_js_6.BlockInputButton.classList.toggle("toggled");
            if (UI_js_6.BlockInputButton.classList.contains("toggled")) {
                App_js_7.ViewerApp.MessageSender.SendToggleBlockInput(true);
            }
            else {
                App_js_7.ViewerApp.MessageSender.SendToggleBlockInput(false);
            }
        });
        UI_js_6.InviteButton.addEventListener("click", (ev) => {
            var url = "";
            if (App_js_7.ViewerApp.Mode == RemoteControlMode_js_2.RemoteControlMode.Normal) {
                url = `${location.origin}${location.pathname}?sessionID=${App_js_7.ViewerApp.ClientID}`;
            }
            else {
                url = `${location.origin}${location.pathname}?clientID=${App_js_7.ViewerApp.ClientID}&serviceID=${App_js_7.ViewerApp.ServiceID}`;
            }
            App_js_7.ViewerApp.ClipboardWatcher.SetClipboardText(url);
            UI_js_7.ShowMessage("Link copied to clipboard.");
        });
        UI_js_6.KeyboardButton.addEventListener("click", (ev) => {
            closeAllHorizontalBars(null);
            UI_js_6.TouchKeyboardTextArea.focus();
            UI_js_6.TouchKeyboardTextArea.setSelectionRange(UI_js_6.TouchKeyboardTextArea.value.length, UI_js_6.TouchKeyboardTextArea.value.length);
            UI_js_6.MenuFrame.classList.remove("open");
            UI_js_6.MenuButton.classList.remove("open");
        });
        UI_js_6.MenuButton.addEventListener("click", (ev) => {
            if (isMenuButtonDragging) {
                isMenuButtonDragging = false;
                return;
            }
            UI_js_6.MenuFrame.classList.toggle("open");
            UI_js_6.MenuButton.classList.toggle("open");
            closeAllHorizontalBars(null);
        });
        UI_js_6.MenuButton.addEventListener("mousedown", (ev) => {
            isMenuButtonDragging = false;
            startMenuDraggingY = ev.clientY;
            window.addEventListener("mousemove", moveMenuButton);
            window.addEventListener("mouseup", removeMouseButtonWindowListeners);
            window.addEventListener("mouseleave", removeMouseButtonWindowListeners);
        });
        UI_js_6.MenuButton.addEventListener("touchmove", (ev) => {
            ev.preventDefault();
            ev.stopPropagation();
            UI_js_6.MenuButton.style.top = `${ev.touches[0].clientY}px`;
        });
        UI_js_6.QualityButton.addEventListener("click", (ev) => {
            closeAllHorizontalBars("qualityBar");
            UI_js_6.QualityBar.classList.toggle("open");
        });
        UI_js_6.QualitySlider.addEventListener("change", (ev) => {
            App_js_7.ViewerApp.MessageSender.SendQualityChange(Number(UI_js_6.QualitySlider.value));
        });
        UI_js_6.StreamVideoButton.addEventListener("click", (ev) => {
            UI_js_6.StreamVideoButton.classList.toggle("toggled");
            if (UI_js_6.StreamVideoButton.classList.contains("toggled")) {
                App_js_7.ViewerApp.MessageSender.SendToggleWebRtcVideo(true);
                UI_js_6.VideoScreenViewer.removeAttribute("hidden");
                UI_js_6.ScreenViewer.setAttribute("hidden", "hidden");
                UI_js_6.QualityButton.setAttribute("hidden", "hidden");
            }
            else {
                App_js_7.ViewerApp.MessageSender.SendToggleWebRtcVideo(false);
                UI_js_6.ScreenViewer.removeAttribute("hidden");
                UI_js_6.QualityButton.removeAttribute("hidden");
                UI_js_6.VideoScreenViewer.setAttribute("hidden", "hidden");
            }
        });
        UI_js_6.AutoQualityAdjustCheckBox.addEventListener("change", ev => {
            App_js_7.ViewerApp.MessageSender.SendAutoQualityAdjust(UI_js_6.AutoQualityAdjustCheckBox.checked);
        });
        [UI_js_6.ScreenViewer, UI_js_6.VideoScreenViewer].forEach(viewer => {
            viewer.addEventListener("pointermove", function (e) {
                currentPointerDevice = e.pointerType;
            });
            viewer.addEventListener("pointerdown", function (e) {
                currentPointerDevice = e.pointerType;
            });
            viewer.addEventListener("pointerenter", function (e) {
                currentPointerDevice = e.pointerType;
            });
            viewer.addEventListener("mousemove", function (e) {
                e.preventDefault();
                if (Date.now() - lastPointerMove < 25) {
                    return;
                }
                lastPointerMove = Date.now();
                var percentX = e.offsetX / viewer.clientWidth;
                var percentY = e.offsetY / viewer.clientHeight;
                App_js_7.ViewerApp.MessageSender.SendMouseMove(percentX, percentY);
            });
            viewer.addEventListener("mousedown", function (e) {
                if (currentPointerDevice == "touch") {
                    return;
                }
                if (e.button != 0 && e.button != 2) {
                    return;
                }
                e.preventDefault();
                var percentX = e.offsetX / viewer.clientWidth;
                var percentY = e.offsetY / viewer.clientHeight;
                App_js_7.ViewerApp.MessageSender.SendMouseDown(e.button, percentX, percentY);
            });
            viewer.addEventListener("mouseup", function (e) {
                if (currentPointerDevice == "touch") {
                    return;
                }
                if (e.button != 0 && e.button != 2) {
                    return;
                }
                e.preventDefault();
                var percentX = e.offsetX / viewer.clientWidth;
                var percentY = e.offsetY / viewer.clientHeight;
                App_js_7.ViewerApp.MessageSender.SendMouseUp(e.button, percentX, percentY);
            });
            viewer.addEventListener("click", function (e) {
                if (cancelNextViewerClick) {
                    cancelNextViewerClick = false;
                    return;
                }
                if (currentPointerDevice == "mouse") {
                    e.preventDefault();
                    e.stopPropagation();
                }
                else if (currentPointerDevice == "touch" && currentTouchCount == 0) {
                    var percentX = e.offsetX / viewer.clientWidth;
                    var percentY = e.offsetY / viewer.clientHeight;
                    App_js_7.ViewerApp.MessageSender.SendTap(percentX, percentY);
                }
            });
            viewer.addEventListener("touchstart", function (e) {
                currentTouchCount = e.touches.length;
                if (currentTouchCount == 1) {
                    startLongPressTimeout = window.setTimeout(() => {
                        var percentX = e.touches[0].pageX / viewer.clientWidth;
                        var percentY = e.touches[0].pageY / viewer.clientHeight;
                        App_js_7.ViewerApp.MessageSender.SendMouseDown(2, percentX, percentY);
                        App_js_7.ViewerApp.MessageSender.SendMouseUp(2, percentX, percentY);
                    }, 1000);
                }
                if (currentTouchCount > 1) {
                    cancelNextViewerClick = true;
                }
                if (currentTouchCount == 2) {
                    startPinchPoint1 = { X: e.touches[0].pageX, Y: e.touches[0].pageY, IsEmpty: false };
                    startPinchPoint2 = { X: e.touches[1].pageX, Y: e.touches[1].pageY, IsEmpty: false };
                    lastPinchDistance = Utilities_js_4.GetDistanceBetween(startPinchPoint1.X, startPinchPoint1.Y, startPinchPoint2.X, startPinchPoint2.Y);
                    lastPinchCenterX = (startPinchPoint1.X + startPinchPoint2.X) / 2;
                    lastPinchCenterY = (startPinchPoint1.Y + startPinchPoint2.Y) / 2;
                }
                isDragging = false;
                UI_js_6.KeyboardButton.removeAttribute("hidden");
                var focusedInput = document.querySelector("input:focus");
                if (focusedInput) {
                    focusedInput.blur();
                }
            });
            viewer.addEventListener("touchmove", function (e) {
                currentTouchCount = e.touches.length;
                clearTimeout(startLongPressTimeout);
                if (e.touches.length == 2) {
                    var pinchPoint1 = {
                        X: e.touches[0].pageX,
                        Y: e.touches[0].pageY,
                        IsEmpty: false
                    };
                    var pinchPoint2 = {
                        X: e.touches[1].pageX,
                        Y: e.touches[1].pageY,
                        IsEmpty: false
                    };
                    var pinchDistance = Utilities_js_4.GetDistanceBetween(pinchPoint1.X, pinchPoint1.Y, pinchPoint2.X, pinchPoint2.Y);
                    var pinchCenterX = (pinchPoint1.X + pinchPoint2.X) / 2;
                    var pinchCenterY = (pinchPoint1.Y + pinchPoint2.Y) / 2;
                    UI_js_6.ScreenViewerWrapper.scrollBy(lastPinchCenterX - pinchCenterX, lastPinchCenterY - pinchCenterY);
                    lastPinchCenterX = pinchCenterX;
                    lastPinchCenterY = pinchCenterY;
                    if (Math.abs(pinchDistance - lastPinchDistance) > 5) {
                        isPinchZooming = true;
                        if (UI_js_6.FitToScreenButton.classList.contains("toggled")) {
                            UI_js_6.FitToScreenButton.click();
                        }
                        var currentWidth = viewer.clientWidth;
                        var currentHeight = viewer.clientHeight;
                        var clientAdjustedScrollLeftPercent = (UI_js_6.ScreenViewerWrapper.scrollLeft + (UI_js_6.ScreenViewerWrapper.clientWidth * .5)) / UI_js_6.ScreenViewerWrapper.scrollWidth;
                        var clientAdjustedScrollTopPercent = (UI_js_6.ScreenViewerWrapper.scrollTop + (UI_js_6.ScreenViewerWrapper.clientHeight * .5)) / UI_js_6.ScreenViewerWrapper.scrollHeight;
                        var currentWidthPercent = Number(viewer.style.width.slice(0, -1));
                        var newWidthPercent = Math.max(100, (currentWidthPercent + (pinchDistance - lastPinchDistance) * (currentWidthPercent / 100)));
                        newWidthPercent = Math.min(5000, newWidthPercent);
                        viewer.style.width = String(newWidthPercent) + "%";
                        var heightChange = viewer.clientHeight - currentHeight;
                        var widthChange = viewer.clientWidth - currentWidth;
                        var pinchAdjustX = pinchCenterX / window.innerWidth - .5;
                        var pinchAdjustY = pinchCenterY / window.innerHeight - .5;
                        var scrollByX = widthChange * (clientAdjustedScrollLeftPercent + (pinchAdjustX * UI_js_6.ScreenViewerWrapper.clientWidth / UI_js_6.ScreenViewerWrapper.scrollWidth));
                        var scrollByY = heightChange * (clientAdjustedScrollTopPercent + (pinchAdjustY * UI_js_6.ScreenViewerWrapper.clientHeight / UI_js_6.ScreenViewerWrapper.scrollHeight));
                        UI_js_6.ScreenViewerWrapper.scrollBy(scrollByX, scrollByY);
                        lastPinchDistance = pinchDistance;
                    }
                    return;
                }
                else if (isDragging) {
                    e.preventDefault();
                    e.stopPropagation();
                    var screenViewerLeft = viewer.getBoundingClientRect().left;
                    var screenViewerTop = viewer.getBoundingClientRect().top;
                    var pagePercentX = (e.touches[0].pageX - screenViewerLeft) / viewer.clientWidth;
                    var pagePercentY = (e.touches[0].pageY - screenViewerTop) / viewer.clientHeight;
                    App_js_7.ViewerApp.MessageSender.SendMouseMove(pagePercentX, pagePercentY);
                }
            });
            viewer.addEventListener("touchend", function (e) {
                currentTouchCount = e.touches.length;
                clearTimeout(startLongPressTimeout);
                if (e.touches.length == 1 && !isPinchZooming) {
                    isDragging = true;
                    var percentX = (e.touches[0].pageX - viewer.getBoundingClientRect().left) / viewer.clientWidth;
                    var percentY = (e.touches[0].pageY - viewer.getBoundingClientRect().top) / viewer.clientHeight;
                    App_js_7.ViewerApp.MessageSender.SendMouseMove(percentX, percentY);
                    App_js_7.ViewerApp.MessageSender.SendMouseDown(0, percentX, percentY);
                    return;
                }
                if (currentTouchCount == 0) {
                    cancelNextViewerClick = false;
                    isPinchZooming = false;
                    startPinchPoint1 = null;
                    startPinchPoint2 = null;
                }
                if (isDragging) {
                    var percentX = (e.changedTouches[0].pageX - viewer.getBoundingClientRect().left) / viewer.clientWidth;
                    var percentY = (e.changedTouches[0].pageY - viewer.getBoundingClientRect().top) / viewer.clientHeight;
                    App_js_7.ViewerApp.MessageSender.SendMouseUp(0, percentX, percentY);
                }
                isDragging = false;
            });
            viewer.addEventListener("contextmenu", (ev) => {
                ev.preventDefault();
            });
            viewer.addEventListener("wheel", function (e) {
                e.preventDefault();
                App_js_7.ViewerApp.MessageSender.SendMouseWheel(e.deltaX, e.deltaY);
            });
        });
        UI_js_6.TouchKeyboardTextArea.addEventListener("input", (ev) => {
            if (UI_js_6.TouchKeyboardTextArea.value.length == 1) {
                App_js_7.ViewerApp.MessageSender.SendKeyPress("Backspace");
            }
            else if (UI_js_6.TouchKeyboardTextArea.value.endsWith("\n")) {
                App_js_7.ViewerApp.MessageSender.SendKeyPress("Enter");
            }
            else if (UI_js_6.TouchKeyboardTextArea.value.endsWith(" ")) {
                App_js_7.ViewerApp.MessageSender.SendKeyPress(" ");
            }
            else {
                var input = UI_js_6.TouchKeyboardTextArea.value.trim().substr(1);
                for (var i = 0; i < input.length; i++) {
                    var character = input.charAt(i);
                    var sendShift = character.match(/[A-Z~!@#$%^&*()_+{}|<>?]/);
                    if (sendShift) {
                        App_js_7.ViewerApp.MessageSender.SendKeyDown("Shift");
                    }
                    App_js_7.ViewerApp.MessageSender.SendKeyPress(character);
                    if (sendShift) {
                        App_js_7.ViewerApp.MessageSender.SendKeyUp("Shift");
                    }
                }
            }
            window.setTimeout(() => {
                UI_js_6.TouchKeyboardTextArea.value = " #";
                UI_js_6.TouchKeyboardTextArea.setSelectionRange(UI_js_6.TouchKeyboardTextArea.value.length, UI_js_6.TouchKeyboardTextArea.value.length);
            });
        });
        UI_js_6.WindowsSessionSelect.addEventListener("focus", () => {
            App_js_7.ViewerApp.MessageSender.GetWindowsSessions();
        });
        UI_js_6.WindowsSessionSelect.addEventListener("change", () => {
            UI_js_7.ShowMessage("Switching sessions...");
            App_js_7.ViewerApp.MessageSender.ChangeWindowsSession(Number(UI_js_6.WindowsSessionSelect.selectedOptions[0].value));
        });
        UI_js_6.RecordSessionButton.addEventListener("click", () => {
            UI_js_6.RecordSessionButton.classList.toggle("toggled");
            if (UI_js_6.RecordSessionButton.classList.contains("toggled")) {
                UI_js_6.RecordSessionButton.innerHTML = `Stop <i class="fas fa-record-vinyl">`;
                App_js_7.ViewerApp.SessionRecorder.Start();
            }
            else {
                UI_js_6.RecordSessionButton.innerHTML = `Start <i class="fas fa-record-vinyl">`;
                App_js_7.ViewerApp.SessionRecorder.Stop();
            }
        });
        UI_js_6.DownloadRecordingButton.addEventListener("click", () => {
            App_js_7.ViewerApp.SessionRecorder.DownloadVideo();
        });
        window.addEventListener("keydown", function (e) {
            if (document.querySelector("input:focus") || document.querySelector("textarea:focus")) {
                return;
            }
            e.preventDefault();
            App_js_7.ViewerApp.MessageSender.SendKeyDown(e.key);
        });
        window.addEventListener("keyup", function (e) {
            if (document.querySelector("input:focus") || document.querySelector("textarea:focus")) {
                return;
            }
            e.preventDefault();
            App_js_7.ViewerApp.MessageSender.SendKeyUp(e.key);
        });
        window.addEventListener("blur", () => {
            App_js_7.ViewerApp.MessageSender.SendSetKeyStatesUp();
        });
        window.ondragover = function (e) {
            e.preventDefault();
            e.dataTransfer.dropEffect = "copy";
        };
        window.ondrop = function (e) {
            e.preventDefault();
            if (e.dataTransfer.files.length < 1) {
                return;
            }
            FileTransferService_js_2.UploadFiles(e.dataTransfer.files);
        };
    }
    exports.ApplyInputHandlers = ApplyInputHandlers;
    function closeAllHorizontalBars(exceptBarId) {
        UI_js_6.HorizontalBars.forEach(x => {
            if (x.id != exceptBarId) {
                x.classList.remove('open');
            }
        });
    }
    function moveMenuButton(ev) {
        if (Math.abs(ev.clientY - startMenuDraggingY) > 5) {
            if (ev.clientY < 0 || ev.clientY > window.innerHeight) {
                return;
            }
            isMenuButtonDragging = true;
            UI_js_6.MenuButton.style.top = `${ev.clientY}px`;
        }
    }
    function removeMouseButtonWindowListeners(ev) {
        window.removeEventListener("mousemove", moveMenuButton);
        window.removeEventListener("mouseup", removeMouseButtonWindowListeners);
        window.removeEventListener("mouseleave", removeMouseButtonWindowListeners);
    }
});
define("RemoteControl/ViewerHubConnection", ["require", "exports", "RemoteControl/UI", "RemoteControl/App", "Shared/Enums/RemoteControlMode", "RemoteControl/Dtos", "Shared/UI", "Shared/Enums/BaseDtoType"], function (require, exports, UI, App_js_8, RemoteControlMode_js_3, Dtos_js_2, UI_js_8, BaseDtoType_js_4) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ViewerHubConnection = void 0;
    var signalR = window["signalR"];
    class ViewerHubConnection {
        constructor() {
            this.MessagePack = window['MessagePack'];
            this.PartialCaptureFrames = [];
        }
        Connect() {
            this.Connection = new signalR.HubConnectionBuilder()
                .withUrl("/ViewerHub")
                .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
                .configureLogging(signalR.LogLevel.Information)
                .build();
            this.ApplyMessageHandlers(this.Connection);
            this.Connection.start().then(() => {
                this.SendScreenCastRequestToDevice();
                this.ToggleConnectUI(false);
            }).catch(err => {
                console.error(err.toString());
                console.log("Connection closed.");
                UI.StatusMessage.innerHTML = `Connection error: ${err.message}`;
                this.ToggleConnectUI(true);
            });
            this.Connection.closedCallbacks.push((ev) => {
                this.ToggleConnectUI(true);
            });
            App_js_8.ViewerApp.ClipboardWatcher.WatchClipboard();
        }
        ChangeWindowsSession(sessionID) {
            if (App_js_8.ViewerApp.Mode == RemoteControlMode_js_3.RemoteControlMode.Unattended) {
                this.Connection.invoke("ChangeWindowsSession", sessionID);
            }
        }
        SendDtoToClient(dto) {
            return this.Connection.invoke("SendDtoToClient", this.MessagePack.encode(dto));
        }
        SendIceCandidate(candidate) {
            if (candidate) {
                this.Connection.invoke("SendIceCandidateToAgent", candidate.candidate, candidate.sdpMLineIndex, candidate.sdpMid);
            }
            else {
                this.Connection.invoke("SendIceCandidateToAgent", "", 0, "");
            }
        }
        SendRtcAnswer(sessionDescription) {
            this.Connection.invoke("SendRtcAnswerToAgent", sessionDescription.sdp);
        }
        SendScreenCastRequestToDevice() {
            this.Connection.invoke("SendScreenCastRequestToDevice", App_js_8.ViewerApp.ClientID, App_js_8.ViewerApp.RequesterName, App_js_8.ViewerApp.Mode, App_js_8.ViewerApp.Otp);
        }
        ToggleConnectUI(shown) {
            if (shown) {
                UI.Screen2DContext.clearRect(0, 0, UI.ScreenViewer.width, UI.ScreenViewer.height);
                UI.ScreenViewer.setAttribute("hidden", "hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
                UI.ConnectBox.style.removeProperty("display");
                UI.StreamVideoButton.classList.remove("toggled");
                UI.BlockInputButton.classList.remove("toggled");
                UI.AudioButton.classList.remove("toggled");
            }
            else {
                UI.ConnectButton.removeAttribute("disabled");
                UI.ConnectBox.style.display = "none";
                UI.ScreenViewer.removeAttribute("hidden");
                UI.StatusMessage.innerHTML = "";
            }
        }
        ApplyMessageHandlers(hubConnection) {
            hubConnection.on("SendDtoToBrowser", (dto) => {
                App_js_8.ViewerApp.DtoMessageHandler.ParseBinaryMessage(dto);
            });
            hubConnection.on("ClipboardTextChanged", (clipboardText) => {
                App_js_8.ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText);
                UI_js_8.ShowMessage("Clipboard updated.");
            });
            hubConnection.on("ScreenData", (selectedDisplay, displayNames) => {
                UI.UpdateDisplays(selectedDisplay, displayNames);
            });
            hubConnection.on("ScreenSize", (width, height) => {
                UI.SetScreenSize(width, height);
            });
            hubConnection.on("ScreenCapture", (buffer, left, top, width, height, imageQuality, endOfFrame) => {
                if (UI.AutoQualityAdjustCheckBox.checked && Number(UI.QualitySlider.value) != imageQuality) {
                    UI.QualitySlider.value = String(imageQuality);
                }
                if (endOfFrame) {
                    this.SendDtoToClient(new Dtos_js_2.GenericDto(BaseDtoType_js_4.BaseDtoType.FrameReceived));
                    var url = window.URL.createObjectURL(new Blob(this.PartialCaptureFrames));
                    var img = document.createElement("img");
                    img.onload = () => {
                        UI.Screen2DContext.drawImage(img, left, top, width, height);
                        window.URL.revokeObjectURL(url);
                    };
                    img.src = url;
                    this.PartialCaptureFrames = [];
                }
                else {
                    this.PartialCaptureFrames.push(buffer);
                }
            });
            hubConnection.on("ConnectionFailed", () => {
                UI.ConnectButton.removeAttribute("disabled");
                UI.StatusMessage.innerHTML = "Connection failed or was denied.";
                UI_js_8.ShowMessage("Connection failed.  Please reconnect.");
                this.Connection.stop();
            });
            hubConnection.on("ConnectionRequestDenied", () => {
                this.Connection.stop();
                UI.StatusMessage.innerHTML = "Connection request denied.";
                UI_js_8.ShowMessage("Connection request denied.");
            });
            hubConnection.on("Unauthorized", () => {
                UI.ConnectButton.removeAttribute("disabled");
                UI.StatusMessage.innerHTML = "Authorization failed.";
                UI_js_8.ShowMessage("Authorization failed.");
                this.Connection.stop();
            });
            hubConnection.on("ViewerRemoved", () => {
                UI.ConnectButton.removeAttribute("disabled");
                UI.StatusMessage.innerHTML = "The session was stopped by your partner.";
                UI_js_8.ShowMessage("Session ended.");
                this.Connection.stop();
            });
            hubConnection.on("SessionIDNotFound", () => {
                UI.ConnectButton.removeAttribute("disabled");
                UI.StatusMessage.innerHTML = "Session ID not found.";
                this.Connection.stop();
            });
            hubConnection.on("ScreenCasterDisconnected", () => {
                UI.StatusMessage.innerHTML = "The host has disconnected.";
                this.Connection.stop();
            });
            hubConnection.on("ReceiveMachineName", (machineName) => {
                document.title = `${machineName} - Remotely Session`;
            });
            hubConnection.on("RelaunchedScreenCasterReady", (newClientID) => {
                App_js_8.ViewerApp.ClientID = newClientID;
                this.Connection.stop();
                this.Connect();
            });
            hubConnection.on("Reconnecting", () => {
                UI_js_8.ShowMessage("Reconnecting...");
            });
            hubConnection.on("CursorChange", (cursor) => {
                UI.UpdateCursor(cursor.ImageBytes, cursor.HotSpot.X, cursor.HotSpot.Y, cursor.CssOverride);
            });
            hubConnection.on("RequestingScreenCast", () => {
                UI_js_8.ShowMessage("Requesting remote control...");
            });
            hubConnection.on("ReceiveRtcOffer", async (sdp, iceServers) => {
                console.log("Rtc offer SDP received.");
                App_js_8.ViewerApp.RtcSession.Init(iceServers);
                await App_js_8.ViewerApp.RtcSession.ReceiveRtcOffer(sdp);
            });
            hubConnection.on("ReceiveIceCandidate", (candidate, sdpMlineIndex, sdpMid) => {
                console.log("Ice candidate received.");
                App_js_8.ViewerApp.RtcSession.ReceiveCandidate({
                    candidate: candidate,
                    sdpMLineIndex: sdpMlineIndex,
                    sdpMid: sdpMid
                });
            });
            hubConnection.on("ShowMessage", (message) => {
                UI_js_8.ShowMessage(message);
            });
            hubConnection.on("WindowsSessions", (windowsSessions) => {
                UI.UpdateWindowsSessions(windowsSessions);
            });
        }
    }
    exports.ViewerHubConnection = ViewerHubConnection;
});
define("RemoteControl/App", ["require", "exports", "Shared/Utilities", "RemoteControl/RtcSession", "RemoteControl/UI", "Shared/Enums/RemoteControlMode", "RemoteControl/ClipboardWatcher", "RemoteControl/DtoMessageHandler", "RemoteControl/MessageSender", "RemoteControl/SessionRecorder", "RemoteControl/InputEventHandlers", "RemoteControl/ViewerHubConnection"], function (require, exports, Utilities, RtcSession_js_1, UI, RemoteControlMode_js_4, ClipboardWatcher_js_1, DtoMessageHandler_js_1, MessageSender_js_1, SessionRecorder_js_1, InputEventHandlers_js_1, ViewerHubConnection_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ViewerApp = void 0;
    var queryString = Utilities.ParseSearchString();
    exports.ViewerApp = {
        ClipboardWatcher: new ClipboardWatcher_js_1.ClipboardWatcher(),
        MessageSender: new MessageSender_js_1.MessageSender(),
        ViewerHubConnection: new ViewerHubConnection_js_1.ViewerHubConnection(),
        DtoMessageHandler: new DtoMessageHandler_js_1.DtoMessageHandler(),
        RtcSession: new RtcSession_js_1.RtcSession(),
        SessionRecorder: new SessionRecorder_js_1.SessionRecorder(),
        ClientID: queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : "",
        Otp: queryString["otp"] ? decodeURIComponent(queryString["otp"]) : "",
        ServiceID: queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : "",
        RequesterName: queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "",
        Mode: RemoteControlMode_js_4.RemoteControlMode.None,
        Init: () => {
            InputEventHandlers_js_1.ApplyInputHandlers();
            if (queryString["clientID"]) {
                exports.ViewerApp.Mode = RemoteControlMode_js_4.RemoteControlMode.Unattended;
                UI.ConnectBox.style.display = "none";
                exports.ViewerApp.ViewerHubConnection.Connect();
            }
            else if (queryString["sessionID"]) {
                UI.SessionIDInput.value = decodeURIComponent(queryString["sessionID"]);
                if (queryString["requesterName"]) {
                    UI.RequesterNameInput.value = decodeURIComponent(queryString["requesterName"]);
                    exports.ViewerApp.ConnectToClient();
                }
            }
        },
        ConnectToClient: () => {
            UI.ConnectButton.disabled = true;
            exports.ViewerApp.ClientID = UI.SessionIDInput.value.split(" ").join("");
            exports.ViewerApp.RequesterName = UI.RequesterNameInput.value;
            exports.ViewerApp.Mode = RemoteControlMode_js_4.RemoteControlMode.Normal;
            exports.ViewerApp.ViewerHubConnection.Connect();
            UI.StatusMessage.innerHTML = "Sending connection request...";
        }
    };
});
//# sourceMappingURL=outViewer.js.map