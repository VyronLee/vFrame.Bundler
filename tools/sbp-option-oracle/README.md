[English](README.md) | [简体中文](README.zh-CN.md)

# SBP option oracle (#110)

A reproducible **evidence tool**, not Bundler's SBP implementation or a product-compatibility certificate.

## Requirements and installation

Use Windows, PowerShell 7+, the existing Unity **2022.3.62f3** editor with Windows support, and an extracted official **SBP 1.20.1** package. Clone this repository; no extra library, editor download, or package installation into the Bundler project is needed. Only disposable projects reference the supplied local SBP package.

## Run

From the repository root:

```powershell
pwsh -NoProfile -File tools/sbp-option-oracle/test-report.ps1
pwsh -NoProfile -File tools/sbp-option-oracle/test-preflight.ps1
pwsh -NoProfile -File tools/sbp-option-oracle/run.ps1 -SbpPackagePath '<extracted-sbp-1.20.1>'
```

Use `-UnityPath '<Unity.exe>'` if necessary. `-RunDirectory '<new-short-path>'` must name a nonexistent directory outside existing Unity projects and the SBP source package; previous runs are never replaced. The runner stops only its own child process on timeout and never closes another Editor.

## What is measured

- Six completed Editor processes: warm/layout/content phases in two projects. Native flag variants have independent caches. Only the host edits C# between processes; both loaded Editor and compiled Player fields are checked.
- Fixed output/cache settings, actual native BuildReport bundle steps, and public SBP write/archive log events distinguish reuse, layout/content invalidation, and force rebuilding. File counts or hashes alone are not evidence of execution.
- A serialization-callback error is enabled only during the build call. Strict/lenient outcomes are recorded as observed, with genuinely clean runs before/after. A missing input checks failure handling separately; no hardcoded bad-asset guard forces rejection.
- Report self-tests use synthetic data **only to test the classifier**. Their success is not recorded as a Unity observation.

## Evidence and interpretation

The runner prints its unique temporary workbench and durable local `TestResults/OptionOracle/<id>/` evidence directory. The latter retains provenance, exact commands/exit codes, phase JSON, build logs and verdict, including failed phases. It is gitignored because raw Unity logs can contain machine/license metadata. Preserve or export it before cleaning local test results.

The committed `baseline-2026-10-18.json` is a machine-path-free snapshot of the recorded source identities and results; it is not a replacement for rerunning the tool.

`verdict.json` reports control validity separately from raw StrictMode outcomes and never certifies product equivalence. Nonzero exit, missing sessions, schema mismatch, missing stimuli or failed controls must not become a green gate. Original invalid probe artifacts are not read, edited or deleted; this protocol supersedes their experimental method, not their recorded failures.

In the recorded 2022.3 fixture, layout changes rebuild two SBP bundles; unchanged input rebuilds none; force rebuild writes all three. Native IgnoreTypeTreeChanges can reuse the old bundle layout while ordinary native builds rebuild. Serialization `Debug.LogError` signals do not reject either raw backend merely because StrictMode is set; native BuildReport reports zero errors for this stimulus. This does **not** prove all StrictMode error classes or a production guard.

Unity **2019.4.40f1**, actual IBundler/Player acceptance, scenes/shaders and other option semantics remain unverified. An unavailable editor is never treated as passed. CI license activation remains an external, separately tracked gate.

## License

[Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0).
