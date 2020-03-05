export interface PSCoreCommandResult {
    DeviceID: string;
    CommandResultID: string;
    VerboseOutput: string[];
    DebugOutput: string[];
    ErrorOutput: string[];
    HostOutput: string;
    InformationOutput: string[];
    WarningOutput: string[];
    TimeStamp: Date;
}