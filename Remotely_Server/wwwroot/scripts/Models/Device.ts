export interface Device {
    Drives: any[];
    ID: string;
    Is64Bit: boolean;
    IsOnline: boolean;
    LastOnline: Date;
    DeviceName: string;
    OrganizationID: string;
    OSArchitecture: any;
    OSDescription: string;
    Platform: string;
    ProcessorCount: number;
    TotalMemory: number;
    FreeStorage: number;
    TotalStorage: number;
    FreeMemory: number;
    CurrentUser: string;
    Tags: string;
}