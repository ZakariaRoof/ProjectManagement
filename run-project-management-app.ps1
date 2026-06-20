$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "ProjectManagementApp.csproj"

dotnet run --project $projectPath --launch-profile https
