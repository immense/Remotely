export interface PSCoreCommandResult {
    MachineID: string;
    CommandContextID: string;
    VerboseOutput: string[];
    DebugOutput: string[];
    ErrorOutput: string[];
    HostOutput: string;
    InformationOutput: string[];
    WarningOutput: string[];
    TimeStamp: Date;
}