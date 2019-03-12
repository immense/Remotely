export function GetCursor(cursorCode: string): string {
    if (!cursorMap[cursorCode]) {
        console.log("Cursor code " + cursorCode + " is unmapped.");
        return "default";
    }
    return cursorMap[cursorCode];
}

var cursorMap = {
    "65563": "wait",
    "65547": "crosshair",
    "65541": "text",
    "65543": "text",
    "65561": "wait",
    "65559": "all-scroll",
    "65553": "ew-resize",
    "65557": "ns-resize",
    "65551": "nesw-resize",
    "65555": "ns-resize",
    "65549": "nwse-resize",
    "65545": "default",
    "65565": "help",
    "3148293": "row-resize",
    "1575755": "col-resize",
    "199481": "move",
    "264975": "move",
    "199463": "move",
    "265011": "all-scroll",
    "265009": "all-scroll",
    "592663": "all-scroll",
    "199467": "all-scroll",
    "265031": "all-scroll",
    "133957": "all-scroll",
    "133955": "all-scroll",
    "68433": "all-scroll",
    "68435": "pointer",
    "65539": "default",
    "65567": "pointer",
    "239210287": "row-resize",
    "706285089": "col-resize",
    "51907077": "row-resize",
    "584124173": "row-resize",
    "567017879": "col-resize",
    "925305447": "row-resize",
    "-1670772177": "col-resize",
    "6883365": "copy"
}
