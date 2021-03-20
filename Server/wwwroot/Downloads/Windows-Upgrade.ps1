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

 
### Site Selection ###
Wrap-Host
Get-Website
Wrap-Host
Wrap-Host "Enter the ID of the website where Remotely is installed." -ForegroundColor Green
Wrap-Host
$ID = Read-Host "Website ID"

$Website = Get-Website | Where-Object {$_.ID -like $ID}
$SiteName = $Website.name


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
        Get-ChildItem -Path "$SitePath" -Recurse | Where-Object {
            $_.Extension -inotlike ".db" -and $_.Name -inotlike "*appsettings*"
        } | Remove-Item -Force
        $Success = $true
    }
    catch {
        Start-Sleep -Seconds 1
    }
}



### Download Server Package ###
try {
    if ((Test-Path -Path "$env:TEMP\Remotely_Server_Win-x64.zip")){
        Remove-Item -Path "$env:TEMP\Remotely_Server_Win-x64.zip" -Force
    }
    Wrap-Host "Downloading server package..."
	$ProgressPreference = "SilentlyContinue"
    Invoke-WebRequest -Uri "https://github.com/lucent-sea/Remotely/releases/latest/download/Remotely_Server_Win-x64.zip" -OutFile "$env:TEMP\Remotely_Server_Win-x64.zip"
    $ProgressPreference = "Continue"
    Wrap-Host "Extracting server files..."
	Expand-Archive -Path "$env:TEMP\Remotely_Server_Win-x64.zip" -DestinationPath $SitePath -Force
}
catch {
    Wrap-Host
    Wrap-Host "Error: Unable to download or extract client files." -ForegroundColor Red
    Do-Pause
    return
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


# Start website.
Start-Website -Name $SiteName

Wrap-Host
Wrap-Host
Wrap-Host "**********************************"
Wrap-Host "      Server upgrade complete!" -ForegroundColor Green
Wrap-Host "**********************************"
Wrap-Host