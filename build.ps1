# Builds LiveSuite.exe into artifacts\ (a single file: .NET Framework 4.8 is already in Windows 10/11).
# Usage: powershell -ExecutionPolicy Bypass -File build.ps1
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
dotnet build "$root\src\LiveSuite\LiveSuite.csproj" -c Release
if ($LASTEXITCODE -ne 0) { throw "build failed" }
New-Item -ItemType Directory -Force "$root\artifacts" | Out-Null
Copy-Item "$root\src\LiveSuite\bin\Release\net48\LiveSuite.exe" "$root\artifacts\LiveSuite.exe" -Force
Write-Host "-> artifacts\LiveSuite.exe"
