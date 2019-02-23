# Remotely
A remote control and remote scripting solution, built with .NET Core and SignalR Core.

Website: https://remotely.lucency.co  
Public Server: https://getremotely.lucency.co

## Build Instructions (Windows 10)  
The following steps will configure your Windows 10 machine for building the Remotely server and clients.
* Install .NET Core SDK.
    * Here: https://www.microsoft.com/net/download
    * Or here: https://dot.net/v1/dotnet-install.ps1
* Run Publish.ps1 in the Utilities folder.
    * The output folder will now contain the server with the clients in the Downloads folder.
    * Change values in appsettings.json for your environment.
    * Documentation for hosting in IIS can be found here: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/index?view=aspnetcore-2.1

## Build Instructions (Linux)
See Setup_Ubuntu_Builder.sh and Remotely_Server_Install.sh in Utilities.