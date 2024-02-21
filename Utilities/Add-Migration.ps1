param(
    [string]$MigrationName
)

if (!$MigrationName) {
    exit 1
}

$ErrorActionPreference = "Stop"


if ($PSScriptRoot) {
    $Root = (Get-Item -Path $PSScriptRoot).Parent.FullName + "\Server"
}
else {
    $Root = (Get-Item -Path (Get-Location).Path).Parent.FullName + "\Server"
}

Push-Location $Root

dotnet ef migrations add $MigrationName --context "SqliteDbContext"
dotnet ef migrations add $MigrationName --context "SqlServerDbContext"
dotnet ef migrations add $MigrationName --context "PostgreSqlDbContext"

Pop-Location