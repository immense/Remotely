export interface GenericCommandResult {
    DeviceID: string;
    CommandContextID: string;
    CommandType: string;
    StandardOutput: string;
    ErrorOutput: string;
    TimeStamp: Date;
}