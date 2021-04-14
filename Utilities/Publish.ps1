<#
.SYNOPSIS
   Publishes the Remotely clients.
.DESCRIPTION
   Publishes the Remotely clients.
   To deploy the server, supply the following arguments: -rid win10-x64 -outdir path\to\dir -hostname https://mysite.mydomain.com
.COPYRIGHT
   Copyright 2020 Translucency Software.  All rights reserved.
.EXAMPLE
   Run it from the Utilities folder (located in the solution directory).
   Or run "powershell -f Publish.ps1 -rid win10-x64 -outdir path\to\dir -hostname https://mysite.mydomain.com
#>

param (
	[string]$OutDir = "",
    # RIDs are described here: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
	[string]$RID = "",
	[string]$Hostname = "",
	[string]$CertificatePath = "",
    [string]$CertificatePassword = "",
    [string]$CurrentVersion = ""
)



$ErrorActionPreference = "Stop"
$InstallerDir = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer"
$VsWhere = "$InstallerDir\vswhere.exe"
$MSBuildPath = (&"$VsWhere" -latest -products * -find "\MSBuild\Current\Bin\MSBuild.exe").Trim()
$Root = (Get-Item -Path $PSScriptRoot).Parent.FullName
$SignAssemblies = $false

if (!$CurrentVersion) {
    Push-Location -Path $Root

    $VersionString = git show -s --format=%ci
    $VersionDate = [DateTimeOffset]::Parse($VersionString)

    $Year = $VersionDate.Year.ToString()
    $Month = $VersionDate.Month.ToString().PadLeft(2, "0")
    $Day = $VersionDate.Day.ToString().PadLeft(2, "0")
    $Hour = $VersionDate.Hour.ToString().PadLeft(2, "0")
    $Minute = $VersionDate.Minute.ToString().PadLeft(2, "0")
    $CurrentVersion = "$Year.$Month.$Day.$Hour$Minute"

    Pop-Location
}

if ($CertificatePath.Length -gt 0 -and 
    (Test-Path -Path $CertificatePath) -eq $true -and 
    $CertificatePassword.Length -gt 0) 
{
    $SignAssemblies = $true
}



Set-Location -Path $Root

#region Functions

function Replace-LineInFile($FilePath, $MatchPattern, $ReplaceLineWith, $MaxCount = -1){
    [string[]]$Content = Get-Content -Path $FilePath
    $Count = 0
    for ($i = 0; $i -lt $Content.Length; $i++)
    {
        if ($Content[$i] -ne $null -and $Content[$i].Contains($MatchPattern)) {
            $Content[$i] = $ReplaceLineWith
            $Count++
        }
        if ($MaxCount -gt 0 -and $Count -ge $MaxCount) {
            break
        }
    }
    ($Content | Out-String).Trim() | Out-File -FilePath $FilePath -Force -Encoding utf8
}

#endregion

if ([string]::IsNullOrWhiteSpace($MSBuildPath) -or !(Test-Path -Path $MSBuildPath)) {
    Write-Host
    Write-Host "ERROR: Unable to find the path to MSBuild.exe." -ForegroundColor Red
    Write-Host
    pause
    return
}


# Update hostname.
if ($Hostname) {
    [Uri]$HostNameUri = $null

    if (![System.Uri]::TryCreate($HostName, [System.UriKind]::Absolute, [ref] $HostNameUri) -or 
        ($HostNameUri.Scheme -notlike [System.Uri]::UriSchemeHttp -and $HostNameUri.Scheme -notlike [System.Uri]::UriSchemeHttps)) {
            Write-Error "`nThe HostName variable is not a valid HTTP Uri."
            return
    }

    Replace-LineInFile -FilePath "$Root\Shared\Models\DesktopAppConfig.cs" -MatchPattern "private string _host" -ReplaceLineWith "private string _host = `"$($Hostname)`";" -MaxCount 1
    Replace-LineInFile -FilePath "$Root\Desktop.Win\Properties\PublishProfiles\ClickOnce-x64.pubxml" -MatchPattern "<InstallUrl>" -ReplaceLineWith "    <InstallUrl>$Hostname/Content/Win-x64/ClickOnce/</InstallUrl>"
    Replace-LineInFile -FilePath "$Root\Desktop.Win\Properties\PublishProfiles\ClickOnce-x86.pubxml" -MatchPattern "<InstallUrl>" -ReplaceLineWith "    <InstallUrl>$Hostname/Content/Win-x86/ClickOnce/</InstallUrl>"
}
else {
    Write-Warning "`nNo hostname parameter was specified.  The server name will need to be entered manually in the desktop client.`n"
}

    
# Clear publish folders.
if ((Test-Path -Path "$Root\Agent\bin\Release\net5.0\win10-x64\publish") -eq $true) {
	Get-ChildItem -Path "$Root\Agent\bin\Release\net5.0\win10-x64\publish" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path  "$Root\Agent\bin\Release\net5.0\win10-x86\publish" ) -eq $true) {
	Get-ChildItem -Path  "$Root\Agent\bin\Release\net5.0\win10-x86\publish" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path "$Root\Agent\bin\Release\net5.0\linux-x64\publish") -eq $true) {
	Get-ChildItem -Path "$Root\Agent\bin\Release\net5.0\linux-x64\publish" | Remove-Item -Force -Recurse
}


# Publish Core clients.
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win10-x64 --configuration Release --output "$Root\Agent\bin\Release\net5.0\win10-x64\publish" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64 --configuration Release --output "$Root\Agent\bin\Release\net5.0\linux-x64\publish" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win10-x86 --configuration Release --output "$Root\Agent\bin\Release\net5.0\win10-x86\publish" "$Root\Agent"

New-Item -Path "$Root\Agent\bin\Release\net5.0\win10-x64\publish\Desktop\" -ItemType Directory -Force
New-Item -Path "$Root\Agent\bin\Release\net5.0\win10-x86\publish\Desktop\" -ItemType Directory -Force
New-Item -Path "$Root\Agent\bin\Release\net5.0\linux-x64\publish\Desktop\" -ItemType Directory -Force


# Publish Linux ScreenCaster
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=packaged-linux-x64 --configuration Release "$Root\Desktop.Linux\"

# Publish Linux GUI App
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=desktop-linux-x64 --configuration Release "$Root\Desktop.Linux\"


# Publish Windows ScreenCaster (32-bit)
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=packaged-win-x86 --configuration Release "$Root\Desktop.Win"

# Publish Windows ScreenCaster (64-bit)
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=packaged-win-x64 --configuration Release "$Root\Desktop.Win"


# Publish Windows GUI App (64-bit)
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=desktop-win-x64 --configuration Release "$Root\Desktop.Win"
&"$MSBuildPath" "$Root\Desktop.Win" -t:Restore -t:Publish -p:PublishProfile="ClickOnce-x64.pubxml" -p:Configuration=Release -p:Platform=x64 -p:ApplicationVersion=$CurrentVersion -p:Version=$CurrentVersion -p:FileVersion=$CurrentVersion -p:PublishDir="$Root\Server\wwwroot\Content\Win-x64\ClickOnce\"

if ($SignAssemblies) {
    &"$Root\Utilities\signtool.exe" sign /f "$CertificatePath" /p $CertificatePassword /t http://timestamp.digicert.com "$Root\Server\wwwroot\Content\Win-x64\Remotely_Desktop.exe"
}


# Publish Windows GUI App (32-bit)
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=desktop-win-x86 --configuration Release "$Root\Desktop.Win"
&"$MSBuildPath" "$Root\Desktop.Win" -t:Restore -t:Publish -p:PublishProfile="ClickOnce-x86.pubxml" -p:Configuration=Release -p:Platform=x86 -p:ApplicationVersion=$CurrentVersion -p:Version=$CurrentVersion -p:FileVersion=$CurrentVersion -p:PublishDir="$Root\Server\wwwroot\Content\Win-x86\ClickOnce\"

if ($SignAssemblies) {
    &"$Root\Utilities\signtool.exe" sign /f "$CertificatePath" /p $CertificatePassword /t http://timestamp.digicert.com "$Root\Server\wwwroot\Content\Win-x86\Remotely_Desktop.exe"
}

# Build installer.
&"$MSBuildPath" "$Root\Agent.Installer.Win" /t:Restore 
&"$MSBuildPath" "$Root\Agent.Installer.Win" /t:Build /p:Configuration=Release /p:Platform=AnyCPU /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion
Copy-Item -Path "$Root\Agent.Installer.Win\bin\Release\Remotely_Installer.exe" -Destination "$Root\Server\wwwroot\Content\Remotely_Installer.exe" -Force
if ($SignAssemblies) {
    &"$Root\Utilities\signtool.exe" sign /f "$CertificatePath" /p $CertificatePassword /t http://timestamp.digicert.com "$Root\Server\wwwroot\Content\Remotely_Installer.exe"
}

# Compress Core clients.
$PublishDir =  "$Root\Agent\bin\Release\net5.0\win10-x64\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win10-x64.zip" -Force
while ((Test-Path -Path "$PublishDir\Remotely-Win10-x64.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Win10-x64.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-Win10-x64.zip" -Force

$PublishDir =  "$Root\Agent\bin\Release\net5.0\win10-x86\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win10-x86.zip" -Force
while ((Test-Path -Path "$PublishDir\Remotely-Win10-x86.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Win10-x86.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-Win10-x86.zip" -Force

$PublishDir =  "$Root\Agent\bin\Release\net5.0\linux-x64\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Linux.zip" -Force
while ((Test-Path -Path "$PublishDir\Remotely-Linux.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Linux.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-Linux.zip" -Force



if ($RID.Length -gt 0 -and $OutDir.Length -gt 0) {
    if ((Test-Path -Path $OutDir) -eq $false){
        New-Item -Path $OutDir -ItemType Directory
    }

    dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime $RID --configuration Release --output $OutDir "$Root\Server\"
}
else {
    Write-Host "`nSkipping server deployment.  Params -outdir and -rid not specified." -ForegroundColor DarkYellow
}