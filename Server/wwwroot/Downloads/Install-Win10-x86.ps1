<#
.SYNOPSIS
   Installs the Remotely Client.
.DESCRIPTION
   Do not modify this script.  It was generated specifically for your account.
.EXAMPLE
   powershell.exe -f Install-Win10-x86.ps1
#>

[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
$LogPath = "$env:TEMP\Remotely_Install.txt"
[string]$HostName = $null
[string]$Organization = $null
$ConnectionInfo = $null
$ArgList = New-Object System.Collections.ArrayList
foreach ($arg in $args){
	$ArgList.Add($arg.ToLower()) | Out-Null
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
	if ([System.Environment]::Is64BitOperatingSystem -eq $true){
		Write-Log -Message "This script is for 32-bit operating systems.  Use the x64 (64-bit) install script on this device."
		Do-Exit
	}

	if ($HostName -eq $null -or $Organization -eq $null) {
		Write-Log "Required parameters are missing.  Please try downloading the installer again."
		Do-Exit
	}

	if ((Is-Administrator) -eq $false) {
		Write-Log -Message "Install script requires elevation.  Attempting to self-elevate..."
		Start-Sleep -Seconds 3
		$param = "-f `"$($MyInvocation.ScriptName)`""
		foreach ($arg in $ArgList){
			$param += " $arg"
		}
		Start-Process -FilePath powershell.exe -ArgumentList "$param" -Verb RunAs
		exit
	}
}

function Uninstall-Remotely {
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc delete Remotely_Service" -Wait -WindowStyle Hidden
	Stop-Process -Name Remotely_Agent -Force -ErrorAction SilentlyContinue
	Remove-Item -Path $InstallPath -Force -Recurse -ErrorAction SilentlyContinue
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
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc delete Remotely_Service" -Wait  -WindowStyle Hidden
    Get-Process | Where-Object {$_.Name -like "Remotely_Agent"} | Stop-Process -Force
	Get-ChildItem -Path "C:\Program Files\Remotely" | Where-Object {$_.Name -notlike "ConnectionInfo.json"} | Remove-Item -Recurse -Force

	if ($HostName.EndsWith("/")) {
		$HostName = $HostName.Substring(0, $HostName.LastIndexOf("/"))
	}

	if ($ArgList.Contains("-path")) {
		Write-Log "Copying install files..."
		$SourceIndex = $ArgList.IndexOf("-path") + 1
		$SourcePath = $ArgList[$SourceIndex].Replace("`"", "").Replace("'", "")
		Copy-Item -Path $ArgList[$SourceIndex] -Destination "$InstallPath\Remotely-Win10-x86.zip"
	}
	else {
		$ProgressPreference = 'SilentlyContinue'
		Write-Log "Downloading client..."
		Invoke-WebRequest -Uri "$HostName/Downloads/Remotely-Win10-x86.zip" -OutFile "$InstallPath\Remotely-Win10-x86.zip"
		$ProgressPreference = 'Continue'

	}
	Expand-Archive -Path "$InstallPath\Remotely-Win10-x86.zip" "$InstallPath"  -Force

	New-Item -ItemType File -Path "$InstallPath\ConnectionInfo.json" -Value (ConvertTo-Json -InputObject $ConnectionInfo) -Force

	New-Service -Name "Remotely_Service" -BinaryPathName "$InstallPath\Remotely_Agent.exe" -DisplayName "Remotely Service" -StartupType Automatic -Description "Background service that maintains a connection to the Remotely server.  The service is used for remote support and maintenance by this computer's administrators."
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc.exe failure `"Remotely_Service`" reset=5 actions=restart/5000" -Wait -WindowStyle Hidden
	Start-Service -Name Remotely_Service
}

try {
	Run-StartupChecks

	if ($ArgList.Contains("-uninstall")) {
		Write-Log "Install started."
        Write-Log
        Write-Log "Install/uninstall logs are being written to `"$LogPath`""
        Write-Log
		Install-Remotely
		Write-Log "Install completed."
		exit
	}
	else {
		Write-Log "Install started."
		Install-Remotely
		Write-Log "Install completed."
		exit
	}
}
catch {
	Write-Log -Message "Error occurred: $($Error[0].InvocationInfo.PositionMessage)"
	Do-Exit
}