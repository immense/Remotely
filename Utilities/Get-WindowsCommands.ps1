# License: https://github.com/MicrosoftDocs/windowsserverdocs/blob/master/LICENSE
# Project: https://github.com/MicrosoftDocs/windowsserverdocs/tree/master/WindowsServerDocs/administration/windows-commands

class Parameter{
    [string]$ParameterType
    [string]$Name
    [string]$Summary
}

class ConsoleCommand {
    [string]$Name
    [string]$Summary
    [string]$Syntax
    [System.Collections.ArrayList]$Parameters
}

function Find-Index([string[]]$Content, [string]$Pattern, [int]$StartIndex) {
    for ($i = $StartIndex; $i -lt $Content.Length; $i++) {
        if ($Content[$i].Contains($Pattern)) {
            return $i
        }
    }
}

Set-Location -Path "C:\Users\typic\Downloads\windows-commands\"

$Commands = [System.Collections.ArrayList]::new()

[string[]]$TOC = Get-Content "TOC.md" | Where-Object {
    $_.StartsWith("### [") -and
    $_.Substring(4).Contains(" ") -eq $false
}


foreach ($line in $TOC) {
    $Command = [ConsoleCommand]::new()
    $Command.Parameters = [System.Collections.ArrayList]::new()


    $File = $line.Split("(")[-1].Replace(")", "")
    [string[]]$Content = Get-Content -Path $File
    $TitleIndex = Find-Index -Content $Content -Pattern "#" -StartIndex 0

    $Command.Name = $Content[$TitleIndex].Replace("# ", "")

    for ($i = $TitleIndex + 1; !$Content[$i].StartsWith("## Syntax"); $i++) {
        if (![string]::IsNullOrWhiteSpace($Content[$i]) -and !$Content[$i].StartsWith(">")) {
            $Command.Summary = $Content[$i]  -replace "\*\*","" -replace "&#42;","*"
            break
        }
    }

    $StartSyntax = Find-Index -Content $Content -Pattern "``````" -StartIndex 0
    if ($StartSyntax -ne $null){
        $StopSyntax = Find-Index -Content $Content -Pattern "``````" -StartIndex ($StartSyntax + 1)
        for ($i = $StartSyntax + 1; $i -lt $StopSyntax; $i++) {
            $Command.Syntax += $Content[$i] + "`r`n`r`n"
        }
        $Command.Syntax = $Command.Syntax.Trim()
    }

   

    $StartParams = Find-Index -Content $Content -Pattern "## Parameters" -StartIndex 0
    $StartTableRowSplit = Find-Index -Content $Content -Pattern "|--" -StartIndex 0
    for ($i = $StartTableRowSplit + 1; ![string]::IsNullOrWhiteSpace($Content[$i]); $i++){
        if ([string]::IsNullOrWhiteSpace($Content[$i]) -or $Content[$i].Contains("## ")){
            break
        }
        $NewParam = [Parameter]::new()
        $NewParam.ParameterType = "String"
        $ParamArray = $Content[$i] -split "\|(?<!\\\|)" | Where-Object {![string]::IsNullOrWhiteSpace($_)}
        if ([string]::IsNullOrWhiteSpace($ParamArray[0])){
            continue
        }
        $NewParam.Name = $ParamArray[0] -replace "\\(?!\\)","" -replace "\\{4}","\\" -replace "``","`"" -replace "\*\*","" -replace "&#42;","*"
        if (![string]::IsNullOrWhiteSpace($ParamArray[1])){
            $NewParam.Summary = $ParamArray[1] -replace "\\(?!\\)","" -replace "\\{4}","\\" -replace "``","`"" -replace "\*\*","" -replace "&#42;","*"
        }
        $Command.Parameters.Add($NewParam) | Out-Null
    }
    $Commands.Add($Command) | Out-Null
}

if ((Test-Path -Path "$env:USERPROFILE\Downloads\CMD.txt") -eq $true){
    Remove-Item "$env:USERPROFILE\Downloads\CMD.txt" -Force
}


$Commands | ForEach-Object {
    $Params = ""
    foreach ($param in $_.Parameters) {
         $Params += "new Parameter(``$($param.Name)``, ``$($param.Summary)``, ````),`r`n"
    }
@"
    new ConsoleCommand(
        ``$($_.Name -replace "\\{4}","\\" -replace "``","`"")``,
        [
            $Params
        ],
        ``$($_.Summary -replace "\\{4}","\\" -replace "``","`"")``,
        ``$($_.Syntax -replace "\\{4}","\\" -replace "``","`"")``,
        "",
        () => { }
    ),
"@ | Out-File -FilePath "$env:USERPROFILE\Downloads\CMD.txt" -Append
}
