declare module server {
	interface genericCommandResult {
		deviceID: string;
		commandResultID: string;
		commandType: string;
		standardOutput: string;
		errorOutput: string;
		timeStamp: Date;
	}
}
