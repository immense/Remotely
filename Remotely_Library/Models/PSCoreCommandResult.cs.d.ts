	interface PSCoreCommandResult {
		CommandContextID: string;
		MachineID: string;
		VerboseOutput: string[];
		DebugOutput: string[];
		ErrorOutput: string[];
		HostOutput: string;
		InformationOutput: string[];
		WarningOutput: string[];
		TimeStamp: Date;
	}
