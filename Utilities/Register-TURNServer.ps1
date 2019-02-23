$Trigger = New-JobTrigger -AtStartup -RandomDelay 00:00:30

Register-ScheduledJob -Trigger $Trigger -Name "TURNServer" -RunNow -ScriptBlock {
    $env:USERS = "user=password"
    $env:REALM = "myserver.example.com"
    $env:UDP_PORT = 3478
    &"C:\path\to\simple-turn-windows-amd64.exe"
}