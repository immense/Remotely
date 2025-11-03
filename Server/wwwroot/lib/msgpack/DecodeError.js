"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DecodeError = void 0;
class DecodeError extends Error {
    constructor(message) {
        super(message);
        // fix the prototype chain in a cross-platform way
        const proto = Object.create(DecodeError.prototype);
        Object.setPrototypeOf(this, proto);
        Object.defineProperty(this, "name", {
            configurable: true,
            enumerable: false,
            value: DecodeError.name,
        });
    }
}
exports.DecodeError = DecodeError;
//# sourceMappingURL=DecodeError.js.map