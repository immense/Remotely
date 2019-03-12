# Remotely
A remote control and remote scripting solution, built with .NET Core and SignalR Core.

Website: https://remotely.lucency.co  
Public Server: https://tryremotely.lucency.co

## Build Instructions (Windows 10)  
The following steps will configure your Windows 10 machine for building the Remotely server and clients.
* Install .NET Core SDK.
    * Link: https://dotnet.microsoft.com/download
* Run Publish.ps1 in the Utilities folder.
    * Example: powershell -f [path]\Publish.ps1 -outdir C:\inetpub\remotely -rid win10-x86
    * The output folder will now contain the server, with the clients in the Downloads folder.
    * Change values in appsettings.json for your environment.
    * Documentation for hosting in IIS can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/index?view=aspnetcore-2.2

## Build Instructions (Linux)
* Install .NET Core SDK.
    * Link: https://dotnet.microsoft.com/download
* Install PowerShell Core.
    * Link: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux?view=powershell-6
* Run Publish.ps1 in the Utilities folder.
    * Example: pwsh -f [path]/Publish.ps1 -outdir /var/www/remotely -rid linux-x64
    * The output folder will now contain the server, with the clients in the Downloads folder.
* Run Remotely_Server_Setup.sh (with sudo) in the Utilities folder.
    * App root will be the above output folder.
    * Change values in appsettings.json for your environment.
    * Documentation for hosting behind Nginx can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2

## Notes
    * .NET Core has two methods of deployment: framework-dependent and self-contained.
        * Framework-dependent deployments require the .NET Core runtime to be installed on the target computers.  It must be the same version or higher that was used to build the app.
        * Self-contained deployments have the runtime built in, so you don't need to install it on the target computers.  As a result, the total file size is much larger.
    * .NET Core uses runtime identifiers that are targeted when building.
        * Link: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog