function Test-OracleEvidence {
    param([object[]]$Sessions)
    if ($Sessions.Count -eq 0) { throw 'No oracle sessions were supplied.' }
    $checks = [Collections.Generic.List[object]]::new()
    function Add-Check([string]$Name, [bool]$Passed) {
        $checks.Add([pscustomobject]@{ Name=$Name; Passed=$Passed })
    }
    $keys = @($Sessions | ForEach-Object { "$($_.track)/$($_.phase)" } | Sort-Object)
    $required = @('ignore/content','ignore/layout','ignore/warm','standard/content','standard/layout','standard/warm')
    Add-Check 'complete session matrix' (($keys -join ',') -eq ($required -join ','))
    if (-not $checks[0].Passed) { return [pscustomobject]@{ Passed=$false; Checks=$checks.ToArray(); StrictOutcomes=@() } }
    Add-Check 'independent Editor sessions' (@($Sessions.sessionId | Select-Object -Unique).Count -eq 6)
    foreach ($session in $Sessions) {
        $expected = @('link:vFrame.Bundler.OptionOracle.OracleAsset','value:System.Int32')
        if ($session.phase -ne 'warm') { $expected = @('layoutOnly:System.Int32') + $expected }
        Add-Check "$($session.track)/$($session.phase) schema and immutable script" (
            [string]::IsNullOrEmpty($session.fatal) -and $session.unityVersion -eq '2022.3.62f3' -and
            $session.sbpVersion -eq '1.20.1' -and $session.processId -gt 0 -and
            -not [string]::IsNullOrEmpty($session.sourceBefore) -and $session.sourceBefore -eq $session.sourceAfter -and
            ($session.editorFields -join '|') -eq ($expected -join '|') -and
            ($session.playerFields -join '|') -eq ($expected -join '|'))
    }
    foreach ($track in @('standard','ignore')) {
        $phases = @($Sessions | Where-Object track -eq $track)
        $warm = $phases | Where-Object phase -eq 'warm'
        $layout = $phases | Where-Object phase -eq 'layout'
        $content = $phases | Where-Object phase -eq 'content'
        Add-Check "$track layout changes only between sessions" ($warm.sourceBefore -ne $layout.sourceBefore -and $layout.sourceBefore -eq $content.sourceBefore)
    }
    $all = @($Sessions | ForEach-Object { $_.observations })
    $normal = @('native-warm','native-unchanged','sbp-warm','sbp-unchanged','native-layout',
        'sbp-layout','sbp-layout-unchanged','sbp-force','native-force','native-content','sbp-content',
        'sbp-content-unchanged','native-ignore-warm','native-ignore-unchanged','native-ignore-layout','native-ignore-content')
    $expectedNames = @($normal)
    foreach ($backend in @('native','sbp-compat')) {
        foreach ($case in @('clean-before','error-strict','clean-after-strict','error-lenient',
            'clean-after-lenient','invalid-strict','invalid-lenient','clean-after-invalid')) {
            $expectedNames += "strict-$backend-$case"
        }
    }
    Add-Check 'complete unique observations' ((@($all.name | Sort-Object) -join '|') -eq (@($expectedNames | Sort-Object) -join '|'))
    if (-not $checks[-1].Passed) { return [pscustomobject]@{ Passed=$false; Checks=$checks.ToArray(); StrictOutcomes=@() } }
    $invalidMeasurements = @($all | Where-Object {
        $o = $_
        $invalidNumbers = @('errorCount','injectedErrors','serializedWrites','archiveWrites','nativeReportErrors' | Where-Object {
            $o.$_ -isnot [int] -and $o.$_ -isnot [long]
        })
        $o.succeeded -isnot [bool] -or $o.loadedValuesValid -isnot [bool] -or
        $o.useCache -isnot [bool] -or $o.gateRemoved -isnot [bool] -or
        $o.returnCode -isnot [string] -or [string]::IsNullOrWhiteSpace($o.returnCode) -or
        $o.options -isnot [string] -or [string]::IsNullOrWhiteSpace($o.options) -or
        $o.outputDirectory -isnot [string] -or [string]::IsNullOrWhiteSpace($o.outputDirectory) -or
        $invalidNumbers.Count -gt 0 -or $o.errorCount -lt 0 -or $o.injectedErrors -lt 0 -or
        ($o.backend -eq 'sbp' -and ($o.serializedWrites -lt 0 -or $o.archiveWrites -lt 0)) -or
        ($o.backend -eq 'native' -and $o.succeeded -and
            ($o.nativeReportErrors -lt 0 -or $o.nativeSteps -isnot [array]))
    })
    Add-Check 'complete typed measurements and available native reports' ($invalidMeasurements.Count -eq 0)
    $wrongBackends = @($all | Where-Object {
        $expectedBackend = if ($_.name.StartsWith('strict-sbp-compat-')) { 'sbp-compat' }
            elseif ($_.name.StartsWith('sbp-')) { 'sbp' } else { 'native' }
        $_.backend -cne $expectedBackend
    })
    Add-Check 'observation backend identities' ($wrongBackends.Count -eq 0)
    if ($invalidMeasurements.Count -gt 0 -or $wrongBackends.Count -gt 0) {
        return [pscustomobject]@{ Passed=$false; Checks=$checks.ToArray(); StrictOutcomes=@() }
    }
    $observed = @{}
    foreach ($record in $all) { $observed[$record.name] = $record }
    foreach ($name in $normal) {
        $o = $observed[$name]
        Add-Check "$name clean build and values" ($o.succeeded -and $o.loadedValuesValid -and $o.errorCount -eq 0 -and
            [string]::IsNullOrEmpty($o.exception) -and $o.gateRemoved -and
            ($o.backend -ne 'native' -or $o.nativeReportErrors -eq 0) -and
            (@($o.bundleNames | Sort-Object) -join ',') -eq 'leaf.ab,note.ab,root.ab')
    }
    foreach ($names in @(
        @('native-warm','native-unchanged','native-layout','native-content'),
        @('native-ignore-warm','native-ignore-unchanged','native-ignore-layout','native-ignore-content'),
        @('sbp-warm','sbp-unchanged','sbp-layout','sbp-layout-unchanged','sbp-content','sbp-content-unchanged')
    )) {
        $records = @($names | ForEach-Object { $observed[$_] })
        Add-Check "fixed output/cache options: $($names[0])" (
            @($records.outputDirectory | Select-Object -Unique).Count -eq 1 -and
            @($records.options | Select-Object -Unique).Count -eq 1 -and @($records | Where-Object { -not $_.useCache }).Count -eq 0)
    }
    $nativeParent = [IO.Path]::GetDirectoryName($observed['native-warm'].outputDirectory)
    $ignoreParent = [IO.Path]::GetDirectoryName($observed['native-ignore-warm'].outputDirectory)
    Add-Check 'native option variants have isolated project caches' ($nativeParent -ne $ignoreParent)
    foreach ($case in @(@('sbp-warm',3),@('sbp-unchanged',0),@('sbp-layout-unchanged',0),@('sbp-content-unchanged',0),@('sbp-force',3))) {
        $o = $observed[$case[0]]
        Add-Check "$($case[0]) actual write/archive work" ($o.serializedWrites -eq $case[1] -and $o.archiveWrites -eq $case[1])
    }
    foreach ($case in @(@('sbp-layout',2),@('sbp-content',1))) {
        $o = $observed[$case[0]]
        Add-Check "$($case[0]) actual invalidation" ($o.serializedWrites -ge $case[1] -and $o.archiveWrites -ge 1)
    }
    Add-Check 'forced SBP rebuild is separate from cache reuse' (-not $observed['sbp-force'].useCache -and $observed['sbp-force'].outputDirectory -eq $observed['sbp-warm'].outputDirectory)
    foreach ($case in @(@('native-warm',3),@('native-unchanged',0),@('native-ignore-warm',3),@('native-ignore-unchanged',0),@('native-force',3))) {
        $o = $observed[$case[0]]
        $steps = @($o.nativeSteps | Where-Object { $_ -like 'Build bundle:*' }).Count
        Add-Check "$($case[0]) real BuildReport bundle work" ($steps -eq $case[1])
    }
    foreach ($name in @('native-layout','native-content','native-ignore-content')) {
        Add-Check "$name real BuildReport invalidation" (@($observed[$name].nativeSteps | Where-Object { $_ -like 'Build bundle:*' }).Count -gt 0)
    }
    $outcomes = @()
    foreach ($backend in @('native','sbp-compat')) {
        $prefix = "strict-$backend-"
        $records = @($all | Where-Object { $_.name.StartsWith($prefix) })
        $wrongModes = @($records | Where-Object {
            $case = $_.name.Substring($prefix.Length)
            $expectedStrict = $case -in @('clean-before','error-strict','clean-after-strict','invalid-strict')
            ($_.options.Split(',').Trim() -contains 'StrictMode') -ne $expectedStrict
        })
        Add-Check "$backend strict/lenient option identities" ($wrongModes.Count -eq 0)
        if ($wrongModes.Count -gt 0) {
            return [pscustomobject]@{ Passed=$false; Checks=$checks.ToArray(); StrictOutcomes=@() }
        }
        $modes = @($records | ForEach-Object { ($_.options.Split(',').Trim() | Where-Object { $_ -ne 'StrictMode' }) -join ',' } | Select-Object -Unique)
        Add-Check "$backend strict/lenient fixed cache and output" (@($records.outputDirectory | Select-Object -Unique).Count -eq 1 -and
            @($records | Where-Object useCache).Count -eq 0 -and $modes.Count -eq 1)
        foreach ($case in @('clean-before','clean-after-strict','clean-after-lenient','clean-after-invalid')) {
            $o = $observed[$prefix + $case]
            Add-Check "$backend $case has no leaked error state" ($o.succeeded -and $o.loadedValuesValid -and
                $o.errorCount -eq 0 -and [string]::IsNullOrEmpty($o.exception) -and $o.gateRemoved -and
                ($o.backend -ne 'native' -or $o.nativeReportErrors -eq 0))
        }
        foreach ($case in @('error-strict','error-lenient')) {
            $o = $observed[$prefix + $case]
            Add-Check "$backend $case stimulus occurred inside build" ($o.injectedErrors -gt 0 -and $o.errorCount -ge $o.injectedErrors -and $o.gateRemoved)
        }
        foreach ($case in @('invalid-strict','invalid-lenient')) {
            $o = $observed[$prefix + $case]
            $complete = (@($o.bundleNames | Sort-Object) -join ',') -eq 'leaf.ab,note.ab,root.ab'
            Add-Check "$backend $case cannot produce a valid bundle set" ($o.gateRemoved -and (-not $o.succeeded -or -not $complete))
        }
        $strict = $observed[$prefix + 'error-strict']
        $lenient = $observed[$prefix + 'error-lenient']
        $outcomes += [pscustomobject]@{
            Backend=$backend; StrictReturnedSuccess=$strict.succeeded; LenientReturnedSuccess=$lenient.succeeded
            StrictWindowErrors=$strict.errorCount; LenientWindowErrors=$lenient.errorCount
            NativeBuildReportErrors=$strict.nativeReportErrors
            Scope='Observed serialization-callback log stimulus only; NOT certification of all StrictMode errors or a production guard.'
        }
    }
    [pscustomobject]@{
        Passed=(@($checks | Where-Object { -not $_.Passed }).Count -eq 0)
        Checks=$checks.ToArray(); StrictOutcomes=$outcomes
        NativeIgnoreLayoutBuildSteps=@($observed['native-ignore-layout'].nativeSteps | Where-Object { $_ -like 'Build bundle:*' }).Count
        ProductEquivalenceCertified=$false
        PendingMatrix=@('Unity 2019.4.40f1; not executed and no installation authorized')
    }
}
