<#
.SYNOPSIS
   Configures IIS and installs the Remotely server.
.COPYRIGHT
   Copyright ©  2019 Translucency Software.  All rights reserved.
#>
$ErrorActionPreference = "Stop"
$Host.UI.RawUI.WindowTitle = "Remotely Setup"
Clear-Host

#region Variables
$InstallPath = ""
$Website = $null
$ServerCmdlets = $false
$FirewallSet = $false
$CopyErrors = $false
if ($PSScriptRoot -eq ""){
    $PSScriptRoot = (Get-Location)
}

$Root = (Get-Item -Path $PSScriptRoot).Parent.FullName
#endregion

#region Functions
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
    Read-Host "Press Enter to exit"
    return
}
### Check Script Root ###
if (!$PSScriptRoot) {
    Wrap-Host
    Wrap-Host "Error: Unable to determine working directory.  Please make sure you're running the full script and not just a section." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    return
}

### Check OS version. ###
$OS = Get-WmiObject -Class Win32_OperatingSystem
if ($OS.Name.ToLower().Contains("home") -or $OS.Caption.ToLower().Contains("home")) {
    Wrap-Host
    Wrap-Host "Error: Windows Home version does not have the necessary features to run Remotely." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    return
}
### Test if Windows Feature cmdlets are available. ###
if ((Get-Command -Name "Add-WindowsFeature" -ErrorAction Ignore) -eq $null) {
    $ServerCmdlets = $false
}
else {
    $ServerCmdlets = $true
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
Read-Host "Press Enter to continue"
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
Wrap-Host "This setup script will install Remotely on this machine.  Please make sure you've already created a website in IIS where Remotely will be installed." -ForegroundColor Green
Wrap-Host
Wrap-Host "If you encounter any problems or have any questions, please contact Translucency_Software@outlook.com." -ForegroundColor Green
Wrap-Host
Read-Host "Press Enter to begin installation"
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
    Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET" -PercentComplete (2/7*100)
    $Result = Add-WindowsFeature Web-Asp-Net
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }
    Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET 4.5" -PercentComplete (3/7*100)
    $Result = Add-WindowsFeature Web-Asp-Net45
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }
    Write-Progress -Activity "IIS Component Installation" -Status "Installing web sockets" -PercentComplete (4/7*100)
    $Result = Add-WindowsFeature Web-WebSockets
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }
    Write-Progress -Activity "IIS Component Installation" -Status "Installing IIS management tools" -PercentComplete (5/7*100)
    $Result = Add-WindowsFeature Web-Mgmt-Tools
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }
    Write-Progress -Activity "IIS Component Installation" -Status "Installing web filtering" -PercentComplete (6/7*100)
    $Result = Add-WindowsFeature Web-Filtering
    if ($Result.RestartNeeded -like "Yes")
    {
        $RebootRequired = $true
    }

    Write-Progress -Activity "IIS Component Installation" -Status "IIS setup completed" -PercentComplete (7/7*100) -Completed
    Start-Sleep 2
}
else
{
    Wrap-Host
    Wrap-Host "Installing IIS components..." -ForegroundColor Green
    Write-Progress -Activity "IIS Component Installation" -Status "Installing web server" -PercentComplete (1/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-WebServer /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET" -PercentComplete (2/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-ASPNET /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "Installing ASP.NET 4.5" -PercentComplete (3/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-ASPNET45 /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "Installing web sockets" -PercentComplete (4/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-WebSockets /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "Installing IIS management tools" -PercentComplete (5/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-ManagementConsole /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "Installing web filtering" -PercentComplete (6/7*100)
    DISM /Online /Enable-Feature /FeatureName:IIS-RequestFiltering /All /Quiet

    Write-Progress -Activity "IIS Component Installation" -Status "IIS setup completed" -PercentComplete (7/7*100) -Completed
    Start-Sleep 2

}

Clear-Host
$Sites = Get-Website
### Site Selection ###
while ($Website -eq $null) {
    Wrap-Host
    $Sites
    Wrap-Host
    Wrap-Host "Enter the ID of the website where Remotely will be installed." -ForegroundColor Green
    Wrap-Host
    $ID = Read-Host "Website ID"

    $Website = Get-Website | Where-Object {$_.ID -like $ID}
}
$InstallPath = $Website.physicalPath.Replace("%SystemDrive%", $env:SystemDrive)

Wrap-Host
Wrap-Host "This will DELETE ALL FILES in the selected website and install Remotely Server.  If this is not your intention, close this window now and create a new website where Remotely Server will be installed." -ForegroundColor Red
Wrap-Host
pause

# Stop site.
Clear-Host
Wrap-Host
Wrap-Host "Stopping website..." -ForegroundColor Green
Stop-Website -Name $($Website.name)


### File Cleanup ###
Wrap-Host
Wrap-Host "Cleaning up existing files..." -ForegroundColor Green
$Success = $false

while ($Success -eq $false) {
    try {
        Get-ChildItem -Path "$InstallPath" -Recurse | Remove-Item -Force -Recurse
        $Success = $true
    }
    catch {
        Start-Sleep -Seconds 1
    }
}



### Download Server Package ###
try {
    if ((Test-Path -Path "$env:TEMP\Server.zip")){
        Remove-Item -Path "$env:TEMP\Server.zip" -Force
    }
    Wrap-Host "Downloading server package..."
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri "https://remotely.lucency.co/Downloads/win-x64/Server.zip" -OutFile "$env:TEMP\Server.zip"
    Wrap-Host "Extracting server files..."
	[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem") | Out-Null
	[System.IO.Compression.ZipFile]::ExtractToDirectory("$env:TEMP\Server.zip", $InstallPath)
}
catch {
    Wrap-Host
    Wrap-Host "Error: Unable to download or extract client files." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    return
}

### Set ACL on website folders and files ###
Wrap-Host
Wrap-Host "Setting ACLs..." -ForegroundColor Green
$Acl = Get-Acl -Path $InstallPath
$Rule = New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\IIS_IUSRS", "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$Acl.AddAccessRule($Rule)
$Acl.SetOwner((New-Object System.Security.Principal.NTAccount("Administrators")))
Set-Acl -Path $InstallPath -AclObject $Acl
Get-ChildItem -Path $InstallPath -Recurse | ForEach-Object {
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
Start-Website -Name $($Website.name)

Wrap-Host
Wrap-Host
Wrap-Host
Wrap-Host "**********************************"
Wrap-Host "      Server setup complete!" -ForegroundColor Green
Wrap-Host "**********************************"
Wrap-Host
Wrap-Host "Website bindings and SSL need to be set up in IIS, if they're not already.  This process needs to be completed manually.  I recommend researching Let's Encrypt for free, automated SSL certificates." -ForegroundColor Green

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
Read-Host "Press Enter to exit..."