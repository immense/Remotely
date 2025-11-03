"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.prettyByte = prettyByte;
function prettyByte(byte) {
    return `${byte < 0 ? "-" : ""}0x${Math.abs(byte).toString(16).padStart(2, "0")}`;
}
//# sourceMappingURL=prettyByte.js.map