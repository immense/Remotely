import { DevicePermissionLink } from "./DevicePermissionLink";

export interface Device {
    AgentVersion: string;
    CurrentUser: string;
    DeviceName: string;
    DevicePermissionLinks: DevicePermissionLink[];
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