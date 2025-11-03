"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.encode = encode;
const Encoder_1 = require("./Encoder");
/**
 * It encodes `value` in the MessagePack format and
 * returns a byte buffer.
 *
 * The returned buffer is a slice of a larger `ArrayBuffer`, so you have to use its `#byteOffset` and `#byteLength` in order to convert it to another typed arrays including NodeJS `Buffer`.
 */
function encode(value, options) {
    const encoder = new Encoder_1.Encoder(options);
    return encoder.encodeSharedRef(value);
}
//# sourceMappingURL=encode.js.map