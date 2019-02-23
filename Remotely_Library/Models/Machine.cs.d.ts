	interface Machine {
		Drives: any[];
		ID: string;
		Is64Bit: boolean;
		IsOnline: boolean;
		LastOnline: Date;
		MachineName: string;
		OrganizationID: string;
		Organization: {
			ID: string;
			OrganizationName: string;
			RemotelyUsers: any[];
			Machines: server.Machine[];
			CommandContexts: any[];
			EventLogs: any[];
			PermissionGroups: any[];
			InviteLinks: any[];
			SharedFiles: any[];
		};
		OSArchitecture: any;
		OSDescription: string;
		Platform: string;
		ProcessorCount: number;
		TotalMemory: number;
		FreeStorage: number;
		TotalStorage: number;
		FreeMemory: number;
		CurrentUser: string;
		PermissionGroups: any[];
		Tags: string;
		ServerVerificationToken: string;
	}
