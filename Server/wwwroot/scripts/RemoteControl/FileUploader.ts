import { ShowMessage, FileTransferProgress, FileTransferInput, FileTransferNameSpan } from "./UI.js";
import { MainRc } from "./Main.js";

export async function UploadFiles(fileList: FileList) {
    if (!FileTransferProgress.parentElement.hasAttribute("hidden")) {
        FileTransferInput.value = null;
        ShowMessage("File transfer already in progress.");
        return;
    }
    ShowMessage("File upload started...");
    FileTransferProgress.value = 0;
    FileTransferProgress.parentElement.removeAttribute("hidden");

    try {
        for (var i = 0; i < fileList.length; i++) {
            FileTransferNameSpan.innerHTML = fileList[i].name;
            var buffer = await fileList[i].arrayBuffer();
            await MainRc.MessageSender.SendFile(new Uint8Array(buffer), fileList[i].name);
        }
        ShowMessage("File upload completed.");
    }
    catch {
        ShowMessage("File upload failed.");
    }
    FileTransferInput.value = null;
    FileTransferProgress.parentElement.setAttribute("hidden", "hidden");
}