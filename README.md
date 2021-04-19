# Remotely
A remote control and remote scripting solution, built with .NET 5, Blazor, SignalR Core, and WebRTC.

[![Build Status](https://dev.azure.com/translucency/Remotely/_apis/build/status/Remotely-ReleaseBuild?branchName=master)](https://dev.azure.com/translucency/Remotely/_build/latest?definitionId=17&branchName=master)
![GitHub Build](https://github.com/lucent-sea/Remotely/workflows/GitHub%20Build/badge.svg)


## Donations
If this project has benefited you in some way, or if you just want to show appreciation, please consider donating to a charity.

Suggested Charities: https://www.givewell.org/charities/top-charities

You can also sponsor the project as a way of saying "thank you".  But if you have to pick between the two, please choose a charity.  Your money will have a much greater impact on their lives than it would mine.

[![GitHub Sponsors](https://img.shields.io/badge/GitHub%20Sponsors-Sponsor-brightgreen)](https://github.com/sponsors/lucent-sea)

[![PayPal Link](https://img.shields.io/badge/PayPal-Donate-brightgreen)](https://www.paypal.me/translucency)


## Project Links
Public Server: https://app.remotely.one  
Website: https://remotely.one  
Subreddit: https://www.reddit.com/r/remotely_app/  
Docker: https://hub.docker.com/repository/docker/translucency/remotely  

![image](https://user-images.githubusercontent.com/20995508/113913261-f7002a00-9790-11eb-81b3-c36fb8aa536d.png)

## WARNING:
Remotely is undergoing a major overhaul for its server installation process.  There will be some instability for a short time, and the documentation below may not reflect the current state.  It's recommended that you use the installation scripts from the latest full release if you don't wish to participate in testing.

## Disclaimer
Hosting a Remotely server requires running an ASP.NET Core web app behind IIS (Windows), Nginx (Ubuntu), or Caddy Server (any OS).  It's expected that the person deploying and maintaining the server is familiar with this process.  Since this is a hobby project that I develop in between working full time and raising a family, there simply isn't time available to provide support in this capacity.

## GitHub Actions
GitHub Actions allows you to build and deploy Remotely for free from their cloud servers.  Since the Windows agent can only be built on Windows, and the Mac agent can only be built on Mac, using a build platform like GitHub Actions or Azure Pipelines is the only reasonable way to build the whole project.  The definitions for the build processes are located in `/.github/workflows/` folder.

I've created a cross-platform command line tool that can leverage the GitHub Actions REST API to build the project and install it on your private server.  This process will also embed your server's URL into the desktop clients, so that they won't need to prompt the end user to enter it.

## Installation Instructions:
- Before attempting installation, verify that your domain name is resolving to your server's IP address.
  - For example, I can use the command `ping app.remotely.one` and see the IP address to which it resolves.
- Find the `Remotely_Server_Installer[.exe]` CLI tool for the latest release on the [Releases page](https://github.com/lucent-sea/Remotely/releases).
  - You will download and run it on the server where you'll be hosting Remotely.
  - You can choose between installing the pre-built release package, or entering GitHub credentials to build and install a customized server.
  - The pre-built package will not have your server's URL embedded in the clients.  End users will need to enter it manually.
- If you use the pre-built package, you're done!  Otherwise, follow the below steps for using the GitHub Actions integration.
- Fork the repo if you haven't already.
- Go to the Actions tab in your forked repo and make sure you can see the Build workflows.
  - Before you can use Actions for the first time, there will be prompt that you must accept on this page.
- Create a Personal Access Token that the installer will use to authorize with GitHub.
  - Located here: https://github.com/settings/tokens
  - It needs to have the `repo` scope.
  - Save the PAT when it's displayed.  It will only be shown once.
- By default, the server will be built from the author's repo.
  - If you want to build from your fork, comment out the `repository` line in `Build.yml` (in your repo).  There's a comment in the file that points out the line.
- On your server, download the latest server installer executable (Linux or Windows) from [my releases page](https://github.com/lucent-sea/Remotely/releases).
- Run the app with elevation (e.g. sudo or "Run as admin").
- Follow the prompts to build and install the server.
- Use `--help` argument to see all the command line arguments.
  - If values are provided for all arguments, it will run non-interactive.

## After Installation
- In the site's content directory, make a copy of the `appsettings.json` file and name it `appsettings.Production.json`.
  - The server will use this new file for reading/writing its settings, and it won't be overwritten by future ugprades.
- If using Caddy, a TLS cert will be installed automatically.
  - When installling on Nginx, the script will use Certbot and prompt you to install a cert.
  - For Windows IIS, you'll need to use a separate program that integrates with Let's Encrypt.
    - Resources: https://letsencrypt.org/docs/client-options/#clients-windows-/-iis
- By default, SQLite is used for the database.
    - The "Remotely.db" database file is automatically created in the root folder of your site.
	- You can browse and modify the contents using [DB Browser for SQLite](https://sqlitebrowser.org/).

## Upgrading
* To upgrade a server, do any of the below to copy the new Server application files.
	* Run one of the GitHub Actions workflows, then copy the ZIP contents to the site's content folder.
	* Build from source as described above and `rsync`/`robocopy` the output files to the server directory.
	* Build from source and deploy to IIS (e.g. `dotnet publish /p:PublishProfile=MyProfile`)
* For Linux, you'll need to restart the Remotely service in systemd after overwriting the files.
* For Windows, you'll need to shut down the site's Application Pool in IIS before copying the files.
	* Windows won't let you overwrite files that are in use.
* The only things that shouldn't be overwritten are the database DB file (if using SQLite) and the `appsettings.Production.json`.  These files should never exist in the publish output.

## Hosting Scenarios
There are countless ways to host an ASP.NET Core app, and I can't document or automate all of them.  For hosting scenarios aside from the above two, please refer to Microsoft's documentation.
- https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/


## Build and Debug Instructions (Windows 10)  
The following steps will configure your Windows 10 machine for building the Remotely server and clients.
* Install Visual Studio 2019.
    * Link: https://visualstudio.microsoft.com/downloads/
	* You only need the below Individual Components for building:
	    * .NET SDK (latest version).
		* MSBuild (which auto-selects Roslyn compilers).
		* NuGet targets and build tasks.
		* .NET Framework 4.8 SDK.
	    * For debugging and development, you'll need all relevant workloads.
* Install Git for Windows.
    * Link: https://git-scm.com/downloads
* Clone the git repository: `git clone https://github.com/lucent-sea/remotely`
* When debugging, the agent will use a pre-defined device ID and connect to https://localhost:5001.
* In development environment, the server will assign all connecting agents to the first organization.
* The above two allow you to debug the agent and server together, and see your device in the list.

## Admin Accounts
The first account created will be an admin for both the server and the organization that's created for the account.

An organization admin has access to the Organization page and server log entries specific to his/her organization.  A server admin has access to the Server Config page and can see server log entries that don't belong to an organization. 


## Branding
Within the Account section, there is a tab for branding, which will apply to the quick support clients and Windows installer.

However, the clients will need to have been built from source with the server URL hard-coded in the apps for them to be able to retrieve the branding info.

## Configuration
The following settings are available in appsettings.json, under the ApplicationOptions section.

When deployed to production, the application will use the `appsettings.Production.json` file, if it's present. To retain your settings between upgrades, copy your `appsettings.json` file to `appsettings.Production.json` on your production server, then make your configuration changes.

Likewise, `appsettings.Development.json` can be used while developing in Visual Studio to override the other.

Whenever there's a reference to `appsettings.json` in this document, it refers to whichever file is currently being used.

For more information on configuring ASP.NET Core, see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/.

* AllowApiLogin: Whether to allow logging in via the API controller.  API access tokens are recommended over this approach.
* BannedDevices: An array of device IDs, names, or IP addresses to ban.  When they try to connect, an uninstall command will immediately be sent back.
* DataRetentionInDays: How long logs and other data will be kept on the server.  Set to -1 to retain indefinitely (not recommended).
* DBProvider: Determines which of the three connection strings (at the top) will be used.  The appropriate DB provider for the database type is automatically loaded in code.
* EnableWindowsEventLog: Whether to also add server log entries to the Windows Event Log.
* EnforceAttendedAccess: Clients will be prompted to allow unattended remote control attempts.
* IceServers: The ICE (STUN/TURN) servers to use for WebRTC.
* KnownProxies: If your reverse proxy is on a different machine and is forwarding requests to the Remotely server, you will need to add the IP of the reverse proxy server to this array.
* MaxOrganizationCount: By default, one organization can exist on the server, which is created automatically when the first account is registered.  Afterward, self-registration will be disabled.
    * Set this to -1 or increase it to a specific number to allow multi-tenancy.
* RedirectToHttps: Whether ASP.NET Core will redirect all traffic from HTTP to HTTPS.  This is independent of Caddy, Nginx, and IIS configurations that do the same.
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
	* More information: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/

## Remote Control Client Requirements
* Windows: Only the latest version of Windows 10 is tested.  Windows 7 and 8.1 should work, though performance will be reduced on Windows 7.
	* Windows 2016/2019 should work as well, but isn't tested regularly.
* Linux: Only Ubuntu 18.04+ is tested.
* For the Ubuntu's "quick support" client, you must first install the following dependencies:
    * libx11-dev
	* libxrandr-dev
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
	
## .NET Deployments
* .NET has two methods of deployment: framework-dependent and self-contained.
	* Framework-dependent deployments require the .NET runtime to be installed on the target computers.  It must be the same version that was used to build the app.
	* Self-contained deployments include a copy of the runtime, so you don't need to install it on the target computers.  As a result, the total file size is much larger.
* .NET uses runtime identifiers that are targeted when building.
	* Link: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog


## Shortcut Keys
There are a few shortcut keys available when using the console.
* / : Slash will allow you to switch between shells.  The names are configurable in the Options page.
* Up/Down: Use arrow up/down to cycle through input history.
* Ctrl + Q: Clear the output window.

## Port Configuration
You can change the local port that the Remotely .NET server listens on by adding the below to `appsettings.Production.json`:

```
"Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:{port-number}"
      }
    }
  }
```

Alternatively, you can use a command-line argument for the `Remotely_Server` process or set an environment variable.
  - `--urls http://localhost:{port-number}`
  - `ASPNETCORE_URLS=http://localhost:{port-number}`

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
