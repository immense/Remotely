import {
    AudioButton,
    ChangeScreenButton,
    HorizontalBars,
    ScreenSelectBar,
    ClipboardTransferButton,
    ClipboardTransferBar,
    TypeClipboardButton,
    ConnectButton,
    CtrlAltDelButton,
    DisconnectButton,
    FileTransferButton,
    FileTransferInput,
    FitToScreenButton,
    ScreenViewer,
    BlockInputButton,
    InviteButton,
    KeyboardButton,
    TouchKeyboardTextArea,
    MenuFrame,
    MenuButton,
    QualityButton,
    QualityBar,
    QualitySlider,
    AutoQualityAdjustCheckBox,
    ScreenViewerWrapper,
    WindowsSessionSelect,
    RecordSessionButton,
    DownloadRecordingButton,
    VideoScreenViewer,

    StreamVideoButton
} from "./UI.js";
import { Sound } from "../Shared/Sound.js";
import { MainViewer } from "./Main.js";
import { Point } from "../Shared/Models/Point.js";
import { UploadFiles } from "./FileUploader.js";
import { RemoteControlMode } from "../Shared/Enums/RemoteControlMode.js";
import { GetDistanceBetween } from "../Shared/Utilities.js";
import { ShowMessage } from "../Shared/UI.js";

var lastPointerMove = Date.now();
var isDragging: boolean;
var currentPointerDevice: string;
var currentTouchCount: number;
var cancelNextViewerClick: boolean;
var isPinchZooming: boolean;
var startPinchPoint1: Point;
var startPinchPoint2: Point;
var lastPinchDistance: number;
var isMenuButtonDragging: boolean;
var startMenuDraggingY: number;
var startLongPressTimeout: number;
var lastPinchCenterX: number;
var lastPinchCenterY: number;

export function ApplyInputHandlers() {
    AudioButton.addEventListener("click", (ev) => {
        AudioButton.classList.toggle("toggled");
        var toggleOn = AudioButton.classList.contains("toggled");
        if (toggleOn) {
            Sound.Init();
        }
        MainViewer.MessageSender.SendToggleAudio(toggleOn);
    });
    ChangeScreenButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars("screenSelectBar");
        ScreenSelectBar.classList.toggle("open");
    });
    ClipboardTransferButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars("clipboardTransferBar");
        ClipboardTransferBar.classList.toggle("open");
    });
    TypeClipboardButton.addEventListener("click", (ev) => {
        if (!navigator.clipboard.readText) {
            alert("Clipboard access isn't supported on this browser.");
            return;
        }

        navigator.clipboard.readText().then(text => {
            MainViewer.MessageSender.SendClipboardTransfer(text, true);
            ShowMessage("Clipboard sent!");
        }, reason => {
            alert("Unable to read clipboard.  Please check your permissions.");
            console.log("Unable to read clipboard.  Reason: " + reason);
        });
    });
    ConnectButton.addEventListener("click", (ev) => {
        MainViewer.ConnectToClient();
    });
    CtrlAltDelButton.addEventListener("click", (ev) => {
        if (!MainViewer.ServiceID) {
            ShowMessage("Not available for this session.");
            return;
        }
        closeAllHorizontalBars(null);
        MainViewer.MessageSender.SendCtrlAltDel();
    });
    DisconnectButton.addEventListener("click", (ev) => {
        ConnectButton.removeAttribute("disabled");
        MainViewer.ViewerHubConnection.Connection.stop();
        if (location.search.includes("fromApi=true")) {
            window.close();
        }
    });
    document.querySelectorAll("#sessionIDInput, #nameInput").forEach(x => {
        x.addEventListener("keypress", (ev: KeyboardEvent) => {
            if (ev.key.toLowerCase() == "enter") {
                MainViewer.ConnectToClient();
            }
        })
    });
    FileTransferButton.addEventListener("click", (ev) => {
        FileTransferInput.click();
    });
    FileTransferInput.addEventListener("change", (ev) => {
        UploadFiles(FileTransferInput.files);
    });
    FitToScreenButton.addEventListener("click", (ev) => {
        FitToScreenButton.classList.toggle("toggled");
        if (FitToScreenButton.classList.contains("toggled")) {
            ScreenViewer.style.removeProperty("max-width");
            ScreenViewer.style.removeProperty("max-height");
            VideoScreenViewer.style.removeProperty("max-width");
            VideoScreenViewer.style.removeProperty("max-height");
        }
        else {
            ScreenViewer.style.maxWidth = "unset";
            ScreenViewer.style.maxHeight = "unset";
            VideoScreenViewer.style.maxWidth = "unset";
            VideoScreenViewer.style.maxHeight = "unset";
        }
    });
    BlockInputButton.addEventListener("click", (ev) => {
        BlockInputButton.classList.toggle("toggled");
        if (BlockInputButton.classList.contains("toggled")) {
            MainViewer.MessageSender.SendToggleBlockInput(true);
        }
        else {
            MainViewer.MessageSender.SendToggleBlockInput(false);
        }
    });
    InviteButton.addEventListener("click", (ev) => {
        var url = "";
        if (MainViewer.Mode == RemoteControlMode.Normal) {
            url = `${location.origin}${location.pathname}?sessionID=${MainViewer.ClientID}`;
        }
        else {
            url = `${location.origin}${location.pathname}?clientID=${MainViewer.ClientID}&serviceID=${MainViewer.ServiceID}`;
        }
        MainViewer.ClipboardWatcher.SetClipboardText(url);
        ShowMessage("Link copied to clipboard.");
    });
    KeyboardButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars(null);
        TouchKeyboardTextArea.focus();
        TouchKeyboardTextArea.setSelectionRange(TouchKeyboardTextArea.value.length, TouchKeyboardTextArea.value.length);
        MenuFrame.classList.remove("open");
        MenuButton.classList.remove("open");
    });
    MenuButton.addEventListener("click", (ev) => {
        if (isMenuButtonDragging) {
            isMenuButtonDragging = false;
            return;
        }
        MenuFrame.classList.toggle("open");
        MenuButton.classList.toggle("open");
        closeAllHorizontalBars(null);
    });
    MenuButton.addEventListener("mousedown", (ev) => {
        isMenuButtonDragging = false;
        startMenuDraggingY = ev.clientY;
        window.addEventListener("mousemove", moveMenuButton);
        window.addEventListener("mouseup", removeMouseButtonWindowListeners);
        window.addEventListener("mouseleave", removeMouseButtonWindowListeners);
    });
    MenuButton.addEventListener("touchmove", (ev) => {
        ev.preventDefault();
        ev.stopPropagation();
        MenuButton.style.top = `${ev.touches[0].clientY}px`;
    });
    QualityButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars("qualityBar");
        QualityBar.classList.toggle("open");
    })
    QualitySlider.addEventListener("change", (ev) => {
        MainViewer.MessageSender.SendQualityChange(Number(QualitySlider.value));
    });
    StreamVideoButton.addEventListener("click", (ev) => {
        StreamVideoButton.classList.toggle("toggled");
        if (StreamVideoButton.classList.contains("toggled")) {
            MainViewer.MessageSender.SendToggleWebRtcVideo(true);
            VideoScreenViewer.removeAttribute("hidden");
            ScreenViewer.setAttribute("hidden", "hidden");
            QualityButton.setAttribute("hidden", "hidden");
        }
        else {
            MainViewer.MessageSender.SendToggleWebRtcVideo(false);
            ScreenViewer.removeAttribute("hidden");
            QualityButton.removeAttribute("hidden");
            VideoScreenViewer.setAttribute("hidden", "hidden");
        }
    });
    AutoQualityAdjustCheckBox.addEventListener("change", ev => {
        MainViewer.MessageSender.SendAutoQualityAdjust(AutoQualityAdjustCheckBox.checked);
    });

    [ ScreenViewer, VideoScreenViewer ].forEach(viewer => {
        viewer.addEventListener("pointermove", function (e: PointerEvent) {
            currentPointerDevice = e.pointerType;
        });

        viewer.addEventListener("pointerdown", function (e: PointerEvent) {
            currentPointerDevice = e.pointerType;
        });

        viewer.addEventListener("pointerenter", function (e: PointerEvent) {
            currentPointerDevice = e.pointerType;
        });

        viewer.addEventListener("mousemove", function (e: MouseEvent) {
            e.preventDefault();
            if (Date.now() - lastPointerMove < 25) {
                return;
            }
            lastPointerMove = Date.now();
            var percentX = e.offsetX / viewer.clientWidth;
            var percentY = e.offsetY / viewer.clientHeight;
            MainViewer.MessageSender.SendMouseMove(percentX, percentY);
        });


        viewer.addEventListener("mousedown", function (e: MouseEvent) {
            if (currentPointerDevice == "touch") {
                return;
            }
            if (e.button != 0 && e.button != 2) {
                return;
            }
            e.preventDefault();
            var percentX = e.offsetX / viewer.clientWidth;
            var percentY = e.offsetY / viewer.clientHeight;
            MainViewer.MessageSender.SendMouseDown(e.button, percentX, percentY);
        });

        viewer.addEventListener("mouseup", function (e: MouseEvent) {
            if (currentPointerDevice == "touch") {
                return;
            }
            if (e.button != 0 && e.button != 2) {
                return;
            }
            e.preventDefault();
            var percentX = e.offsetX / viewer.clientWidth;
            var percentY = e.offsetY / viewer.clientHeight;
            MainViewer.MessageSender.SendMouseUp(e.button, percentX, percentY);
        });

        viewer.addEventListener("click", function (e: MouseEvent) {
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
                MainViewer.MessageSender.SendTap(percentX, percentY);
            }
        });

        viewer.addEventListener("touchstart", function (e: TouchEvent) {
            currentTouchCount = e.touches.length;

            if (currentTouchCount == 1) {
                startLongPressTimeout = window.setTimeout(() => {
                    var percentX = e.touches[0].pageX / viewer.clientWidth;
                    var percentY = e.touches[0].pageY / viewer.clientHeight;
                    MainViewer.MessageSender.SendMouseDown(2, percentX, percentY);
                    MainViewer.MessageSender.SendMouseUp(2, percentX, percentY);
                }, 1000);
            }

            if (currentTouchCount > 1) {
                cancelNextViewerClick = true;
            }
            if (currentTouchCount == 2) {
                startPinchPoint1 = { X: e.touches[0].pageX, Y: e.touches[0].pageY, IsEmpty: false };
                startPinchPoint2 = { X: e.touches[1].pageX, Y: e.touches[1].pageY, IsEmpty: false };
                lastPinchDistance = GetDistanceBetween(startPinchPoint1.X,
                    startPinchPoint1.Y,
                    startPinchPoint2.X,
                    startPinchPoint2.Y);
                lastPinchCenterX = (startPinchPoint1.X + startPinchPoint2.X) / 2;
                lastPinchCenterY = (startPinchPoint1.Y + startPinchPoint2.Y) / 2;
            }
            isDragging = false;
            KeyboardButton.removeAttribute("hidden");
            var focusedInput = document.querySelector("input:focus") as HTMLInputElement;
            if (focusedInput) {
                focusedInput.blur();
            }
        });



        viewer.addEventListener("touchmove", function (e: TouchEvent) {
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
                var pinchDistance = GetDistanceBetween(pinchPoint1.X,
                    pinchPoint1.Y,
                    pinchPoint2.X,
                    pinchPoint2.Y);


                var pinchCenterX = (pinchPoint1.X + pinchPoint2.X) / 2;
                var pinchCenterY = (pinchPoint1.Y + pinchPoint2.Y) / 2;

                ScreenViewerWrapper.scrollBy(lastPinchCenterX - pinchCenterX,
                    lastPinchCenterY - pinchCenterY);

                lastPinchCenterX = pinchCenterX;
                lastPinchCenterY = pinchCenterY;

                if (Math.abs(pinchDistance - lastPinchDistance) > 5) {
                    isPinchZooming = true;
                    if (FitToScreenButton.classList.contains("toggled")) {
                        FitToScreenButton.click();
                    }

                    var currentWidth = viewer.clientWidth;
                    var currentHeight = viewer.clientHeight;

                    var clientAdjustedScrollLeftPercent = (ScreenViewerWrapper.scrollLeft + (ScreenViewerWrapper.clientWidth * .5)) / ScreenViewerWrapper.scrollWidth;
                    var clientAdjustedScrollTopPercent = (ScreenViewerWrapper.scrollTop + (ScreenViewerWrapper.clientHeight * .5)) / ScreenViewerWrapper.scrollHeight;

                    var currentWidthPercent = Number(viewer.style.width.slice(0, -1));
                    var newWidthPercent = Math.max(100, (currentWidthPercent + (pinchDistance - lastPinchDistance) * (currentWidthPercent / 100)));
                    newWidthPercent = Math.min(5000, newWidthPercent);
                    viewer.style.width = String(newWidthPercent) + "%";

                    var heightChange = viewer.clientHeight - currentHeight;
                    var widthChange = viewer.clientWidth - currentWidth;

                    var pinchAdjustX = pinchCenterX / window.innerWidth - .5;
                    var pinchAdjustY = pinchCenterY / window.innerHeight - .5;

                    var scrollByX = widthChange * (clientAdjustedScrollLeftPercent + (pinchAdjustX * ScreenViewerWrapper.clientWidth / ScreenViewerWrapper.scrollWidth));
                    var scrollByY = heightChange * (clientAdjustedScrollTopPercent + (pinchAdjustY * ScreenViewerWrapper.clientHeight / ScreenViewerWrapper.scrollHeight));

                    ScreenViewerWrapper.scrollBy(scrollByX, scrollByY);

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
                MainViewer.MessageSender.SendMouseMove(pagePercentX, pagePercentY);
            }
        });

        viewer.addEventListener("touchend", function (e: TouchEvent) {
            currentTouchCount = e.touches.length;

            clearTimeout(startLongPressTimeout);

            if (e.touches.length == 1 && !isPinchZooming) {
                isDragging = true;
                var percentX = (e.touches[0].pageX - viewer.getBoundingClientRect().left) / viewer.clientWidth;
                var percentY = (e.touches[0].pageY - viewer.getBoundingClientRect().top) / viewer.clientHeight;
                MainViewer.MessageSender.SendMouseMove(percentX, percentY);
                MainViewer.MessageSender.SendMouseDown(0, percentX, percentY);
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
                MainViewer.MessageSender.SendMouseUp(0, percentX, percentY);
            }

            isDragging = false;
        });


        viewer.addEventListener("contextmenu", (ev) => {
            ev.preventDefault();
        });

        viewer.addEventListener("wheel", function (e: WheelEvent) {
            e.preventDefault();
            MainViewer.MessageSender.SendMouseWheel(e.deltaX, e.deltaY);
        });

    });


    TouchKeyboardTextArea.addEventListener("input", (ev) => {
        if (TouchKeyboardTextArea.value.length == 1) {
            MainViewer.MessageSender.SendKeyPress("Backspace");
        }
        else if (TouchKeyboardTextArea.value.endsWith("\n")) {
            MainViewer.MessageSender.SendKeyPress("Enter");
        }
        else if (TouchKeyboardTextArea.value.endsWith(" ")) {
            MainViewer.MessageSender.SendKeyPress(" ");
        }
        else {
            var input = TouchKeyboardTextArea.value.trim().substr(1);
            for (var i = 0; i < input.length; i++) {
                var character = input.charAt(i);
                var sendShift = character.match(/[A-Z~!@#$%^&*()_+{}|<>?]/);
                if (sendShift) {
                    MainViewer.MessageSender.SendKeyDown("Shift");
                }

                MainViewer.MessageSender.SendKeyPress(character);

                if (sendShift) {
                    MainViewer.MessageSender.SendKeyUp("Shift");
                }
            }
        }

        window.setTimeout(() => {
            TouchKeyboardTextArea.value = " #";
            TouchKeyboardTextArea.setSelectionRange(TouchKeyboardTextArea.value.length, TouchKeyboardTextArea.value.length);
        });
    });
    WindowsSessionSelect.addEventListener("focus", () => {
        MainViewer.MessageSender.GetWindowsSessions();
    });
    WindowsSessionSelect.addEventListener("change", () => {
        ShowMessage("Switching sessions...");
        MainViewer.MessageSender.ChangeWindowsSession(Number(WindowsSessionSelect.selectedOptions[0].value));
    });
    RecordSessionButton.addEventListener("click", () => {
        RecordSessionButton.classList.toggle("toggled");
        if (RecordSessionButton.classList.contains("toggled")) {
            RecordSessionButton.innerHTML = `Stop <i class="fas fa-record-vinyl">`;
            MainViewer.SessionRecorder.Start();
        }
        else {
            RecordSessionButton.innerHTML = `Start <i class="fas fa-record-vinyl">`;
            MainViewer.SessionRecorder.Stop();
        }
    });
    DownloadRecordingButton.addEventListener("click", () => {
        MainViewer.SessionRecorder.DownloadVideo();
    });

    window.addEventListener("keydown", function (e) {
        if (document.querySelector("input:focus") || document.querySelector("textarea:focus")) {
            return;
        }
        e.preventDefault();
        MainViewer.MessageSender.SendKeyDown(e.key);
    });
    window.addEventListener("keyup", function (e) {
        if (document.querySelector("input:focus") || document.querySelector("textarea:focus")) {
            return;
        }
        e.preventDefault();
        MainViewer.MessageSender.SendKeyUp(e.key);
    });

    window.addEventListener("blur", () => {
        MainViewer.MessageSender.SendSetKeyStatesUp();
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
        UploadFiles(e.dataTransfer.files);
    };
}

function closeAllHorizontalBars(exceptBarId: string) {
    HorizontalBars.forEach(x => {
        if (x.id != exceptBarId) {
            x.classList.remove('open');
        }
    })
}

function moveMenuButton(ev: MouseEvent) {
    if (Math.abs(ev.clientY - startMenuDraggingY) > 5) {
        if (ev.clientY < 0 || ev.clientY > window.innerHeight) {
            return;
        }
        isMenuButtonDragging = true;
        MenuButton.style.top = `${ev.clientY}px`;
    }
}

function removeMouseButtonWindowListeners(ev: MouseEvent) {
    window.removeEventListener("mousemove", moveMenuButton);
    window.removeEventListener("mouseup", removeMouseButtonWindowListeners);
    window.removeEventListener("mouseleave", removeMouseButtonWindowListeners);
}