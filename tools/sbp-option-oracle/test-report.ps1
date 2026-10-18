$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'report.ps1')

$rejected = $false
try { Test-OracleEvidence -Sessions @() | Out-Null }
catch {
    if ($_.Exception.Message -ne 'No oracle sessions were supplied.') { throw }
    $rejected = $true
}
if (-not $rejected) { throw 'An empty run must never be accepted.' }
Write-Output 'PASS: an empty oracle run is rejected'

$incomplete = Test-OracleEvidence -Sessions @([pscustomobject]@{ track='standard'; phase='warm' })
$matrixCheck = @($incomplete.Checks | Where-Object Name -eq 'complete session matrix')
if ($incomplete.Passed -or $matrixCheck.Count -ne 1 -or $matrixCheck[0].Passed) {
    throw 'Incomplete sessions must have an explicit failed matrix check.'
}
Write-Output 'PASS: an incomplete session matrix cannot be certified'

# These synthetic observations test the classifier only; they are never Unity evidence.
function New-TestObservation([string]$Name) {
    $backend = if ($Name.StartsWith('sbp')) { 'sbp' } else { 'native' }
    $track = if ($Name.StartsWith('native-ignore')) { 'ignore' } else { 'standard' }
    $key = if ($backend -eq 'sbp') { 'sbp' } elseif ($track -eq 'ignore') { 'native-ignore' } else { 'native' }
    $options = 'ChunkBasedCompression'
    if ($track -eq 'ignore') { $options += ', IgnoreTypeTreeChanges' }
    $cache = -not $Name.EndsWith('force')
    if (-not $cache) { $options += ', ForceRebuildAssetBundle' }
    $count = if ($Name.EndsWith('warm') -or $Name.EndsWith('force')) { 3 }
        elseif ($Name.EndsWith('unchanged') -or $Name -eq 'native-ignore-layout') { 0 }
        elseif ($Name.EndsWith('layout')) { 2 } else { 1 }
    [pscustomobject]@{
        name=$Name; backend=$backend; returnCode='Manifest'; succeeded=$true; loadedValuesValid=$true; errorCount=0; injectedErrors=0
        exception=''; gateRemoved=$true; bundleNames=@('leaf.ab','note.ab','root.ab'); options=$options; useCache=$cache
        outputDirectory=(Join-Path ([IO.Path]::GetTempPath()) "oracle-synthetic/$track/OracleOutput/$key")
        serializedWrites=$count; archiveWrites=$count; nativeSteps=@(1..$count | Where-Object { $count -gt 0 } | ForEach-Object { "Build bundle: $_" })
        nativeReportErrors=0
    }
}
function New-TestSessions {
    $normal = @('native-warm','native-unchanged','sbp-warm','sbp-unchanged','native-layout',
        'sbp-layout','sbp-layout-unchanged','sbp-force','native-force','native-content','sbp-content',
        'sbp-content-unchanged','native-ignore-warm','native-ignore-unchanged','native-ignore-layout','native-ignore-content')
    $observations = @($normal | ForEach-Object { New-TestObservation $_ })
    foreach ($backend in @('native','sbp-compat')) {
        foreach ($case in @('clean-before','error-strict','clean-after-strict','error-lenient',
            'clean-after-lenient','invalid-strict','invalid-lenient','clean-after-invalid')) {
            $o = New-TestObservation "strict-$backend-$case"
            $o.backend = $backend
            $o.outputDirectory = Join-Path ([IO.Path]::GetTempPath()) "oracle-synthetic/standard/OracleOutput/strict-$backend"
            $o.useCache = $false
            $o.options = 'ChunkBasedCompression, ForceRebuildAssetBundle'
            if ($case.EndsWith('strict') -or $case -eq 'clean-before') { $o.options += ', StrictMode' }
            if ($case.StartsWith('error-')) { $o.errorCount = 1; $o.injectedErrors = 1 }
            if ($case.StartsWith('invalid-')) { $o.succeeded = $false; $o.returnCode = 'NullManifest'; $o.bundleNames = @() }
            $observations += $o
        }
    }
    foreach ($track in @('standard','ignore')) {
        foreach ($phase in @('warm','layout','content')) {
            $fields = @('link:vFrame.Bundler.OptionOracle.OracleAsset','value:System.Int32')
            if ($phase -ne 'warm') { $fields = @('layoutOnly:System.Int32') + $fields }
            $names = @($normal | Where-Object {
                $actualTrack = if ($_.StartsWith('native-ignore')) { 'ignore' } else { 'standard' }
                $actualPhase = if ($_ -match 'warm|unchanged$' -and $_ -notmatch 'layout|content') { 'warm' }
                    elseif ($_ -match 'layout|force') { 'layout' } else { 'content' }
                $actualTrack -eq $track -and $actualPhase -eq $phase
            })
            if ($phase -eq 'content' -and $track -eq 'standard') { $names += @($observations.name | Where-Object { $_.StartsWith('strict-') }) }
            $hash = if ($phase -eq 'warm') { 'schema-a' } else { 'schema-b' }
            [pscustomobject]@{
                track=$track; phase=$phase; unityVersion='2022.3.62f3'; sbpVersion='1.20.1'; processId=123
                sessionId="$track-$phase"; fatal=''; editorFields=$fields; playerFields=@($fields)
                sourceBefore=$hash; sourceAfter=$hash; observations=@($observations | Where-Object { $_.name -in $names })
            }
        }
    }
}
$baseline = Test-OracleEvidence -Sessions @(New-TestSessions)
if (-not $baseline.Passed) { $baseline.Checks | Where-Object { -not $_.Passed } | Format-Table; throw 'Valid classifier fixture rejected.' }
if ($baseline.ProductEquivalenceCertified -or -not $baseline.StrictOutcomes[0].StrictReturnedSuccess) {
    throw 'The classifier must preserve raw strict outcomes without certifying product equivalence.'
}
Write-Output 'PASS: a complete synthetic fixture validates the classifier, not Unity behavior'

foreach ($mutation in @('schema','source','injection','cache','output','duplicates','native-work','invalid-input',
    'strict-flag','lenient-flag','backend','missing-report','missing-success','missing-return-code','counter-type')) {
    $sessions = @(New-TestSessions)
    $all = @($sessions | ForEach-Object { $_.observations })
    switch ($mutation) {
        schema { $sessions[1].playerFields = @('wrong:field') }
        source { $sessions[1].sourceAfter = 'changed-during-session' }
        injection { ($all | Where-Object name -eq 'strict-native-error-strict').injectedErrors = 0 }
        cache { ($all | Where-Object name -eq 'sbp-layout').serializedWrites = 0 }
        output { ($all | Where-Object name -eq 'native-unchanged').outputDirectory += '-fresh-directory' }
        duplicates { $sessions[0].observations += $sessions[0].observations[0] }
        native-work { ($all | Where-Object name -eq 'native-warm').nativeSteps = @() }
        invalid-input {
            $o = $all | Where-Object name -eq 'strict-native-invalid-strict'
            $o.succeeded = $true; $o.bundleNames = @('leaf.ab','note.ab','root.ab')
        }
        strict-flag {
            foreach ($o in $all | Where-Object { $_.name.StartsWith('strict-') }) {
                $o.options = $o.options.Replace(', StrictMode','')
            }
        }
        lenient-flag { ($all | Where-Object name -eq 'strict-native-error-lenient').options += ', StrictMode' }
        backend { ($all | Where-Object name -eq 'strict-native-error-strict').backend = 'sbp-compat' }
        missing-report { ($all | Where-Object name -eq 'native-unchanged').nativeReportErrors = -1 }
        missing-success { ($all | Where-Object name -eq 'strict-native-error-strict').PSObject.Properties.Remove('succeeded') }
        missing-return-code { ($all | Where-Object name -eq 'strict-native-error-strict').PSObject.Properties.Remove('returnCode') }
        counter-type { ($all | Where-Object name -eq 'sbp-unchanged').serializedWrites = '0' }
    }
    if ((Test-OracleEvidence -Sessions $sessions).Passed) { throw "Invalid classifier fixture accepted: $mutation" }
    Write-Output "PASS: rejects missing/invalid $mutation evidence"
}
