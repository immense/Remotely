export interface Machine {
    Drives: any[];
    ID: string;
    Is64Bit: boolean;
    IsOnline: boolean;
    LastOnline: Date;
    MachineName: string;
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