<#
.SYNOPSIS
   Configures IIS and installs the Remotely server.
.COPYRIGHT
   Copyright ©  2020 Translucency Software.  All rights reserved.
#>
param (
     # The name to use for the IIS Application Pool for the Remotely site.
    [Parameter(Mandatory=$True)]
	[string]$AppPoolName,
     # The name to use for the IIS site.
    [Parameter(Mandatory=$True)]
	[string]$SiteName,
    # The folder path where the Remotely server files are located.
    [Parameter(Mandatory=$True)]
    [string]$SitePath,
    # Whether to run the script without any prompts.
    [switch]$Quiet,
    # The path to Windows ACME Simple (wacs.exe) to use for automatically obtaining and
    # installing a Let's Encrypt certificate.
    # (Project and downloads: https://github.com/win-acme/win-acme)
    [string]$WacsPath,
    # The email address to use when registering the certificate with WACS.
    [string]$EmailAddress
)

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$ErrorActionPreference = "Stop"
$Host.UI.RawUI.WindowTitle = "Remotely Setup"
Clear-Host

#region Variables
$ServerCmdlets = $false
$FirewallSet = $false
$CopyErrors = $false
if ($PSScriptRoot -eq ""){
    $PSScriptRoot = (Get-Location)
}

$Root = (Get-Item -Path $PSScriptRoot).Parent.FullName
$BindingHostname = ""
#endregion

#region Functions
function Do-Pause {
    if (!$Quiet){
        pause
    }
}
function Wrap-Host
{
    [CmdletBinding()]
    [Alias()]
    [OutputType([int])]
    Param
    (
        # The text to write.
        [Parameter(Mandatory=$false,
                   ValueFromPipelineByPropertyName=$true,
                   Position=0)]
        [String]
        $Text,

        # Param2 help description
        [Parameter(Mandatory=$false,
                   ValueFromPipelineByPropertyName=$true,
                   Position=1)]
        [ConsoleColor]
        $ForegroundColor
    )

    Begin
    {
    }
    Process
    {
        if (!$Text){
            Write-Host
            return
        }
        $Width = $Host.UI.RawUI.BufferSize.Width
        $SB = New-Object System.Text.StringBuilder
        while ($Text.Length -gt $Width) {
            [int]$LastSpace = $Text.Substring(0, $Width).LastIndexOf(" ")
            $SB.AppendLine($Text.Substring(0, $LastSpace).Trim()) | Out-Null
            $Text = $Text.Substring(($LastSpace), $Text.Length - $LastSpace).Trim()
        }
        $SB.Append($Text) | Out-Null
        if ($ForegroundColor)
        {
            Write-Host $SB.ToString() -ForegroundColor $ForegroundColor
        }
        else
        {
            Write-Host $SB.ToString()
        }
        
    }
    End
    {
    }
}

#endregion


#region Prerequisite Tests
### Test if process is elevated. ###
$User = [Security.Principal.WindowsIdentity]::GetCurrent()
if ((New-Object Security.Principal.WindowsPrincipal $User).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator) -eq $false) {
    Wrap-Host
    Wrap-Host "Error: This installation needs to be run from an elevated process (Run as Administrator)." -ForegroundColor Red
    Do-Pause
    return
}
### Check PS version. ###
if ((Get-Host).Version.Major -lt 5) {
    Wrap-Host
    Wrap-Host "Error: PowerShell 5 is required.  Please install it via the Windows Management Framework 5.1 download from Microsoft." -ForegroundColor Red
    Do-Pause
    return
}
### Check Script Root ###
if (!$PSScriptRoot) {
    Wrap-Host
    Wrap-Host "Error: Unable to determine working directory.  Please make sure you're running the full script and not just a section." -ForegroundColor Red
    Do-Pause
    return
}

### Check OS version. ###
$OS = Get-WmiObject -Class Win32_OperatingSystem
if ($OS.Name.ToLower().Contains("home") -or $OS.Caption.ToLower().Contains("home")) {
    Wrap-Host
    Wrap-Host "Error: Windows Home version does not have the necessary features to run Remotely." -ForegroundColor Red
    Do-Pause
    return
}
### Test if Windows Feature cmdlets are available. ###
if ((Get-Command -Name "Add-WindowsFeature" -ErrorAction Ignore) -eq $null) {
    $ServerCmdlets = $false
}
else {
    $ServerCmdlets = $true
}
### Check if PostgreSQL is installed. ###
##if ((Get-Package -Name "*PostgreSQL*" -ErrorAction SilentlyContinue) -eq $null){
##    Wrap-Host
##    Wrap-Host "ERROR: PostgreSQL was not found.  Please install it from https://postgresql.org." -ForegroundColor Red
##    Wrap-Host
##    Do-Pause
##    return
##}

$HostingPackage = Get-Package -Name "*.NET*Windows Server Hosting*" -ErrorAction SilentlyContinue
$Result = Invoke-WebRequest -Uri "https://dotnet.microsoft.com/download/dotnet/current/runtime" -UseBasicParsing
$BundleLink = $Result.Links | Where-Object {$_.outerHTML -like "*Download Hosting Bundle*"}
$Result = Invoke-WebRequest -Uri ("https://dotnet.microsoft.com$($BundleLink.href)") -UseBasicParsing
$DownloadLink = $Result.Links | Where-Object {$_.outerHTML -like "*Click here to download manually*"}
$Parts = $DownloadLink.href.Split("-")
$RemoteVersionString = $Parts[$Parts.Length - 2]

if ($HostingPackage -ne $null) {
    $LocalVersion = [System.Version]::Parse($HostingPackage.Version)

}
$RemoteVersion = [System.Version]::Parse($RemoteVersionString)

if ($HostingPackage -eq $null -or $RemoteVersion -gt $LocalVersion) {
    Wrap-Host "Downloading .NET Core Runtime and Hosting Bundle..."
    $ProgressPreference = "SilentlyContinue"
    Invoke-WebRequest -Uri $DownloadLink.href -OutFile "$env:TEMP\dotnet-hosting-win.exe" -UseBasicParsing
    $ProgressPreference = "Continue"
    Start-Process -FilePath "$env:TEMP\dotnet-hosting-win.exe" -ArgumentList "/install /quiet /norestart" -Wait
    Wrap-Host
    Wrap-Host ".NET Runtime installation completed."
}
else {
    Wrap-Host ".NET Hosting Bundle is up-to-date."
}

#endregion

### Hosting Requirements ###
Wrap-Host
Wrap-Host "**********************************"
Wrap-Host "           IMPORTANT" -ForegroundColor Green
Wrap-Host "**********************************"
Wrap-Host
Wrap-Host "You must have the .NET Core Runtime installed, which includes the IIS hosting module. The SDK doesn't have this."
Wrap-Host
Wrap-Host "If you have not already done so, please download and install it from the following link:"
Wrap-Host
Wrap-Host "https://dotnet.microsoft.com/download"
Wrap-Host
Do-Pause
Clear-Host


### Intro ###
Clear-Host
Wrap-Host
Wrap-Host "**********************************"
Wrap-Host "         Remotely Setup" -ForegroundColor Cyan
Wrap-Host "**********************************"
Wrap-Host
Wrap-Host "Hello, and thank you for trying out Remotely!" -ForegroundColor Green
Wrap-Host
Wrap-Host "This setup script will create an IIS site and install Remotely on this machine." -ForegroundColor Green
Wrap-Host
Do-Pause
Clear-Host


### Automatic IIS Setup ###
if ($ServerCmdlets) {
    $RebootRequired = $false
    Wrap-Host
    Wrap-Host "Installing IIS components..." -ForegroundColor Green
    Write-Progress -Activity "IIS Component Installation" -Status "Installing web server" -PercentComplete (1/7*100)
    $Result = Add-WindowsFeature Web-Server
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }
    #Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET" -PercentComplete (2/7*100)
    #$Result = Add-WindowsFeature Web-Asp-Net
    #if ($Result.RestartNeeded -like "Yes")
    #{
    #    $RebootRequired = $true
    #}
    #Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET 4.5" -PercentComplete (3/7*100)
    #$Result = Add-WindowsFeature Web-Asp-Net45
    #if ($Result.RestartNeeded -like "Yes")
    #{
    #    $RebootRequired = $true
    #}
    #Write-Progress -Activity "IIS Component Installation" -Status "Installing web sockets" -PercentComplete (4/7*100)
    #$Result = Add-WindowsFeature Web-WebSockets
    #if ($Result.RestartNeeded -like "Yes")
    #{
    #    $RebootRequired = $true
    #}
    Write-Progress -Activity "IIS Component Installation" -Status "Installing IIS management tools" -PercentComplete (5/7*100)
    $Result = Add-WindowsFeature Web-Mgmt-Tools
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }
    #Write-Progress -Activity "IIS Component Installation" -Status "Installing web filtering" -PercentComplete (6/7*100)
    #$Result = Add-WindowsFeature Web-Filtering
    #if ($Result.RestartNeeded -like "Yes")
    #{
    #    $RebootRequired = $true
    #}

    Write-Progress -Activity "IIS Component Installation" -Status "IIS setup completed" -PercentComplete (7/7*100) -Completed
    Start-Sleep 2
}
else
{
    Wrap-Host
    Wrap-Host "Installing IIS components..." -ForegroundColor Green
    Write-Progress -Activity "IIS Component Installation" -Status "Installing web server" -PercentComplete (1/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-WebServer /All /Quiet

    #Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET" -PercentComplete (2/7*100)
    #DISM /Online /Enable-Feature /FeatureName:IIS-ASPNET /All /Quiet

    #Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET 4.5" -PercentComplete (3/7*100)
    #DISM /Online /Enable-Feature /FeatureName:IIS-ASPNET45 /All /Quiet

    #Write-Progress -Activity "IIS Component Installation" -Status "Installing web sockets" -PercentComplete (4/7*100)
    #DISM /Online /Enable-Feature /FeatureName:IIS-WebSockets /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "Installing IIS management tools" -PercentComplete (5/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-ManagementConsole /All /Quiet

    #Write-Progress -Activity "IIS Component Installation" -Status "Installing web filtering" -PercentComplete (6/7*100)
    #DISM /Online /Enable-Feature /FeatureName:IIS-RequestFiltering /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "IIS setup completed" -PercentComplete (7/7*100) -Completed
    Start-Sleep 2

}

Clear-Host
### Create IIS Site ##

if ((Get-IISAppPool -Name $AppPoolName) -eq $null) {
    New-WebAppPool -Name $AppPoolName
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -name processModel.identityType -Value 4
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -name processModel.loadUserProfile -Value $true
}

if ((Get-Website -Name $SiteName) -eq $null) {
    New-Website -Name $SiteName -PhysicalPath $SitePath -HostHeader $BindingHostname -ApplicationPool $AppPoolName
}


Wrap-Host
Wrap-Host "This will DELETE ALL FILES in the selected website and install Remotely Server.  If this is not your intention, close this window now and create a new website where Remotely Server will be installed." -ForegroundColor Red
Wrap-Host
Do-Pause

# Stop site.
Clear-Host
Wrap-Host
Wrap-Host "Stopping website..." -ForegroundColor Green
Stop-Website -Name $SiteName


### File Cleanup ###
Wrap-Host
Wrap-Host "Cleaning up existing files..." -ForegroundColor Green
$Success = $false

while ($Success -eq $false) {
    try {
        Get-ChildItem -Path "$SitePath" -Recurse | Remove-Item -Force -Recurse
        $Success = $true
    }
    catch {
        Start-Sleep -Seconds 1
    }
}

### Set ACL on website folders and files ###
Wrap-Host
Wrap-Host "Setting ACLs..." -ForegroundColor Green
$Acl = Get-Acl -Path $SitePath
$Rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\IIS_IUSRS", "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$Acl.AddAccessRule($Rule)
$Acl.SetOwner((New-Object System.Security.Principal.NTAccount("Administrators")))
Set-Acl -Path $SitePath -AclObject $Acl
Get-ChildItem -Path $SitePath -Recurse | ForEach-Object {
    Set-Acl -Path $_.FullName -AclObject $Acl   
}

### Firewall Rules ###
Wrap-Host
Wrap-Host "Checking firewall rules for HTTP/HTTPS..." -ForegroundColor Green
try
{
    Enable-NetFirewallRule -Name "IIS-WebServerRole-HTTP-In-TCP"
    Enable-NetFirewallRule -Name "IIS-WebServerRole-HTTPS-In-TCP"
    if ((Get-NetFirewallRule -Name "IIS-WebServerRole-HTTP-In-TCP").Enabled -like "False" -or (Get-NetFirewallRule -Name "IIS-WebServerRole-HTTP-In-TCP").Enabled -like "False")
    {
        $FirewallSet = $false
    }
    else
    {
        $FirewallSet = $true
    }
}
catch
{
    $FirewallSet = $false
}

# Start website.
Start-Website -Name $SiteName


### SSL certificate installation. ###
if ($WacsPath) {
    if (Test-Path -Path $WacsPath) {
        &"$WacsPath" --target iis --siteid (Get-Website -Name $SiteName).ID --installation iis --emailaddress $EmailAddress --accepttos 
    }
}

Wrap-Host
Wrap-Host
Wrap-Host
Wrap-Host "**********************************"
Wrap-Host "      Server setup complete!" -ForegroundColor Green
Wrap-Host "**********************************"
Wrap-Host
Wrap-Host "If a path to Win-Acme exe path (WacsPath) wasn't provided, SSL/TLS needs to be set up in IIS.  I recommend checking out Let's Encrypt for free, automated SSL certificates." -ForegroundColor Green

if ($RebootRequired) {
    Wrap-Host
    Wrap-Host "A reboot is required for the new IIS components to work properly.  Please reboot your computer at your earliest convenience." -ForegroundColor Red
}
if ($FirewallSet -eq $false)
{
    Wrap-Host
    Wrap-Host "Firewall rules were not properly set.  Please ensure that ports 80 (HTTP) and 443 (HTTPS) are open.  Windows Firewall has predefined rules for these called ""World Wide Web Services (HTTP(S) Traffic-In)""." -ForegroundColor Red
}

if ($CopyErrors)
{
    Wrap-Host
    Wrap-Host "There were errors copying some of the server files.  Please try deleting all files in the website directory and trying again." -ForegroundColor Red
}
Wrap-Host
Do-Pause