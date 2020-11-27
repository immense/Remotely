# Remotely
A remote control and remote scripting solution, built with .NET Core, SignalR Core, and WebRTC.

[![Build Status](https://dev.azure.com/translucency/Remotely/_apis/build/status/Remotely-ReleaseBuild?branchName=master)](https://dev.azure.com/translucency/Remotely/_build/latest?definitionId=17&branchName=master)
![GitHub Build](https://github.com/lucent-sea/Remotely/workflows/GitHub%20Build/badge.svg)

## Donations
If this project has benefited you in some way, or if you just want to show appreciation, please consider donating to a charity.

Suggested Charities: https://www.givewell.org/charities/top-charities

If you want to send a few dollars my way as well, you can with the below links.  But if you have to choose between one or the other, please pick the charity.  Your money will have a much greater impact on their lives than mine.

Ongoing via GitHub: https://github.com/sponsors/lucent-sea  
One-Time via PayPal: https://paypal.me/translucency  

## Project Links
Demo App: https://app.remotely.one  
Website: https://remotely.one  
Subreddit: https://www.reddit.com/r/remotely_app/  

## Disclaimer
Hosting a Remotely server requires building and running an ASP.NET Core web app behind IIS (Windows) or Nginx (Ubuntu).  It's expected that the person deploying and maintaining the server is familiar with this process.

It's *highly* encouraged that you get comfortable building and deploying from source.  This allows you to hard-code your server's hostname into the desktop client and the installer, which makes for a better experience for the end user.  If you don't want to use any of the methods below, you can look at the GitHub Actions workflows to see how the process can be automated, using the `Publish.ps1` script.  You can use those as reference for creating an automation process that works for you.  You can also use Azure Pipelines for free (which I personally use).

## Build Instructions (GitHub)
GitHub Actions allows you to build and deploy Remotely for free from their cloud servers.  The definitions for the build processes are located in `/.github/workflows/` folder.

After forking the repo, follow the instructions in the workflow YML file for configuring GitHub Secrets, then run the workflow.

## Build Instructions (Windows 10)  
The following steps will configure your Windows 10 machine for building the Remotely server and clients.
* Install Visual Studio 2019.
    * Link: https://visualstudio.microsoft.com/downloads/
	* You only need the below Individual Components for building:
	    * .NET Core SDK (latest version).
		* MSBuild (which auto-selects Roslyn compilers).
		* NuGet targets and build tasks.
		* .NET Framework 4.8 SDK.
	    * For debugging and development, you'll need all relevant workloads.
* Install Node.js.
	* Link: https://nodejs.org/
* Install Git for Windows.
    * Link: https://git-scm.com/downloads
* Clone the git repository: `git clone https://github.com/lucent-sea/remotely`
* Run Publish.ps1 in the [Utilities folder in source control](https://raw.githubusercontent.com/lucent-sea/Remotely/master/Utilities/Publish.ps1).
    * Example: `powershell -f [path]\Publish.ps1 -outdir C:\inetpub\remotely -rid win10-x64 -hostname https://mysite.mydomain.com`
    	* Use `-rid linux-x64` if you're going to host on Ubuntu.
    * The output folder will now contain the server, with the clients in the Downloads folder.
	* The above hostname will be hardcoded in the screen-sharing desktop apps and the installer.
* When debugging, the agent will use a pre-defined device ID and connect to https://localhost:5001.
* In development environment, the server will assign all connecting agents to the first organization.
* The above two allow you to debug the agent and server together, and see your device in the list.

## Hosting a Server (Windows)
* Build the Remotely server and clients using the above steps.
* Create a site in IIS that will run Remotely.
* Run Install-RemotelyServer.ps1 (as an administrator), which is in the [Utilities folder in source control](https://raw.githubusercontent.com/lucent-sea/Remotely/master/Utilities/Install-RemotelyServer.ps1) and on the Releases page.
    * Alternatively, you can build from source and copy the server files to the site folder.
* Download and install the .NET Core Runtime (not the SDK) with the Hosting Bundle.
	* Link: https://dotnet.microsoft.com/download/dotnet-core/current/runtime
	* This includes the Hosting Bundle for Windows, which allows you to run ASP.NET Core in IIS.
	* Important: If you installed .NET Core Runtime before installing all the required IIS features, you may need to run a repair on the .NET Core Runtime installation.
* Change values in appsettings.json for your environment.  Make a copy named `appsettings.Production.json` (see Configuration section below).
* By default, SQLite is used for the database.
    * The "Remotely.db" database file is automatically created in the root folder of your site.
	* You can browse and modify the contents using [DB Browser for SQLite](https://sqlitebrowser.org/).
* If the site will be public-facing, configure your bindings in IIS.
* An SSL certificate for HTTPS is recommended.  You can install one for free using Let's Encrypt.
	* Resources: https://letsencrypt.org/, https://certifytheweb.com/
* Documentation for hosting in IIS can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/index?view=aspnetcore-3.1

## Hosting a Server (Ubuntu)
* Ubuntu 18.04 and 19.04 have been tested.
* Run Ubuntu_Server_Install.sh (with sudo), which is in the [Utilities folder in source control](https://raw.githubusercontent.com/lucent-sea/Remotely/master/Utilities/Remotely_Server_Install.sh).
	* The script is designed to install Remotely and Nginx on the same server, running Ubuntu 18.04 or 19.04.  You'll need to manually set up other configurations.
    * A helpful user supplied an example Apache configuration, which can be found in the Utilities folder.
    * The script will prompt for the "App root" location, which is the above directory where the server files are located.
	* The script installs the .NET Core runtime, as well as other dependencies.
	* Certbot is used in this script and will install an SSL certificate for your site.  Your server needs to have a public domain name that is accessible from the internet for this to work.
		* More information: https://letsencrypt.org/, https://certbot.eff.org/
	* Alternatively, you can build from source (using RuntimeIdentifier "linux-x64" for the server) and copy the server files to the site folder.
* Change values in appsettings.json for your environment.  Make a copy named `appsettings.Production.json` (see Configuration section below).
* Documentation for hosting behind Nginx can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-3.1


## Hosting Scenarios
There are countless ways to host an ASP.NET Core app, and I can't document or automate all of them.  For hosting scenarios aside from the above two, please refer to Microsoft's documentation.
- https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1


## Upgrading
* To upgrade a server, do any of the below to copy the new Server application files.
	* Run one of the GitHub Actions workflows.
	* Build from source as described above and `rsync`/`robocopy` the output files to the server directory.
	* Build from source and deploy to IIS (e.g. `dotnet publish /p:PublishProfile=MyProfile`)
	* Re-run the installer script supplied in the releases.
* For Linux, you'll also need to restart the Remotely service in systemd after overwriting the files.
* The only things that can't be overwritten are the database DB file (if using SQLite) and the `appsettings.Production.json`.  These files should never exist in the publish output.


## Configuration
The following settings are available in appsettings.json, under the ApplicationOptions section.

When deployed to production, the application will use the `appsettings.Production.json` file, if it's present. To retain your settings between upgrades, copy your `appsettings.json` file to `appsettings.Production.json` on your production server, then make your configuration changes.

Likewise, `appsettings.Development.json` can be used while developing in Visual Studio to override the other.

Whenever there's a reference to `appsettings.json` in this document, it refers to whichever file is currently being used.

For more information on configuring ASP.NET Core, see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1.

* AllowApiLogin: Whether to allow logging in via the API controller.  API access tokens are recommended over this approach.
* BannedDevices: An array of device IDs, names, or IP addresses to ban.  When they try to connect, an uninstall command will immediately be sent back.
* DataRetentionInDays: How long event logs and remote command logs will be kept.
* DBProvider: Determines which of the three connection strings (at the top) will be used.  The appropriate DB provider for the database type is automatically loaded in code.
* DefaultPrompt: The default prompt string you'll see for each line on the console.
* EnableWindowsEventLog: Whether to also add server log entries to the Windows Event Log.
* EnforceAttendedAccess: Clients will be prompted to allow unattended remote control attempts.
* IceServers: The ICE (STUN/TURN) servers to use for WebRTC.
* KnownProxies: If your Nginx server is on a different machine and is forwarding requests to the Remotely server, you will need to add the IP of the Nginx server to this array.
* MaxOrganizationCount: By default, one organization can exist on the server, which is created automatically when the first account is registered.  Afterward, self-registration will be disabled.
    * Set this to -1 or increase it to a specific number to allow multi-tenancy.
* RedirectToHttps: Whether ASP.NET Core will redirect all traffic from HTTP to HTTPS.  This is independent of Nginx and IIS configurations that do the same.
* RemoteControlNotifyUsers: Whether to show a notification to the end user when an unattended remote control session starts.
* RemoteControlSessionLimit: How many concurrent remote control sessions are allowed per organization.
* RemoteControlRequiresAuthentication: Whether the remote control page requires authentication to establish a connection.
* Require2FA: Require users to set up 2FA before they can use the main app.
* Smpt*: SMTP settings for auto-generated system emails (such as registration and password reset).
* Theme: The color theme to use for the site.  Values are "Light" or "Dark".  This can also be configured per-user in Account - Options.
* TrustedCorsOrigins: For cross-origin API requests via JavaScript.  The websites listed in this array with be allowed to make requests to the API.  This does not grant authentication, which is still required on most endpoints.
* UseHsts: Whether ASP.NET Core will use HTTP Strict Transport Security.
* UseWebRtc: Attempt to create a peer-to-peer connection via WebRTC for screen sharing.
    * Only works on Windows agents.


## Changing the Database
By default, Remotely uses a SQLite database.  When first run, it creates a file as specified for the SQLite connection string in appsettings.json.

You can change database by changing `DBProvider` in `ApplicationOptions` to `SQLServer` or `PostgreSQL`.  Be sure to set the connection string for the new database provider in the server's `appsettings.Production.json`.

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

## Remote Control Client Requirements
* Windows: Only the latest version of Windows 10 is tested.  Windows 7 and 8.1 should work, though performance will be reduced on Windows 7.
	* Windows 2016/2019 should work as well, but isn't tested regularly.
* Linux: Only Ubuntu 18.04+ is tested.
* For the Ubuntu's "quick support" client, you must first install the following dependencies:
    * libx11-dev
    * libc6-dev
    * libgdiplus
    * libxtst-dev
    * xclip

## Remote Control on Mobile
Ideally, you'd be doing remote control from an actual computer or laptop.  However, I've tried to make the remote control at least somewhat usable from a mobile device.  Here are the controls:
* Left-click: Single tap
* Right-click: Tap and hold
* Click-and-drag: Tap and hold with one finger, tap and release a second finger (without pinch-zooming)
	* The click-and-drag operation will begin where finger one is held.

## End User Support Page
There's a page at `/GetSupport` where end users can request support.  When the form is submitted, an alert appears on the main page, above the grid.

A shortcut to this page is placed in the `\Program Files\Remotely\` folder.  You can copy it anywhere you like.  You can also have it copied to the desktop automatically by using the `-supportshortcut` switch on the installer.
	
## .NET Core Deployments
* .NET Core has two methods of deployment: framework-dependent and self-contained.
	* Framework-dependent deployments require the .NET Core runtime to be installed on the target computers.  It must be the same version that was used to build the app.
	* Self-contained deployments include a copy of the runtime, so you don't need to install it on the target computers.  As a result, the total file size is much larger.
* .NET Core uses runtime identifiers that are targeted when building.
	* Link: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog


## Shortcut Keys
There are a few shortcut keys available when using the console.
* / : Slash will open the autocomplete for selecting the current command mode.  The names are configurable in the Account - Options page.
* Up/Down: Use arrow up/down to cycle through input history.
* Ctrl + Up/Down: Scroll the console output window.
* Ctrl + Q: Clear the output window.
* Esc: Close the autocomplete window.


## API and Integrations
Remotely has a basic API, which can be browsed at https://app.remotely.one/swagger (or your own server instance).  Most endpoints require authentication via an API access token, which can be created by going to Account - API Access.

When accessing the API from the browser on another website, you'll need to set up CORS in appsettings by adding the website origin URL to the TrustedCorsOrigins array.  If you're not familiar with how CORS works, I recommend reading up on it before proceeding.  For example, if I wanted to create a login form on https://lucency.co that logged into the Remotely API, I'd need to add "https://lucency.co" to the TrustedCorsOrigins.

The API key and secret must be added to the request's Authorization header in the following format: [ApiKey]:[ApiSecret]

Below is an example API request:

	POST https://localhost:5001/API/Scripting/ExecuteCommand/PSCore/f2b0a595-5ea8-471b-975f-12e70e0f3497 HTTP/1.1
	Content-Type: application/json
	Authorization: 31fb288d-af97-4ce1-ae7b-ceebb98281ac:HLkrKaZGExYvozSPvcACZw9awKkhHnNK
	User-Agent: PostmanRuntime/7.22.0
	Accept: */*
	Cache-Control: no-cache
	Host: localhost:5001
	Accept-Encoding: gzip, deflate, br
	Content-Length: 12
	Connection: close

	Get-Location

Below are examples of using the cookie-based login API (JavaScript):

	// Log in with one request, then launch remote control with another.
	fetch("https://localhost:5001/api/Login/", {
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
	fetch("https://localhost:5001/api/RemoteControl/", {
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

## Alerts
The Alerts API gives you the ability to add monitoring and alerting functionality to your device endpoints.  This feature is intended to add basic RMM-type functionality without diverging too far from Remotely's primary purpose.

Alerts can be set up to show a notification on the Remotely website, send an email, and/or perform a separate API request.

To use Alerts, you'd first need to make an API token (or multiple tokens) for your devices to use.  Then create a scheduled task or some other recurring script to do the work.  Below is an example of how to use PowerShell to create a Scheduled Job that checks the disk space on a daily schedule.

```
$Trigger = New-JobTrigger -Daily -At "5 AM"
$Option = New-ScheduledJobOption -RequireNetwork

Register-ScheduledJob -ScriptBlock {
    $OsDrive = Get-PSDrive -Name C
    $FreeSpace = $OsDrive.Free / ($OsDrive.Used + $OsDrive.Free)
    if ($FreeSpace -lt .1) {
        Invoke-WebRequest -Uri "https://localhost:5001/api/Alerts/Create/" -Method Post -Headers @{ 
            Authorization="3e9d8273-1dc1-4303-bd50-7a133e36b9b7:S+82XKZdvg278pSFHWtUklqHENuO5IhH"
        } -Body @"
            {
                "AlertDeviceID": "f2b0a595-5ea8-471b-975f-12e70e0f3497",
                "AlertMessage": "Low hard drive space. Free Space: $([Math]::Round($FreeSpace * 100))%",
                "ApiRequestBody": null,
                "ApiRequestHeaders": null,
                "ApiRequestMethod": null,
                "ApiRequestUrl": null,
                "EmailBody": "Low hard drive space for device Maker.",
                "EmailSubject": "Hard Drive Space Alert",
                "EmailTo": "translucency_software@outlook.com",
                "ShouldAlert": true,
                "ShouldEmail": true,
                "ShouldSendApiRequest": false
            }
"@ -ContentType "application/json"
    }
} -Name "Check OS Drive Space" -Trigger $Trigger -ScheduledJobOption $Option
```
