export interface PSCoreCommandResult {
    DeviceID: string;
    CommandContextID: string;
    VerboseOutput: string[];
    DebugOutput: string[];
    ErrorOutput: string[];
    HostOutput: string;
    InformationOutput: string[];
    WarningOutput: string[];
    TimeStamp: Date;
}