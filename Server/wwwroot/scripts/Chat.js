import * as HubConnection from "./HubConnection.js";
import { DataSource } from "./DataGrid.js";
import { ShowMessage } from "./UI.js";
export function CreateChatWindow(deviceID, deviceName) {
    var chatWindow = document.getElementById("chat-" + deviceID);
    if (!chatWindow) {
        var windowHtml = `
            <div class="chat-header">
                <h6 class="mt-3">Chat with ${deviceName}</h6>
                <div class="text-right">
                    <i class="fas fa-window-close close-button pointer"></i>
                </div>
            </div>
            <div class="chat-messages">
            </div>
            <textarea class="chat-input" value="" style="border: 1px solid gray"></textarea>
        `;
        chatWindow = document.createElement("div");
        chatWindow.classList.add("chat-window");
        chatWindow.style.right = "20px";
        chatWindow.style.bottom = "20px";
        chatWindow.setAttribute("id", "chat-" + deviceID);
        chatWindow.innerHTML = windowHtml;
        document.body.appendChild(chatWindow);
        chatWindow.addEventListener("mousedown", (ev) => {
            document.querySelectorAll(".chat-window").forEach(x => {
                x.style.zIndex = "0";
            });
            ev.currentTarget.style.zIndex = "1";
        });
        chatWindow.querySelector(".close-button").onclick = (ev) => {
            ev.preventDefault();
            ev.stopPropagation();
            document.body.removeChild(chatWindow);
        };
        chatWindow.querySelector(".chat-header").onmousedown = (ev) => {
            ev.preventDefault();
            chatWindow.removeEventListener("mousemove", moveChatWindow);
            chatWindow.removeEventListener("mouseup", stopMovingChatWindow);
            chatWindow.removeEventListener("mouseleave", stopMovingChatWindow);
            chatWindow.addEventListener("mousemove", moveChatWindow);
            chatWindow.addEventListener("mouseup", stopMovingChatWindow);
            chatWindow.addEventListener("mouseleave", stopMovingChatWindow);
        };
        chatWindow.querySelector(".chat-input").onkeypress = (ev) => {
            if (ev.key.toLowerCase() == "enter") {
                ev.preventDefault();
                ev.stopPropagation();
                if (!DataSource.find(x => x.ID == deviceID).IsOnline) {
                    ShowMessage("Device is offline.");
                    return;
                }
                var inputText = ev.currentTarget.value;
                if (!inputText) {
                    return;
                }
                chatWindow.querySelector(".chat-messages").innerHTML += `
                    <div>
                        <span class="text-secondary font-weight-bold">You: </span>
                        <span>${inputText}</span>
                    </div>
                `;
                ev.currentTarget.value = "";
                HubConnection.Connection.invoke("Chat", inputText, [deviceID]);
                var chatMessages = chatWindow.querySelector(".chat-messages");
                chatMessages.scrollTo({ top: chatMessages.scrollHeight });
            }
        };
    }
}
export function ReceiveChatText(deviceID, deviceName, message, disconnected) {
    CreateChatWindow(deviceID, deviceName);
    var chatWindow = document.getElementById("chat-" + deviceID);
    var chatMessages = chatWindow.querySelector(".chat-messages");
    if (disconnected) {
        chatMessages.innerHTML += `
            <div>
                <span class="font-italic">${deviceName} disconnected from chat.</span>
                <span>${message}</span>
            </div>
        `;
    }
    else {
        chatMessages.innerHTML += `
            <div>
                <span class="text-primary font-weight-bold">${deviceName}: </span>
                <span>${message}</span>
            </div>
        `;
    }
    chatMessages.scrollTo({ top: chatMessages.scrollHeight });
}
function moveChatWindow(ev) {
    var chatWindow = ev.currentTarget;
    chatWindow.style.right = String(parseInt(chatWindow.style.right || "0") - ev.movementX) + "px";
    chatWindow.style.bottom = String(parseInt(chatWindow.style.bottom || "0") - ev.movementY) + "px";
}
function stopMovingChatWindow(ev) {
    ev.currentTarget.removeEventListener("mousemove", moveChatWindow);
    ev.currentTarget.removeEventListener("mouseup", stopMovingChatWindow);
    ev.currentTarget.removeEventListener("mouseleave", stopMovingChatWindow);
}
//# sourceMappingURL=Chat.js.map