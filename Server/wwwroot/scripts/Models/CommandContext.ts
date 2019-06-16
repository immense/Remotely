export interface CommandContext {
    ID: string;
    CommandMode: string;
    CommandText: string;
    SenderUserID: string;
    SenderConnectionID: string;
    TargetDeviceIDs: string[];
    PSCoreResults: any[];
    CMDResults: any[];
    WinPSResults: any[];
}