export interface GenericCommandResult {
    MachineID: string;
    CommandContextID: string;
    CommandType: string;
    StandardOutput: string;
    ErrorOutput: string;
    TimeStamp: Date;
}