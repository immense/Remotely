declare interface CaptureFrameDto {
    EndOfFrame: boolean;
    Left: number;
    Top: number;
    Width: number;
    Height: number;
    ImageBytes: Uint8Array;
    ImageQuality: number;
}