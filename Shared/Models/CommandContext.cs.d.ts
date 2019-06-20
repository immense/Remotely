declare module server {
	interface commandContext {
		iD: string;
		commandMode: string;
		commandText: string;
		senderUserID: string;
		senderConnectionID: string;
		targetDeviceIDs: string[];
		pSCoreResults: any[];
		commandResults: any[];
		timeStamp: Date;
		organizationID: string;
	}
}
