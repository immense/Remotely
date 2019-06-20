<#
.SYNOPSIS
   Publishes the Remotely clients.
.DESCRIPTION
   Publishes the Remotely clients.
   To deploy the server, supply the following arguments: -rid win10-x64 -outdir path\to\dir
.COPYRIGHT
   Copyright ©  2019 Translucency Software.  All rights reserved.
.EXAMPLE
   Run it from the Utilities folder (located in the solution directory).
   Or run "powershell -f PublishClients.ps1 -rid win10-x64 -outdir path\to\dir
#>
$ErrorActionPreference = "Stop"
$Year = (Get-Date).Year.ToString()
$Month = (Get-Date).Month.ToString().PadLeft(2, "0")
$Day = (Get-Date).Day.ToString().PadLeft(2, "0")
$Hour = (Get-Date).Hour.ToString().PadLeft(2, "0")
$Minute = (Get-Date).Minute.ToString().PadLeft(2, "0")
$CurrentVersion = "$Year.$Month.$Day.$Hour$Minute"
$OutDir = ""
# RIDs are described here: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
$RID = ""
$MSBuildPath = (Get-ChildItem -Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\" -Recurse -Filter "MSBuild.exe" -File)[0].FullName
$DevEnv = (Get-ChildItem -Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\" -Recurse -Filter "devenv.com" -File)[0].FullName

if (!(Test-Path -Path $MSBuildPath)) {
    Write-Host "ERROR: Unable to find the path to MSBuild.exe." -ForegroundColor Red
    pause
    return
}

for ($i = 0; $i -lt $args.Count; $i++)
{ 
    $arg = $args[$i].ToString().ToLower()
    if ($arg.Contains("outdir")){
        $OutDir = $args[$i+1]
    }
    elseif ($arg.Contains("rid")){
        $RID = $args[$i+1]
    }
}

$Root = (Get-Item -Path $PSScriptRoot).Parent.FullName

Set-Location -Path $Root

# Add Current Version file to root content folder for client update checks.
Set-Content -Path ".\Server\CurrentVersion.txt" -Value $CurrentVersion.Trim() -Encoding UTF8 -Force

    
# Clear publish folders.
if ((Test-Path -Path ".\Agent\bin\Release\netcoreapp2.2\win10-x64\publish") -eq $true) {
	Get-ChildItem -Path ".\Agent\bin\Release\netcoreapp2.2\win10-x64\publish" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path  ".\Agent\bin\Release\netcoreapp2.2\win10-x86\publish" ) -eq $true) {
	Get-ChildItem -Path  ".\Agent\bin\Release\netcoreapp2.2\win10-x86\publish" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path ".\Agent\bin\Release\netcoreapp2.2\linux-x64\publish") -eq $true) {
	Get-ChildItem -Path ".\Agent\bin\Release\netcoreapp2.2\linux-x64\publish" | Remove-Item -Force -Recurse
}


# Publish Core clients.
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win10-x64 --configuration Release --output "$Root\Agent\bin\Release\netcoreapp2.2\win10-x64\publish" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win10-x86 --configuration Release --output "$Root\Agent\bin\Release\netcoreapp2.2\win10-x86\publish" "$Root\Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64 --configuration Release --output "$Root\Agent\bin\Release\netcoreapp2.2\linux-x64\publish" "$Root\Agent"

New-Item -Path ".\Agent\bin\Release\netcoreapp2.2\win10-x64\publish\ScreenCast\" -ItemType Directory -Force
New-Item -Path ".\Agent\bin\Release\netcoreapp2.2\win10-x86\publish\ScreenCast\" -ItemType Directory -Force
New-Item -Path ".\Agent\bin\Release\netcoreapp2.2\linux-x64\publish\ScreenCast\" -ItemType Directory -Force


# Publish Linux ScreenCaster
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64  --configuration Release --output "$Root\Agent\bin\Release\netcoreapp2.2\linux-x64\publish\ScreenCast\" "$Root\ScreenCast.Linux\"

# Publish Linux GUI App
$PublishDir = "$Root\Desktop.Unix\bin\Release\netcoreapp2.2\linux-x64\publish\"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64  --configuration Release --output "$PublishDir" "$Root\Desktop.Unix\"
# Compress Linux GUI App
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely_Desktop.Unix.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely_Desktop.Unix.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely_Desktop.Unix.zip" -Destination "$Root\Server\wwwroot\Downloads\Remotely_Desktop.Unix.zip" -Force

# Build .NET Framework Projects
&"$MSBuildPath" "$Root\ScreenCast.Win" /t:Build /p:Configuration=Release
&"$MSBuildPath" "$Root\Desktop.Win" /t:Build /p:Configuration=Release


# Copy .NET Framework ScreenCaster to Agent output folder.
Get-ChildItem -Path ".\ScreenCast.Win\bin\Release\" -Exclude "*.xml" | Copy-Item -Destination ".\Agent\bin\Release\netcoreapp2.2\win10-x64\publish\ScreenCast\" -Force
Get-ChildItem -Path ".\ScreenCast.Win\bin\Release\" -Exclude "*.xml" | Copy-Item -Destination ".\Agent\bin\Release\netcoreapp2.2\win10-x86\publish\ScreenCast\" -Force



# Compress Core clients.
$PublishDir =  "$Root\Agent\bin\Release\netcoreapp2.2\win10-x64\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win10-x64.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely-Win10-x64.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Win10-x64.zip" -Destination "$Root\Server\wwwroot\Downloads\Remotely-Win10-x64.zip" -Force

$PublishDir =  "$Root\Agent\bin\Release\netcoreapp2.2\win10-x86\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win10-x86.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely-Win10-x86.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Win10-x86.zip" -Destination "$Root\Server\wwwroot\Downloads\Remotely-Win10-x86.zip" -Force

$PublishDir =  "$Root\Agent\bin\Release\netcoreapp2.2\linux-x64\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Linux.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely-Linux.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Linux.zip" -Destination "$Root\Server\wwwroot\Downloads\Remotely-Linux.zip" -Force

# Copy desktop app to Downloads folder.
&"$DevEnv" "$Root\Desktop.Win.Installer\Desktop.Win.Installer.vdproj" /build "Release|x86"
Copy-Item -Path ".\Desktop.Win.Installer\Release\Remotely_Desktop_Installer.msi" -Destination ".\Server\wwwroot\Downloads\Remotely_Desktop_Installer.msi" -Force


if ($RID.Length -gt 0 -and $OutDir.Length -gt 0) {
    if ((Test-Path -Path $OutDir) -eq $false){
        New-Item -Path $OutDir -ItemType Directory
    }
    dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime $RID --configuration Release --output $OutDir "$Root\Server\"
}
else {
    Write-Host "`r`nSkipping server deployment.  Params -outdir and -rid not specified." -ForegroundColor DarkYellow
}