export interface Device {
    Alias: string;
    AgentVersion: string;
    CpuUtilization: number;
    CurrentUser: string;
    DeviceName: string;
    DeviceGroupID: string;
    Drives: any[];
    ID: string;
    Is64Bit: boolean;
    IsOnline: boolean;
    LastOnline: Date;
    OrganizationID: string;
    OSArchitecture: any;
    OSDescription: string;
    Platform: string;
    ProcessorCount: number;
    TotalMemory: number;
    TotalStorage: number;
    Tags: string;
    UsedMemory: number;
    UsedStorage: number;
}