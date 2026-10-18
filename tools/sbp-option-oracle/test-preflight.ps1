#Requires -Version 7.0
$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('vf110-preflight-' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($root) | Out-Null
$unity = Join-Path $root 'not-an-editor.exe'
[IO.File]::WriteAllText($unity,'not executable: every test must reject before process creation')
$package = Join-Path $root 'sbp'
[IO.Directory]::CreateDirectory($package) | Out-Null
[IO.File]::WriteAllText((Join-Path $package 'package.json'),'{"name":"com.unity.scriptablebuildpipeline","version":"1.20.1"}')
function Assert-Rejected([string]$Target,[string]$Message) {
    $output = & pwsh -NoProfile -File (Join-Path $PSScriptRoot 'run.ps1') -UnityPath $unity -SbpPackagePath $package -RunDirectory $Target 2>&1 | Out-String
    if ($LASTEXITCODE -eq 0 -or -not $output.Contains($Message)) { throw "Expected preflight rejection: $Message`n$output" }
}
Assert-Rejected $root 'must not already exist'
Write-Output 'PASS: cannot overwrite an existing run'
$existingProject = Join-Path $root 'existing-project'
[IO.Directory]::CreateDirectory((Join-Path $existingProject 'ProjectSettings')) | Out-Null
[IO.File]::WriteAllText((Join-Path $existingProject 'ProjectSettings/ProjectVersion.txt'),'m_EditorVersion: 2022.3.62f3')
$nested = Join-Path $existingProject 'nested-oracle'
Assert-Rejected $nested 'outside existing Unity projects'
if (Test-Path -LiteralPath $nested) { throw 'Existing-project guard wrote before rejecting.' }
Write-Output 'PASS: cannot nest the generated project inside an existing Unity project'
$insidePackage = Join-Path $package 'nested-oracle'
Assert-Rejected $insidePackage 'must not be inside the SBP source package'
if (Test-Path -LiteralPath $insidePackage) { throw 'Package guard wrote before rejecting.' }
Write-Output 'PASS: cannot mutate the input package with a nested generated project'
Write-Output "Preserved preflight fixtures: $root"
