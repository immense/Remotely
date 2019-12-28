$Global:Commands = ""
function Append-Command($Command){
    $Params = ""
    if ($Command.HelpFile.Length -gt 0) {
        $Help = Get-Help $Command.Name
        $Synopsis = $Help.Synopsis.Trim().Replace("\", "\\").Replace("``", "")
        $Syntax = ($Help.syntax | Out-String).Trim().Replace("``", "")
        foreach ($param in $Help.parameters.parameter) {
            $Params += "new Parameter(``$(($param.name | Out-String).Trim())``, ``$(($param.description | Out-String).Trim().Replace('`', '"').Replace('${}',''))``, ``$($param.type.name.Trim().Replace('`', '^'))``),`r`n"
        }
    }
    else {
        $Synopsis = "See help file for details."
        $Syntax = "";
        foreach ($param in $_.Parameters.Keys) {
            $Params += "new Parameter(``$($param)``, ``See help file for details.``, ``$($_.Parameters[$param].ParameterType.ToString().Trim().Replace('`', '^'))``),`r`n"
        }
    }
  

$Global:Commands += @"
    new ConsoleCommand(
        ``$($Command.Name.Replace("\", "\\"))``,
        [
            $($Params.Trim())
        ],
        ``$Synopsis``,
        ``$Syntax``,
        "",
        (parameters, paramDictionary) => {
           
        }
    ),`r`n
"@
}

Update-Help
if ((Test-Path -Path "$env:USERPROFILE\Downloads\PS.txt") -eq $true) {
	Remove-Item -Path "$env:USERPROFILE\Downloads\PS.txt" -Force
}


$Cmdlets = Get-Command -All | Where-Object { 
    $_.CommandType -like "Function" -or
    $_.CommandType -like "Cmdlet" -and
    $_.Source -notlike ""
}
$Cmdlets | ForEach-Object {
    Append-Command  -Command $_
    $Error.Clear()
}

$Global:Commands = $Global:Commands -replace  "([^\\])\\([\w\d])", '$1\\$2'

$Global:Commands | Out-File -FilePath "$env:USERPROFILE\Downloads\PS.txt"