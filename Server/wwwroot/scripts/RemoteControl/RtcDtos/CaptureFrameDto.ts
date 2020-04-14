import { DynamicDto } from "../DynamicDto.js";

export interface CaptureFrameDto extends DynamicDto {
    EndOfFrame: boolean;
    Left: number;
    Top: number;
    Width: number;
    Height: number;
    ImageBytes: Uint8Array;
    ImageQuality: number;
}