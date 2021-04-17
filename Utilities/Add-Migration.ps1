param(
    [string]$MigrationName
)

if (!$MigrationName) {
    exit 1
}

$ErrorActionPreference = "Stop"


function Replace-LineInFile($FilePath, $MatchPattern, $ReplaceLineWith, $MaxCount = -1){
    $FullPath = (Get-Item -Path $FilePath).FullName
    [string[]]$Content = Get-Content -Path $FullPath
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
    [System.IO.File]::WriteAllLines($FullPath, $Content)
}


if ($PSScriptRoot) {
    $Root = (Get-Item -Path $PSScriptRoot).Parent.FullName + "\Server"
}
else {
    $Root = (Get-Item -Path (Get-Location).Path).Parent.FullName + "\Server"
}

Push-Location "$Root"

Replace-LineInFile -FilePath "$Root\appsettings.json" -MatchPattern '"DBProvider":' -ReplaceLineWith '    "DBProvider": "SQLite",' -MaxCount 1

dotnet ef migrations add $MigrationName --context "SqliteDbContext"

Replace-LineInFile -FilePath "$Root\appsettings.json" -MatchPattern '"DBProvider":' -ReplaceLineWith '    "DBProvider": "SQLServer",' -MaxCount 1

dotnet ef migrations add $MigrationName --context "SqlServerDbContext"

Replace-LineInFile -FilePath "$Root\appsettings.json" -MatchPattern '"DBProvider":' -ReplaceLineWith '    "DBProvider": "PostgreSQL",' -MaxCount 1

dotnet ef migrations add $MigrationName --context "PostgreSqlDbContext"

Replace-LineInFile -FilePath "$Root\appsettings.json" -MatchPattern '"DBProvider":' -ReplaceLineWith '    "DBProvider": "SQLite",' -MaxCount 1

Pop-Location