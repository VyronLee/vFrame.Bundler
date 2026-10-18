#Requires -Version 7.0
param(
    [Parameter(Mandatory)][string]$SbpPackagePath,
    [string]$UnityPath = 'C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe',
    [string]$RunDirectory = (Join-Path ([IO.Path]::GetTempPath()) ('vf110-' + [guid]::NewGuid().ToString('N').Substring(0,8)))
)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'report.ps1')
if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) { throw 'Unity editor not found; no installation is performed.' }
$SbpPackagePath = (Resolve-Path -LiteralPath $SbpPackagePath).Path
$package = ConvertFrom-Json -InputObject ([IO.File]::ReadAllText((Join-Path $SbpPackagePath 'package.json')))
if ($package.name -ne 'com.unity.scriptablebuildpipeline' -or $package.version -ne '1.20.1') {
    throw 'An extracted official com.unity.scriptablebuildpipeline 1.20.1 package is required.'
}
if (Test-Path -LiteralPath $RunDirectory) { throw 'RunDirectory must not already exist; earlier evidence is never overwritten.' }
$RunDirectory = [IO.Path]::GetFullPath($RunDirectory)
$ancestor = [IO.DirectoryInfo]::new($RunDirectory).Parent
while ($null -ne $ancestor) {
    if (Test-Path -LiteralPath (Join-Path $ancestor.FullName 'ProjectSettings/ProjectVersion.txt')) {
        throw 'RunDirectory must be outside existing Unity projects.'
    }
    $ancestor = $ancestor.Parent
}
$packagePrefix = $SbpPackagePath.TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
if ($RunDirectory.StartsWith($packagePrefix,[StringComparison]::OrdinalIgnoreCase)) {
    throw 'RunDirectory must not be inside the SBP source package.'
}
[IO.Directory]::CreateDirectory($RunDirectory) | Out-Null
$fixture = Join-Path $PSScriptRoot 'fixture'
$manifest = @{ dependencies = @{
    'com.unity.scriptablebuildpipeline' = ('file:' + $SbpPackagePath.Replace('\','/'))
    'com.unity.modules.assetbundle' = '1.0.0'
    'com.unity.modules.jsonserialize' = '1.0.0'
} }
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$revision = & git -C $repo rev-parse HEAD
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify the source revision.' }
$evidence = Join-Path $repo ('TestResults/OptionOracle/' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($evidence) | Out-Null
$provenance = [ordered]@{
    ticket = 'https://github.com/VyronLee/vFrame.AIWorkspace/issues/110'
    sourceBaseRevision = $revision
    runDirectory = $RunDirectory
    retainedEvidenceDirectory = $evidence
    unityExecutable = $UnityPath
    sbpVersion = $package.version
    sbpPackageMetadataHash = (Get-FileHash (Join-Path $SbpPackagePath 'package.json') -Algorithm SHA256).Hash
    sbpSourceFiles = @(@('Editor/ContentPipeline.cs','Editor/CompatibilityBuildPipeline.cs',
        'Editor/Tasks/WriteSerializedFiles.cs','Editor/Tasks/ArchiveAndCompressBundles.cs',
        'Editor/Utilities/TaskCachingUtility.cs','Editor/Utilities/BuildCacheUtility.cs') | ForEach-Object {
        @{ path=$_; sha256=(Get-FileHash -LiteralPath (Join-Path $SbpPackagePath $_) -Algorithm SHA256).Hash }
    })
    sourceFiles = @(Get-ChildItem -LiteralPath $PSScriptRoot -Recurse -File | Where-Object {
        $_.Extension -in @('.ps1','.cs','.asmdef')
    } | ForEach-Object {
        @{ path = [IO.Path]::GetRelativePath($PSScriptRoot,$_.FullName); sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    })
    commands = @()
    pendingMatrix = @('Unity 2019.4.40f1 (not executed; no editor installation authorized)')
}
$provenancePath = Join-Path $RunDirectory 'provenance.json'
[IO.File]::WriteAllText($provenancePath,($provenance | ConvertTo-Json -Depth 8))
Write-Output "OracleRun=$RunDirectory"
Write-Output "RetainedEvidence=$evidence"
function Save-OracleArtifacts([string]$Project, [string]$Track, [string]$Log) {
    Copy-Item -LiteralPath $provenancePath -Destination (Join-Path $evidence 'provenance.json') -Force
    if (Test-Path -LiteralPath $Log) { Copy-Item -LiteralPath $Log -Destination $evidence }
    $results = Join-Path $Project 'OracleResults'
    if (Test-Path -LiteralPath $results) {
        foreach ($file in Get-ChildItem -LiteralPath $results -Filter '*.json' -File) {
            $destination = Join-Path $evidence ("$Track-" + $file.Name)
            if (-not (Test-Path -LiteralPath $destination)) { Copy-Item -LiteralPath $file.FullName -Destination $destination }
        }
    }
}
$sessions = @()
foreach ($track in @('standard','ignore')) {
    # Separate projects prevent native flag variants from sharing Unity's internal bundle cache.
    $project = Join-Path $RunDirectory ("project-" + $track)
    $runtime = Join-Path $project 'Assets/Oracle/Runtime'
    $editor = Join-Path $project 'Assets/Oracle/Editor'
    foreach ($path in @($runtime,$editor,(Join-Path $project 'Packages'),(Join-Path $project 'ProjectSettings'))) {
        [IO.Directory]::CreateDirectory($path) | Out-Null
    }
    Copy-Item -LiteralPath (Join-Path $fixture 'OracleAsset.cs'),(Join-Path $fixture 'vFrame.Bundler.OptionOracle.asmdef') -Destination $runtime
    Copy-Item -LiteralPath (Join-Path $fixture 'OracleRunner.cs'),(Join-Path $fixture 'vFrame.Bundler.OptionOracle.Editor.asmdef') -Destination $editor
    [IO.File]::WriteAllText((Join-Path $project 'Packages/manifest.json'),($manifest | ConvertTo-Json -Depth 5))
    [IO.File]::WriteAllText((Join-Path $project 'ProjectSettings/ProjectVersion.txt'),"m_EditorVersion: 2022.3.62f3`n")
    foreach ($phase in @('warm','layout','content')) {
        if ($phase -eq 'layout') {
            # The preceding process has exited; no script changes occur inside a running Editor.
            $path = Join-Path $runtime 'OracleAsset.cs'
            $source = [IO.File]::ReadAllText($path)
            if ([regex]::Matches($source,'// ORACLE_LAYOUT_FIELD').Count -ne 1) { throw 'Expected one schema insertion marker.' }
            [IO.File]::WriteAllText($path,$source.Replace('// ORACLE_LAYOUT_FIELD','public int layoutOnly;'))
        }
        $log = Join-Path $RunDirectory ("$track-$phase.log")
        $args = @('-batchmode','-nographics','-projectPath',$project,'-buildTarget','Win64',
            '-executeMethod','vFrame.Bundler.OptionOracle.Editor.OracleRunner.Run','-oraclePhase',$phase,
            '-oracleTrack',$track,'-logFile',$log)
        $start = [Diagnostics.ProcessStartInfo]::new($UnityPath)
        $start.UseShellExecute = $false
        foreach ($argument in $args) { $start.ArgumentList.Add($argument) }
        $start.Environment['VFRAME_ORACLE_INJECT'] = Join-Path $project 'oracle-inject.flag'
        $provenance.commands += @{ executable = $UnityPath; arguments = $args; startedUtc = [DateTime]::UtcNow.ToString('o') }
        [IO.File]::WriteAllText($provenancePath,($provenance | ConvertTo-Json -Depth 8))
        $process = $null
        $exitCode = $null
        try {
            $process = [Diagnostics.Process]::Start($start)
            if (-not $process.WaitForExit(300000)) {
                $process.Kill($true)
                $process.WaitForExit()
                $provenance.commands[-1].timedOut = $true
                throw "Oracle phase timed out: $track/$phase. Only the owned child process was stopped."
            }
        }
        finally {
            try {
                if ($null -ne $process -and $process.HasExited) {
                    $exitCode = $process.ExitCode
                    $provenance.commands[-1].exitCode = $exitCode
                }
                $provenance.commands[-1].finishedUtc = [DateTime]::UtcNow.ToString('o')
                [IO.File]::WriteAllText($provenancePath,($provenance | ConvertTo-Json -Depth 8))
                Save-OracleArtifacts $project $track $log
            }
            finally { if ($null -ne $process) { $process.Dispose() } }
        }
        Write-Output "Track=$track Phase=$phase ExitCode=$exitCode Log=$log"
        if ($exitCode -ne 0) { throw "Oracle phase failed: $track/$phase. Preserved evidence: $RunDirectory" }
        $sessions += ConvertFrom-Json -InputObject ([IO.File]::ReadAllText((Join-Path $project "OracleResults/$phase.json")))
    }
}
$verdict = Test-OracleEvidence -Sessions $sessions
[IO.File]::WriteAllText((Join-Path $RunDirectory 'verdict.json'),($verdict | ConvertTo-Json -Depth 10))
Copy-Item -LiteralPath (Join-Path $RunDirectory 'verdict.json') -Destination $evidence
$verdict.Checks | Format-Table -AutoSize
if (-not $verdict.Passed) { throw "Oracle controls failed/inconclusive; inspect $RunDirectory/verdict.json" }
Write-Output "Oracle controls passed. Product equivalence and the pending 2019.4 matrix are NOT certified. Evidence: $evidence"
