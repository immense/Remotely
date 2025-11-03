import { FileTransferProgress, FileTransferInput, FileTransferNameSpan } from "./UI.js";
import { ViewerApp } from "./App.js";
import { ShowToast } from "./UI.js";
const PartialDownloads = Object.create(null);
export async function UploadFiles(fileList) {
    var _a, _b, _c;
    if (!((_a = FileTransferProgress.parentElement) === null || _a === void 0 ? void 0 : _a.hasAttribute("hidden"))) {
        FileTransferInput.value = "";
        ShowToast("File transfer already in progress.");
        return;
    }
    ShowToast("File upload started");
    FileTransferProgress.value = 0;
    (_b = FileTransferProgress.parentElement) === null || _b === void 0 ? void 0 : _b.removeAttribute("hidden");
    try {
        for (let i = 0; i < fileList.length; i++) {
            const file = fileList[i];
            FileTransferNameSpan.innerHTML = file.name;
            const buffer = await file.arrayBuffer();
            await ViewerApp.MessageSender.SendFile(new Uint8Array(buffer), file.name);
        }
        ShowToast("File upload completed.");
    }
    catch (_d) {
        ShowToast("File upload failed.");
    }
    finally {
        FileTransferInput.value = "";
        (_c = FileTransferProgress.parentElement) === null || _c === void 0 ? void 0 : _c.setAttribute("hidden", "hidden");
    }
}
function toPlainArrayBuffer(u8) {
    const ab = new ArrayBuffer(u8.byteLength);
    new Uint8Array(ab).set(u8);
    return ab;
}
export async function ReceiveFile(file) {
    if (file.StartOfFile) {
        ShowToast(`Downloading file ${file.FileName}`);
    }
    let partial = PartialDownloads[file.MessageId];
    if (!partial) {
        partial = [];
        PartialDownloads[file.MessageId] = partial;
    }
    if (file.EndOfFile) {
        // Convert each Uint8Array to a plain ArrayBuffer for BlobPart
        const parts = partial.map(toPlainArrayBuffer);
        const blob = new Blob(parts, { type: "application/octet-stream" });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.style.display = "none";
        link.href = url;
        link.download = file.FileName;
        document.body.appendChild(link);
        link.click();
        setTimeout(() => {
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        }, 100);
        delete PartialDownloads[file.MessageId]; // optional cleanup
    }
}
//# sourceMappingURL=FileTransferService.js.map