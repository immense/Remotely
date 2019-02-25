	interface PSCoreCommandResult {
		CommandContextID: string;
		DeviceID: string;
		VerboseOutput: string[];
		DebugOutput: string[];
		ErrorOutput: string[];
		HostOutput: string;
		InformationOutput: string[];
		WarningOutput: string[];
		TimeStamp: Date;
	}
