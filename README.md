# Remotely
[![Build Status](https://dev.azure.com/translucency/Remotely/_apis/build/status/Remotely?branchName=master)](https://dev.azure.com/translucency/Remotely/_build/latest?definitionId=2&branchName=master)


A remote control and remote scripting solution, built with .NET Core and SignalR Core.

Website: https://remotely.lucency.co  
Public Server: https://tryremotely.lucency.co

## Build Instructions (Windows 10)  
The following steps will configure your Windows 10 machine for building the Remotely server and clients.
* Install Visual Studio 2019.
    * Link: https://visualstudio.microsoft.com/downloads/
* Install the latest .NET Core SDK.
    * Link: https://dotnet.microsoft.com/download
* Clone the git repository and open the solution in Visual Studio.
* Build (in Release configuration) the Remotely_Desktop and Remotely_ScreenCast projects.
	* By default, the screen-sharing desktop app prompts for a host URL and can be changed thereafter.  To hard-code a URL, set the ForceHost value in /Remotely_Desktop/ViewModels/MainWindowViewModel.cs to the server's URL.
* Run Publish.ps1 in the Utilities folder.
    * Example: powershell -f [path]\Publish.ps1 -outdir C:\inetpub\remotely -rid win10-x86
    * The output folder will now contain the server, with the clients in the Downloads folder.

## Hosting a Server (Windows)
* Download and install the .NET Core Runtime.
	* Link: https://dotnet.microsoft.com/download
	* This includes the Hosting Bundle for Windows, which allows you to run ASP.NET Core in IIS.
* Download and unzip the Remotely Windows server package into your IIS site folder.
* An SSL certificate for HTTPS is required.  You can install one for free using Let's Encrypt.
	* Resources: https://letsencrypt.org/, https://certifytheweb.com/
* Documentation for hosting in IIS can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/index?view=aspnetcore-2.2

## Hosting a Server (Linux)
* Download and install the .NET Core Runtime.
	* Link: https://dotnet.microsoft.com/download
* Download and unzip the Remotely Linux server package.
* These files would typically go in /var/www/remotely/.
* Run Remotely_Server_Setup.sh (with sudo), which is in the Utilities folder in source control.
	* Certbot is used in this script and will install an SSL certificate for your site.  Your server needs to have a public domain name that is accessible from the internet.
		* More information: https://letsencrypt.org/, https://certbot.eff.org/
    * App root will be the above output folder.
* Change values in appsettings.json for your environment.
* Documentation for hosting behind Nginx can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2

## Session Recording
* You can turn on session recording in appsettings.json.
* The following requirements must be met for it to work:
	* On Linux, libgdiplus must be installed (sudo apt-get install libdgiplus).
	* The process running the app must have access to create and/or modify a folder name "Recordings" in the site's root content folder.
	* FFmpeg must be downloaded and in the same folder as your web app's EXE/DLL.
		* Link: https://www.ffmpeg.org/download.html
* Remote control sessions will first be recorded as a series of images, which will then be converted to MP4 using FFmpeg.

## Remote Control on Mobile
Ideally, you'd be doing remote control from an actual computer or laptop.  However, I've tried to make the remote control at least somewhat usable from a mobile device.  Here are the controls:
* Left-click: Single tap
* Right-click: Double tap
* Click-and-drag: Tap and hold with one finger, tap and release a second finger (without pinch-zooming)
	* The click-and-drag operation will begin where finger one is held

## .NET Core Deployments
* .NET Core has two methods of deployment: framework-dependent and self-contained.
	* Framework-dependent deployments require the .NET Core runtime to be installed on the target computers.  It must be the same version or higher that was used to build the app.
	* Self-contained deployments include a copy of the runtime, so you don't need to install it on the target computers.  As a result, the total file size is much larger.
* .NET Core uses runtime identifiers that are targeted when building.
	* Link: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
