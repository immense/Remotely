import { DevicePermissionLink } from "./DevicePermissionLink";

export interface Device {
    Alias: string;
    AgentVersion: string;
    CurrentUser: string;
    DeviceName: string;
    DeviceGroupID: string;
    Drives: any[];
    FreeMemory: number;
    FreeStorage: number;
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
}