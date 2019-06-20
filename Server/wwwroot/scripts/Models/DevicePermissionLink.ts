export interface DevicePermissionLink {
    DeviceID: string;
    Device: {
	    CurrentUser: string;
	    Drives: any[];
	    FreeMemory: number;
	    FreeStorage: number;
	    ID: string;
	    Is64Bit: boolean;
	    IsOnline: boolean;
	    LastOnline: Date;
	    DeviceName: string;
	    Organization: {
		    ID: string;
		    OrganizationName: string;
		    RemotelyUsers: any[];
		    Devices: any[];
		    CommandContexts: any[];
		    EventLogs: any[];
		    PermissionGroups: any[];
		    InviteLinks: any[];
		    SharedFiles: any[];
	    };
	    OrganizationID: string;
	    OSArchitecture: any;
	    OSDescription: string;
	    DevicePermissionLinks: any[];
	    Platform: string;
	    ProcessorCount: number;
	    ServerVerificationToken: string;
	    Tags: string;
	    TotalMemory: number;
	    TotalStorage: number;
    };
    PermissionGroupID: string;
    PermissionGroup: {
	    ID: string;
	    Name: string;
	    Organization: {
		    ID: string;
		    OrganizationName: string;
		    RemotelyUsers: any[];
		    Devices: any[];
		    CommandContexts: any[];
		    EventLogs: any[];
		    PermissionGroups: any[];
		    InviteLinks: any[];
		    SharedFiles: any[];
	    };
	    UserPermissionLinks: any[];
	    DevicePermissionLinks: any[];
    };
}
