/**
 * Splits a string into "parts" number of pieces.  Once "parts" number is
 * reached, additional occurrences of the splitter are ignored.
 * @param splitter
 * @param parts
 */
export function Split(originalString, splitter, parts) {
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
export function CreateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
export function EncodeForHTML(text) {
    var tempDiv = document.createElement("div");
    tempDiv.innerText = text;
    return tempDiv.innerHTML;
}
export function ParseSearchString() {
    var queryStrings = {};
    var queryArray = location.search.substring(1).split("&");
    queryArray.forEach(value => {
        var keyValue = value.split("=");
        queryStrings[keyValue[0]] = keyValue[1];
    });
    return queryStrings;
}
export function GetDistanceBetween(fromX, fromY, toX, toY) {
    return Math.sqrt(Math.pow(fromX - toX, 2) +
        Math.pow(fromY - toY, 2));
}
export async function When(predicate) {
    return new Promise((resolve, reject) => {
        function checkCondition() {
            if (predicate()) {
                resolve();
            }
            else {
                window.setTimeout(() => {
                    checkCondition();
                }, 500);
            }
        }
        checkCondition();
    });
}
export function ConvertBase64ToUInt8Array(base64) {
    var binaryString = window.atob(base64);
    var bytes = new Uint8ClampedArray(binaryString.length);
    for (var i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
}
export function ConvertUInt8ArrayToBase64(array) {
    var base64String = '';
    for (var i = 0; i < array.byteLength; i++) {
        base64String += String.fromCharCode(array[i]);
    }
    return btoa(base64String);
}
export function RemoveFromArray(array, item) {
    var index = array.indexOf(item);
    if (index > -1) {
        array.splice(index, 1);
    }
}
;
export function SetClipboardText(text) {
    var input = document.createElement("input");
    input.style.position = "fixed";
    input.style.top = "-1000px";
    input.type = "text";
    document.body.appendChild(input);
    input.value = text;
    input.select();
    document.execCommand("copy", false);
    input.remove();
}
//# sourceMappingURL=Utilities.js.map