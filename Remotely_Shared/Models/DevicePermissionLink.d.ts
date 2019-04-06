	interface DevicePermissionLink {
		DeviceID: string;
		Device: .Device;
		PermissionGroupID: string;
		PermissionGroup: {
			ID: string;
			Name: string;
			Organization: {
				ID: string;
				OrganizationName: string;
				RemotelyUsers: any[];
				Devices: .Device[];
				CommandContexts: .CommandContext[];
				EventLogs: any[];
				PermissionGroups: any[];
				InviteLinks: any[];
				SharedFiles: any[];
			};
			UserPermissionLinks: any[];
			DevicePermissionLinks: .DevicePermissionLink[];
		};
	}
