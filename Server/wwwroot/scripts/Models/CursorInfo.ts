import { Point } from "./Point";

export interface CursorInfo {
	ImageBytes: Uint8Array;
    HotSpot: Point;
    CssOverride: string;
}