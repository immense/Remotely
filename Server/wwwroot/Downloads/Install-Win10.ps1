<#
.SYNOPSIS
   Installs the Remotely Client.
.DESCRIPTION
   Do not modify this script.  It was generated specifically for your account.
.EXAMPLE
   powershell.exe -f Install-Win10.ps1
   powershell.exe -f Install-Win10.ps1 -DeviceAlias "My Super Computer" -DeviceGroup "My Stuff"
#>

param (
	[string]$DeviceAlias,
	[string]$DeviceGroup,
	[string]$Path,
	[switch]$Uninstall
)

[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
$LogPath = "$env:TEMP\Remotely_Install.txt"
[string]$HostName = $null
[string]$Organization = $null
$ConnectionInfo = $null

if ([System.Environment]::Is64BitOperatingSystem){
	$Platform = "x64"
}
else {
	$Platform = "x86"
}

$InstallPath = "$env:ProgramFiles\Remotely"

function Write-Log($Message){
	Write-Host $Message
	"$((Get-Date).ToString()) - $Message" | Out-File -FilePath $LogPath -Append
}
function Do-Exit(){
	Write-Host "Exiting..."
	Start-Sleep -Seconds 3
	exit
}
function Is-Administrator() {
    $Identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $Principal = New-Object System.Security.Principal.WindowsPrincipal -ArgumentList $Identity
    return $Principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
} 

function Run-StartupChecks {

	if ($HostName -eq $null -or $Organization -eq $null) {
		Write-Log "Required parameters are missing.  Please try downloading the installer again."
		Do-Exit
	}

	if ((Is-Administrator) -eq $false) {
		Write-Log -Message "Install script requires elevation.  Attempting to self-elevate..."
		Start-Sleep -Seconds 3
		$param = "-f `"$($MyInvocation.ScriptName)`""

		Start-Process -FilePath powershell.exe -ArgumentList "-DeviceAlias $DeviceAlias -DeviceGroup $DeviceGroup -Path $Path" -Verb RunAs
		exit
	}
}

function Stop-Remotely {
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc delete Remotely_Service" -Wait -WindowStyle Hidden
	Stop-Process -Name Remotely_Agent -Force -ErrorAction SilentlyContinue
	Stop-Process -Name Remotely_Desktop -Force -ErrorAction SilentlyContinue
}

function Uninstall-Remotely {
	Stop-Remotely
	Remove-Item -Path $InstallPath -Force -Recurse -ErrorAction SilentlyContinue
	Remove-NetFirewallRule -Name "Remotely ScreenCast" -ErrorAction SilentlyContinue
}

function Install-Remotely {
	if ((Test-Path -Path "$InstallPath") -eq $true){
		if ((Test-Path -Path "$InstallPath\ConnectionInfo.json") -eq $true){
			$ConnectionInfo = Get-Content -Path "$InstallPath\ConnectionInfo.json" | ConvertFrom-Json
			if ($ConnectionInfo -ne $null) {
				$ConnectionInfo.Host = $HostName
				$ConnectionInfo.OrganizationID = $Organization
				$ConnectionInfo.ServerVerificationToken = ""
			}
		
		}
	}
	else {
		New-Item -ItemType Directory -Path "$InstallPath" -Force
	}

	if ($ConnectionInfo -eq $null) {
		$ConnectionInfo = @{
			DeviceID = (New-Guid).ToString();
			Host = $HostName;
			OrganizationID = $Organization;
			ServerVerificationToken = "";
		}
	}

	if ($HostName.EndsWith("/")) {
		$HostName = $HostName.Substring(0, $HostName.LastIndexOf("/"))
	}

	if ($Path) {
		Write-Log "Copying install files..."
		Copy-Item -Path $Path -Destination "$env:TEMP\Remotely-Win10-$Platform.zip"

	}
	else {
		$ProgressPreference = 'SilentlyContinue'
		Write-Log "Downloading client..."
		Invoke-WebRequest -Uri "$HostName/Downloads/Remotely-Win10-$Platform.zip" -OutFile "$env:TEMP\Remotely-Win10-$Platform.zip" 
		$ProgressPreference = 'Continue'
	}

	if (!(Test-Path -Path "$env:TEMP\Remotely-Win10-$Platform.zip")) {
		Write-Log "Client files failed to download."
		Do-Exit
	}

	Stop-Remotely
	Get-ChildItem -Path "C:\Program Files\Remotely" | Where-Object {$_.Name -notlike "ConnectionInfo.json"} | Remove-Item -Recurse -Force

	Expand-Archive -Path "$env:TEMP\Remotely-Win10-$Platform.zip" -DestinationPath "$InstallPath"  -Force

	New-Item -ItemType File -Path "$InstallPath\ConnectionInfo.json" -Value (ConvertTo-Json -InputObject $ConnectionInfo) -Force

	if ($DeviceAlias -or $DeviceGroup) {
		$DeviceSetupOptions = @{
			DeviceAlias = $DeviceAlias;
			DeviceGroup = $DeviceGroup;
		}

		New-Item -ItemType File -Path "$InstallPath\DeviceSetupOptions.json" -Value (ConvertTo-Json -InputObject $DeviceSetupOptions) -Force
	}

	New-Service -Name "Remotely_Service" -BinaryPathName "$InstallPath\Remotely_Agent.exe" -DisplayName "Remotely Service" -StartupType Automatic -Description "Background service that maintains a connection to the Remotely server.  The service is used for remote support and maintenance by this computer's administrators."
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc.exe failure `"Remotely_Service`" reset=5 actions=restart/5000" -Wait -WindowStyle Hidden
	Start-Service -Name Remotely_Service

	New-NetFirewallRule -Name "Remotely Desktop" -DisplayName "Remotely ScreenCast" -Description "The agent that allows screen sharing and remote control for Remotely." -Direction Inbound -Enabled True -Action Allow -Program "C:\Program Files\Remotely\Desktop\Remotely_Desktop.exe" -ErrorAction SilentlyContinue
}

function Install-DesktopRuntime() {
	$UninstallKeys = New-Object System.Collections.ArrayList
	Get-ChildItem -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" | ForEach-Object {
		$UninstallKeys.Add($_) | Out-Null
	}

	if ([System.Environment]::Is64BitOperatingSystem) {
		Get-ChildItem -Path "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" | ForEach-Object {
		  $UninstallKeys.Add($_) | Out-Null
		}
	}

	$RuntimeRegKey = $UninstallKeys | Where-Object {
		$_.GetValue("DisplayName") -like "Microsoft Windows Desktop Runtime - 3.1.1*" 
	}

	if ($RuntimeRegKey -eq $null) {
		Write-Host ".NET Core Windows Desktop runtime not found.  Downloading installer."
		$Response = Invoke-WebRequest -Uri "https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.1-windows-$Platform-installer" -UseBasicParsing
		$DownloadLink = $Response.Links | Where-Object { $_.href -like "*windowsdesktop-runtime*" }
		$ProgressPreference = 'SilentlyContinue'
		Invoke-WebRequest -Uri $DownloadLink.href -OutFile "$env:TEMP\windowsdesktop-runtime.exe"
		$ProgressPreference = 'Continue'
		Write-Host "Installing .NET Core Windows Desktop runtime."
		Start-Process -FilePath "$env:TEMP\windowsdesktop-runtime.exe" -ArgumentList "/install /quiet /norestart" -Wait
	}

}

try {
	Run-StartupChecks

	Write-Log "Install/uninstall logs are being written to `"$LogPath`""
    Write-Log

	if ($Uninstall) {
		Write-Log "Uninstall started."
		Uninstall-Remotely
		Write-Log "Uninstall completed."
		exit
	}
	else {
		Write-Log "Install started."
        Write-Log
		Install-DesktopRuntime
		Install-Remotely
		Write-Log "Install completed."
		exit
	}
}
catch {
	Write-Log -Message "Error occurred: $($Error[0].InvocationInfo.PositionMessage)"
	Do-Exit
}
