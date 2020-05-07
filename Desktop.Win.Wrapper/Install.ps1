[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12 -bor [System.Net.SecurityProtocolType]::Tls13
[System.IO.Directory]::CreateDirectory("$env:AppData\Remotely")
Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile "$env:AppData\Remotely\dotnet-install.ps1" -UseBasicParsing
&"$env:AppData\Remotely\dotnet-install.ps1" -Runtime dotnet
&"$env:AppData\Remotely\dotnet-install.ps1" -Runtime windowsdesktop
Start-Process -FilePath "dotnet.exe" -ArgumentList "$PSScriptRoot\Remotely_Desktop.dll" -WindowStyle Hidden