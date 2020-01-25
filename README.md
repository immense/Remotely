# Remotely
[![Build Status](https://dev.azure.com/translucency/Remotely/_apis/build/status/Remotely-ReleaseBuild?branchName=master)](https://dev.azure.com/translucency/Remotely/_build/latest?definitionId=17&branchName=master)
[![Donate](https://www.paypalobjects.com/webstatic/en_US/i/buttons/pp-acceptance-small.png)](https://paypal.me/translucency)

A remote control and remote scripting solution, built with .NET Core and SignalR Core.

Website: https://remotely.lucency.co  
Public Server: https://tryremotely.lucency.co (not intended for production use)

*Notice: Remotely is currently in maintenance mode and will likely stay in a more-or-less prototype state.*

## Client Prerequisites:
* Endpoint devices require the .NET Core runtime to be installed.
	* For Windows, the Desktop Runtime is required.
    * Download Link: https://dotnet.microsoft.com/download/dotnet-core/current/runtime

## Build Instructions (Windows 10)  
The following steps will configure your Windows 10 machine for building the Remotely server and clients.
* Install Visual Studio 2019.
    * Link: https://visualstudio.microsoft.com/downloads/
* Install the latest .NET Core SDK.
    * Link: https://dotnet.microsoft.com/download
* Clone the git repository and open the solution in Visual Studio.
* Run Publish.ps1 in the [Utilities folder in source control](https://raw.githubusercontent.com/Jay-Rad/Remotely/master/Utilities/Install-RemotelyServer.ps1).
    * Example: powershell -f [path]\Publish.ps1 -outdir C:\inetpub\remotely -rid win10-x64 -hostname https://mysite.mydomain.com
    * The output folder will now contain the server, with the clients in the Downloads folder.
	* The above hostname will be hardcoded in the screen-sharing desktop apps, but can be changed via the options menu.

## Hosting a Server (Windows)
* Build the Remotely server and clients using the above steps.
* Create a site in IIS that will run Remotely.
* Run Install-RemotelyServer.ps1 (as an administrator), which is in the [Utilities folder in source control](https://raw.githubusercontent.com/Jay-Rad/Remotely/master/Utilities/Install-RemotelyServer.ps1) and on the Releases page.
    * Alternatively, you can build from source and copy the server files to the site folder.
* Download and install the .NET Core Runtime (not the SDK) with the Hosting Bundle.
	* Link: https://dotnet.microsoft.com/download/dotnet-core/current/runtime
	* This includes the Hosting Bundle for Windows, which allows you to run ASP.NET Core in IIS.
	* Important: If you installed .NET Core Runtime before installing all the required IIS features, you may need to run a repair on the .NET Core Runtime installation.
* Change values in appsettings.json for your environment.
* If using SQLite configuration, make sure the ApplicationPool's account has write access to the DB file location.
* If the site will be public-facing, configure your bindings in IIS.
* An SSL certificate for HTTPS is recommended.  You can install one for free using Let's Encrypt.
	* Resources: https://letsencrypt.org/, https://certifytheweb.com/
* Documentation for hosting in IIS can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/index?view=aspnetcore-3.1

## Hosting a Server (Ubuntu)
* Ubuntu 18.04 and 19.04 have been tested.
* Run Remotely_Server_Setup.sh (with sudo), which is in the [Utilities folder in source control](https://raw.githubusercontent.com/Jay-Rad/Remotely/master/Utilities/Remotely_Server_Install.sh).
	* The script is designed to install Remotely and Nginx on the same server, running Ubuntu 18.04 or 19.04.  You'll need to manually set up other configurations.
    * A helpful user supplied an example Apache configuration, which can be found in the Utilities folder.
    * The script will prompt for the "App root" location, which is the above directory where the server files are located.
	* The script installs the .NET Core runtime, as well as other dependencies.
	* Certbot is used in this script and will install an SSL certificate for your site.  Your server needs to have a public domain name that is accessible from the internet for this to work.
		* More information: https://letsencrypt.org/, https://certbot.eff.org/
	* Alternatively, you can build from source (using RuntimeIdentifier "linux-x64" for the server) and copy the server files to the site folder.
* Change values in appsettings.json for your environment.
* Documentation for hosting behind Nginx can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-3.1

## Logging
* On clients, logs are kept in %temp%\Remotely_Logs.log.
	* For the Agent running as a Windows service, this maps to C:\Windows\Temp\Remotely_Logs.log.
* On the server, some event information is explicitly written to the EventLogs table in the database.
* Built-in ASP.NET Core logs are written to the console (stdout).  You can redirect this to a file if desired.
	* In IIS, this can be done in the web.config file by setting stdoutLogEnabled to true.
* On Windows Servers, the above logs can also be written to the Windows Event Log.
	* This is enabled in appsettings.json by setting EnableWindowsEventLog to true.
* You can configure logging levels and other settings in appsetttings.json.
	* More information: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1

## Remote Control Requirements
* Windows: Only the latest version of Windows 10 is tested.
	* Requires .NET Core Desktop Runtime.
	* Windows 2016/2019 should work as well, but isn't tested regularly.
* Linux: Only Ubuntu 18.04+ is tested.

## Session Recording
* You can turn on session recording in appsettings.json.
* The following requirements must be met for it to work:
	* On Linux, libgdiplus and libc6-dev must be installed (sudo apt-get install libgdiplus libc6-dev).
	* The process running the app must have access to create and modify a folder name "Recordings" in the site's root content folder.
	* FFmpeg must be executable from the process running the Remotely server.
		* Link: https://www.ffmpeg.org/download.html

## Remote Control on Mobile
Ideally, you'd be doing remote control from an actual computer or laptop.  However, I've tried to make the remote control at least somewhat usable from a mobile device.  Here are the controls:
* Left-click: Single tap
* Right-click: Tap and hold
* Click-and-drag: Tap and hold with one finger, tap and release a second finger (without pinch-zooming)
	* The click-and-drag operation will begin where finger one is held.

## Shortcut Keys
There are a few shortcut keys available when using the console.
* / : Slash will open the autocomplete for selecting the current command mode.  The names are configurable in the Account - Options page.
* Up/Down: Use arrow up/down to cycle through input history.
* Ctrl + Up/Down: Scroll the console output window.
* Ctrl + Q: Clear the output window.
* Esc: Close the autocomplete window.

## Configuration
The following settings are available in appsettings.json.

Note: To retain your settings between upgrades, copy your settings to appsettings.Production.json, which will supersede the original.

* DefaultPrompt: The default prompt string you'll see for each line on the console.
* DBProvider: Determines which of the three connection strings (at the top) will be used.  The appropriate DB provider for the database type is automatically loaded in code.
* MaxOrganizationCount: By default, one organization can exist on the server, which is created automatically when the first account is registered.  Afterward, self-registration will be disabled.
    * Set this to -1 or increase it to a specific number to allow multi-tenancy.
* RecordRemoteControlSessions: Whether or not to record remote control sessions.
* RedirectToHttps: Whether ASP.NET Core will redirect all traffic from HTTP to HTTPS.  This is independent of Nginx and IIS configurations that do the same.
* UseHsts: Whether ASP.NET Core will use HTTP Strict Transport Security.
* DataRetentionInDays: How long event logs and remote command logs will be kept.
* RemoteControlSessionLimit: How many concurrent remote control sessions are allowed per organization.
* RemoteControlRequiresAuthentication: Whether the remote control page requires authentication to establish a connection.
* AllowApiLogin: Whether to allow logging in via the API (see below).
* TrustedCorsOrigins: For cross-origin API requests via JavaScript.  The websites listed in this array with be allowed to make requests to the API.  This does not grant authentication, which is still required on most endpoints.
* KnownProxies: If your Nginx server is on a different machine and is forwarding requests to the Remotely server, you will need to add the IP of the Nginx server to this array.
* Smpt*: SMTP settings for auto-generated system emails (such as registration and password reset).
* Theme: The color theme to use for the site.  Values are "Light" or "Dark".
* UseWebRtc: Attempt to create a peer-to-peer connection via WebRTC for screen sharing.
    * Only works on Windows agents.
    * Session recording will not work if a WebRTC connection is made.

## API and Integrations
Remotely has a basic API, which can be browsed at https://tryremotely.lucency.co/swagger (or your own server instance).  Most endpoints require authentication via the /api/Login, which in turn requires the AllowApiLogin option to be set to true in appsettings.json.  If you're not familiar with how CORS works, I recommend reading up on it before proceeding.

Once AllowApiLogin is set to true, you'll be able to use the API with languages such as C# and PowerShell.  For JavaScript clients to work, you'll also need to add the website's origin to the TrustedCorsOrigins array.  For example, if I wanted to create a login form on https://lucency.co that logged into the Remotely API, I'd need to add "https://lucency.co" to the TrustedCorsOrigins.

Below are examples of how to use the API for starting a remote control session.

**JavaScript Client**  

	// Log in with one request, then launch remote control with another.
	fetch("https://localhost:44351/api/Login/", {
		method: "post",
		credentials: "include",
		mode: "cors",
		body: '{"Email":"email@example.com", "Password":"P@ssword1"}',
		headers: {
			"Content-Type": "application/json",
		}
	}).then(response=>{
		if (response.ok) {
			fetch("https://localhost:44351/api/RemoteControl/b68c24b0-2c67-4524-ad28-dadea7a576a4", {
				method: "get",
				credentials: "include",
				mode: "cors"
			}).then(response=>{
				if (response.ok) {
					response.text().then(url=>{
						window.open(url);
					})
				}
			})
		}
	})

	// Log in and launch remote control in the same request.
	fetch("https://localhost:44351/api/RemoteControl/", {
		method: "post",
		credentials: "include",
		mode: "cors",
		body: '{"Email":"email@example.com", "Password":"P@ssword1", "DeviceID":"b68c24b0-2c67-4524-ad28-dadea7a576a4"}',
		headers: {
			"Content-Type": "application/json",
		}
	}).then(response=>{
		if (response.ok) {
			response.text().then(url=>{
				window.open(url);
			})
		}
	})

**C# Client**

	private static async Task StartRemoteControl()
	{
		var client = new System.Net.Http.HttpClient();
		client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

		var stringContent = new System.Net.Http.StringContent(
			@"{
				'Email': 'email@example.com',
				'Password': 'P@ssword1'
			}",
			System.Text.Encoding.UTF8,
			"application/json");


		var response = await client.PostAsync("https://localhost:44351/api/Login", stringContent);
		if (response.IsSuccessStatusCode)
		{
			response = await client.GetAsync("https://localhost:44351/api/RemoteControl/b68c24b0-2c67-4524-ad28-dadea7a576a4");
			if (response.IsSuccessStatusCode)
			{
				var remoteControlURL = await response.Content.ReadAsStringAsync();
				System.Diagnostics.Process.Start(remoteControlURL);
			}
		}
	}

**PowerShell Client**

	$WebSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
	 
	$Response = Invoke-WebRequest -Uri "https://localhost:44351/api/Login" -WebSession $WebSession -Method Post -Body '
	{
		"Email": "email@example.com",
		"Password": "P@ssword1"
	}
	' -ContentType "application/json"


	if ($Response.StatusCode -eq 200) {
		$Response = Invoke-WebRequest -Uri "https://localhost:44351/api/RemoteControl/b68c24b0-2c67-4524-ad28-dadea7a576a4" -WebSession $WebSession -Method Get -ContentType "application/json"
		if ($Response.StatusCode -eq 200) {
			Start-Process -FilePath $Response.Content
		}
	}

## .NET Core Deployments
* .NET Core has two methods of deployment: framework-dependent and self-contained.
	* Framework-dependent deployments require the .NET Core runtime to be installed on the target computers.  It must be the same version or higher that was used to build the app.
	* Self-contained deployments include a copy of the runtime, so you don't need to install it on the target computers.  As a result, the total file size is much larger.
* .NET Core uses runtime identifiers that are targeted when building.
	* Link: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
