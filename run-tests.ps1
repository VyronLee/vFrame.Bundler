$ErrorActionPreference = "Stop"
$unity = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe"
$proj  = Split-Path -Parent $MyInvocation.MyCommand.Path
$log   = Join-Path $proj "test-run.log"
$res   = Join-Path $proj "TestResults\headless-editmode-results.xml"

# Clean stale artifacts (Unity overwrites -logFile; HeadlessTestRunner overwrites the XML,
# but clearing avoids reading a previous run's output if this run fails to write).
if (Test-Path $log) { Remove-Item $log -Force }
if (Test-Path $res) { Remove-Item $res -Force }

# In-process TestRunnerApi runner (no Unity Test Protocol port), so this works under
# -batchmode -executeMethod even where `-runTests` is blocked on this host.
$argList = @(
    "-batchmode", "-nographics", "-quit",
    "-projectPath", $proj,
    "-executeMethod", "vFrame.Bundler.Editor.TestRunner.HeadlessTestRunner.RunAllEditMode",
    "-logFile", $log
)

$p = Start-Process -FilePath $unity -ArgumentList $argList -PassThru -Wait
Write-Output "UnityExitCode=$($p.ExitCode)"

Write-Output "--- results XML ---"
if (Test-Path $res) {
    Get-Content $res -Raw
} else {
    Write-Output "(NO results XML - executeMethod did not fire or asmdef did not compile)"
}
