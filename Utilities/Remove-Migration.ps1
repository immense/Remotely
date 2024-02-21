$ErrorActionPreference = "Stop"

if ($PSScriptRoot) {
    $Root = (Get-Item -Path $PSScriptRoot).Parent.FullName + "\Server"
}
else {
    $Root = (Get-Item -Path (Get-Location).Path).Parent.FullName + "\Server"
}

Push-Location "$Root"

dotnet ef migrations remove --context "SqliteDbContext"
dotnet ef migrations remove --context "SqlServerDbContext"
dotnet ef migrations remove --context "PostgreSqlDbContext"

Pop-Location