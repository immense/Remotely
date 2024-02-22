<#
.SYNOPSIS
   Publishes the Remotely clients.
.DESCRIPTION
   Publishes the Remotely clients.
   To deploy the server, supply the following arguments: -rid win-x64 -outdir path\to\dir -hostname https://mysite.mydomain.com
.COPYRIGHT
   Copyright 2023 Immense Networks.  All rights reserved.
.EXAMPLE
   Run it from the Utilities folder (located in the solution directory).
   Or run "powershell -f Publish.ps1 -rid win-x64 -outdir path\to\dir -hostname https://mysite.mydomain.com
#>

param (
	[string]$OutDir = "",
    # RIDs are described here: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
	[string]$RID = "",
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

    $CurrentVersion = $VersionDate.ToString("yyyy.MM.dd.HHmm")

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

function Wait-ForExists($FilePath) {
    while ((Test-Path -Path $FilePath) -eq $false){
        Write-Host "Waiting for file: $FilePath"
        Start-Sleep -Seconds 3
    }
}
#endregion

if ([string]::IsNullOrWhiteSpace($MSBuildPath) -or !(Test-Path -Path $MSBuildPath)) {
    Write-Host
    Write-Host "ERROR: Unable to find the path to MSBuild.exe." -ForegroundColor Red
    Write-Host
    pause
    return
}

    
# Clear publish folders.
if ((Test-Path -Path "$Root\Agent\bin\publish\win-x64") -eq $true) {
	Get-ChildItem -Path "$Root\Agent\bin\publish\win-x64" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path  "$Root\Agent\bin\publish\win-x86" ) -eq $true) {
	Get-ChildItem -Path  "$Root\Agent\bin\publish\win-x86" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path "$Root\Agent\bin\publish\linux-x64") -eq $true) {
	Get-ChildItem -Path "$Root\Agent\bin\publish\linux-x64" | Remove-Item -Force -Recurse
}


# Publish agents.
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win-x64 --self-contained --configuration Release --output "$Root\Agent\bin\publish\win-x64" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64 --self-contained --configuration Release --output "$Root\Agent\bin\publish\linux-x64" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win-x86 --self-contained --configuration Release --output "$Root\Agent\bin\publish\win-x86" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime osx-x64 --self-contained --configuration Release --output "$Root\Agent\bin\publish\osx-x64" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime osx-arm64 --self-contained --configuration Release --output "$Root\Agent\bin\publish\osx-arm64" "$Root\Agent"

New-Item -Path "$Root\Agent\bin\publish\win-x64\Desktop\" -ItemType Directory -Force
New-Item -Path "$Root\Agent\bin\publish\win-x86\Desktop\" -ItemType Directory -Force
New-Item -Path "$Root\Agent\bin\publish\linux-x64\Desktop\" -ItemType Directory -Force


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

if ($SignAssemblies) {
    &"$Root\Utilities\signtool.exe" sign /fd SHA256 /f "$CertificatePath" /p $CertificatePassword /t http://timestamp.digicert.com "$Root\Server\wwwroot\Content\Win-x64\Remotely_Desktop.exe"
}


# Publish Windows GUI App (32-bit)
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion -p:PublishProfile=desktop-win-x86 --configuration Release "$Root\Desktop.Win"

if ($SignAssemblies) {
    &"$Root\Utilities\signtool.exe" sign /fd SHA256 /f "$CertificatePath" /p $CertificatePassword /t http://timestamp.digicert.com "$Root\Server\wwwroot\Content\Win-x86\Remotely_Desktop.exe"
}

# Build installer.
&"$MSBuildPath" "$Root\Agent.Installer.Win" /t:Restore 
&"$MSBuildPath" "$Root\Agent.Installer.Win" /t:Build /p:Configuration=Release /p:Platform=AnyCPU /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion
Copy-Item -Path "$Root\Agent.Installer.Win\bin\Release\Remotely_Installer.exe" -Destination "$Root\Server\wwwroot\Content\Remotely_Installer.exe" -Force

if ($SignAssemblies) {
    &"$Root\Utilities\signtool.exe" sign /fd SHA256 /f "$CertificatePath" /p $CertificatePassword /t http://timestamp.digicert.com "$Root\Server\wwwroot\Content\Remotely_Installer.exe"
}

# Compress Agents.
$PublishDir =  "$Root\Agent\bin\publish\win-x64"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win-x64.zip" -Force
Wait-ForExists -FilePath "$PublishDir\Remotely-Win-x64.zip"
Move-Item -Path "$PublishDir\Remotely-Win-x64.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-Win-x64.zip" -Force

$PublishDir =  "$Root\Agent\bin\publish\win-x86"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win-x86.zip" -Force
Wait-ForExists -FilePath "$PublishDir\Remotely-Win-x86.zip"
Move-Item -Path "$PublishDir\Remotely-Win-x86.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-Win-x86.zip" -Force

$PublishDir =  "$Root\Agent\bin\publish\linux-x64"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Linux.zip" -Force
Wait-ForExists -FilePath "$PublishDir\Remotely-Linux.zip"
Move-Item -Path "$PublishDir\Remotely-Linux.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-Linux.zip" -Force

$PublishDir =  "$Root\Agent\bin\publish\osx-x64"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-MacOS-x64.zip" -Force
Wait-ForExists -FilePath "$PublishDir\Remotely-MacOS-x64.zip"
Move-Item -Path "$PublishDir\Remotely-MacOS-x64.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-MacOS-x64.zip" -Force

$PublishDir =  "$Root\Agent\bin\publish\osx-arm64"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-MacOS-arm64.zip" -Force
Wait-ForExists -FilePath "$PublishDir\Remotely-MacOS-arm64.zip"
Move-Item -Path "$PublishDir\Remotely-MacOS-arm64.zip" -Destination "$Root\Server\wwwroot\Content\Remotely-MacOS-arm64.zip" -Force


if ($RID.Length -gt 0 -and $OutDir.Length -gt 0) {
    if ((Test-Path -Path $OutDir) -eq $false){
        New-Item -Path $OutDir -ItemType Directory
    }

    dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime $RID --self-contained --configuration Release --output $OutDir "$Root\Server\"
}
else {
    Write-Host "`nSkipping server deployment.  Params -outdir and -rid not specified." -ForegroundColor DarkYellow
}