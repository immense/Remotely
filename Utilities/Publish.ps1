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
Set-Content -Path ".\Remotely_Server\CurrentVersion.txt" -Value $CurrentVersion.Trim() -Encoding UTF8 -Force

    
# Clear publish folders.
if ((Test-Path -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish") -eq $true) {
	Get-ChildItem -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path  ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish" ) -eq $true) {
	Get-ChildItem -Path  ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish" | Remove-Item -Force -Recurse
}
if ((Test-Path -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\linux-x64\publish") -eq $true) {
	Get-ChildItem -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\linux-x64\publish" | Remove-Item -Force -Recurse
}


# Publish Core clients.
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win10-x64 --configuration Release --output "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish" "$Root\Remotely_Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime win10-x86 --configuration Release --output "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish" "$Root\Remotely_Agent"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64 --configuration Release --output "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\linux-x64\publish" "$Root\Remotely_Agent"

New-Item -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish\ScreenCast\" -ItemType Directory -Force
New-Item -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish\ScreenCast\" -ItemType Directory -Force
New-Item -Path ".\Remotely_Agent\bin\Release\netcoreapp2.2\linux-x64\publish\ScreenCast\" -ItemType Directory -Force


# Publish Linux ScreenCaster
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64  --configuration Release --output "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\linux-x64\publish\ScreenCast\" "$Root\Remotely_ScreenCast.Linux\"

# Publish Linux GUI App
$PublishDir = "$Root\Remotely_Desktop.Unix\bin\Release\netcoreapp2.2\linux-x64\publish\"
dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime linux-x64  --configuration Release --output "$PublishDir" "$Root\Remotely_Desktop.Unix\"
# Compress Linux GUI App
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely_Desktop.Unix.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely_Desktop.Unix.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely_Desktop.Unix.zip" -Destination "$Root\Remotely_Server\wwwroot\Downloads\Remotely_Desktop.Unix.zip" -Force


# Copy .NET Framework ScreenCaster to Agent output folder.
if ((Test-Path -Path ".\Remotely_ScreenCast.Win\bin\Release\Remotely_ScreenCast.exe") -eq $true) {
    Copy-Item -Path ".\Remotely_ScreenCast.Win\bin\Release\Remotely_ScreenCast.exe" -Destination ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish\ScreenCast\Remotely_ScreenCast.exe" -Force
    Copy-Item -Path ".\Remotely_ScreenCast.Win\bin\Release\Remotely_ScreenCast.exe" -Destination ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish\ScreenCast\Remotely_ScreenCast.exe" -Force
}
elseif ((Test-Path -Path ".\Remotely_ScreenCast.Win\bin\Debug\Remotely_ScreenCast.exe") -eq $true) {
    Copy-Item -Path ".\Remotely_ScreenCast.Win\bin\Debug\Remotely_ScreenCast.exe" -Destination ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish\ScreenCast\Remotely_ScreenCast.exe" -Force
    Copy-Item -Path ".\Remotely_ScreenCast.Win\bin\Debug\Remotely_ScreenCast.exe" -Destination ".\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish\ScreenCast\Remotely_ScreenCast.exe" -Force
}



# Compress Core clients.
$PublishDir =  "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x64\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win10-x64.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely-Win10-x64.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Win10-x64.zip" -Destination "$Root\Remotely_Server\wwwroot\Downloads\Remotely-Win10-x64.zip" -Force

$PublishDir =  "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\win10-x86\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Win10-x86.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely-Win10-x86.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Win10-x86.zip" -Destination "$Root\Remotely_Server\wwwroot\Downloads\Remotely-Win10-x86.zip" -Force

$PublishDir =  "$Root\Remotely_Agent\bin\Release\netcoreapp2.2\linux-x64\publish"
Compress-Archive -Path "$PublishDir\*" -DestinationPath "$PublishDir\Remotely-Linux.zip" -CompressionLevel Optimal -Force
while ((Test-Path -Path "$PublishDir\Remotely-Linux.zip") -eq $false){
    Start-Sleep -Seconds 1
}
Move-Item -Path "$PublishDir\Remotely-Linux.zip" -Destination "$Root\Remotely_Server\wwwroot\Downloads\Remotely-Linux.zip" -Force

# Copy desktop app to Downloads folder.
if ((Test-Path -Path ".\Remotely_Desktop.Win\bin\Release\Remotely_Desktop.exe") -eq $true) {
    Copy-Item -Path ".\Remotely_Desktop.Win\bin\Release\Remotely_Desktop.exe" -Destination ".\Remotely_Server\wwwroot\Downloads\Remotely_Desktop.exe" -Force
}
elseif ((Test-Path -Path ".\Remotely_Desktop.Win\bin\Debug\Remotely_Desktop.exe") -eq $true) {
    Copy-Item -Path ".\Remotely_Desktop.Win\bin\Debug\Remotely_Desktop.exe" -Destination ".\Remotely_Server\wwwroot\Downloads\Remotely_Desktop.exe" -Force
}

if ($RID.Length -gt 0 -and $OutDir.Length -gt 0) {
    if ((Test-Path -Path $OutDir) -eq $false){
        New-Item -Path $OutDir -ItemType Directory
    }
    dotnet publish /p:Version=$CurrentVersion /p:FileVersion=$CurrentVersion --runtime $RID --configuration Release --output $OutDir "$Root\Remotely_Server\"
}
else {
    Write-Host "`r`nSkipping server deployment.  Params -outdir and -rid not specified." -ForegroundColor DarkYellow
}