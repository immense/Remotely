declare module server {
	interface genericCommandResult {
		deviceID: string;
		commandContextID: string;
		commandType: string;
		standardOutput: string;
		errorOutput: string;
		timeStamp: Date;
	}
}
