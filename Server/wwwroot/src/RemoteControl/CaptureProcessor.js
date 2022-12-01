import { ViewerApp } from "./App.js";
import { Screen2DContext } from "./UI.js";
const BitmapQueue = [];
const ImagePartials = {};
let CanvasLock = 1;
let NextSequence = 0;
export function HandleCaptureReceived(captureFrame) {
    if (!ImagePartials[captureFrame.Sequence]) {
        ImagePartials[captureFrame.Sequence] = [];
    }
    ImagePartials[captureFrame.Sequence].push(captureFrame.ImageBytes);
    if (captureFrame.EndOfFrame) {
        var partials = ImagePartials[captureFrame.Sequence];
        let completedFrame = new Blob(partials);
        delete ImagePartials[captureFrame.Sequence];
        createImageBitmap(completedFrame).then(bitmap => {
            BitmapQueue.push({
                ImageContent: bitmap,
                FrameData: captureFrame
            });
            if (CanvasLock < 1) {
                return;
            }
            CanvasLock--;
            processQueue();
        });
        //let url = window.URL.createObjectURL(completedFrame);
        //let img = new Image(captureFrame.Width, captureFrame.Height);
        //img.onload = () => {
        //    UI.Screen2DContext.drawImage(img,
        //        captureFrame.Left,
        //        captureFrame.Top,
        //        captureFrame.Width,
        //        captureFrame.Height);
        //    window.URL.revokeObjectURL(url);
        //};
    }
}
function processQueue() {
    try {
        if (BitmapQueue.length > 0) {
            let completedFrame = BitmapQueue.shift();
            let content = completedFrame.ImageContent;
            let data = completedFrame.FrameData;
            if (data.Sequence == 0) {
                NextSequence = 0;
                BitmapQueue.splice(0);
            }
            if (data.Sequence != NextSequence) {
                console.debug("Frame out of sequence.  Putting it back in queue.");
                BitmapQueue.push(completedFrame);
                BitmapQueue.sort((a, b) => a.FrameData.Sequence - b.FrameData.Sequence);
                window.setTimeout(() => processQueue(), 1);
                return;
            }
            createImageBitmap(content).then(bitmap => {
                Screen2DContext.drawImage(bitmap, data.Left, data.Top, data.Width, data.Height);
                bitmap.close();
                ViewerApp.MessageSender.SendFrameReceived();
                NextSequence = data.Sequence + 1;
                window.setTimeout(() => processQueue(), 1);
            });
        }
        else {
            CanvasLock++;
        }
    }
    catch (ex) {
        CanvasLock = 1;
        console.error(ex);
    }
}
//# sourceMappingURL=CaptureProcessor.js.map