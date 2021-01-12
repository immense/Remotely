	interface commandResult {
		iD: string;
		commandMode: string;
		commandText: string;
		senderUserID: string;
		senderConnectionID: string;
		targetDeviceIDs: string[];
		pSCoreResults: any[];
		commandResults: .genericCommandResult[];
		timeStamp: Date;
		organizationID: string;
	}
