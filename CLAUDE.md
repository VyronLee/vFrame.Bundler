# vFrame.Bundler

Unity AssetBundle build pipeline and runtime loader with one API across `AssetDatabase`, `Resources`, and `AssetBundle` modes. Handles dependency analysis, packing, manifest generation, and automatic reference-counted collection.

## Package
- **UPM name**: `com.vyronlee.vframe.bundler`
- **Unity minimum**: 2019.4 (built with 2022.3.62f3)

## Assemblies
| Assembly | Location | Platform | Dependencies |
|----------|----------|----------|-------------|
| `vFrame.Bundler` | `Runtime/` | All | None (standalone) |
| `vFrame.Bundler.Editor` | `Editor/` | Editor only | `vFrame.Bundler` |

No test assembly in this repo.

## Build Pipeline (Editor)
Three pipelines under `BundleGenerator`:
- **BuiltinPipeline** — formal AssetBundle build (default)
- **SimulationPipeline** — editor-only; generates manifest without real AB files
- **ScriptablePipeline** — for custom workflows

Entry: `BundleGenerator.BuiltinPipeline.Build(BundleBuildRules, BundleBuildSettings)`.

**BuiltinPipeline task order** (do not reorder):
1. `AnalyzeMainAssetsTask` 2. `AnalyzeDependencyAssetsTask` 3. `AutoGroupingDependenciesTask`
4. `BuildBundlesInfoTask` 5. `BuildAssetBundleTask` 6. `ValidateBuildOutcomesTask` 7. `BuildBundlerManifestTask` (writes `BundlerManifest.json`)

**Rules** (`BundleBuildRules`): `MainRules` (directly-loaded assets; `Include`/`Exclude` regex under `SearchPath`) and `GroupRules` (shared-dependency grouping).

**PackType**: `PackBySingleFile` | `PackByAllFiles` | `PackByTopDirectory` | `PackByAllDirectories`.

**Menu**: `Tools/vFrame/Bundler/Profiler` — runtime JSON-RPC profiler (connects to `BundlerOptions.ListenAddress`, default `http://127.0.0.1:16667/`).

## Runtime Loading
**Modes** (`BundlerMode`): `AssetDatabase` (editor direct), `Resources`, `AssetBundle` (requires manifest).

Construct `new Bundler(BundlerManifest, BundlerOptions)`; drive each frame with `Update()` then `Collect()`; shut down with `Destroy()`. `SearchPaths` are searched in order — put patch/hotfix directories first.

`IBundler` loads assets/scenes via `LoadAsset`/`LoadAssetAsync`/`LoadAssetWithSubAssets*`/`LoadScene*` (sync + async variants).

### Wrapper structs (critical)
`LoadAsset`/`LoadAssetAsync` return **`Asset`/`AssetAsync` structs**, not raw `Object` — the wrapper drives automatic reference counting.

```csharp
var prefab = bundler.LoadAsset<GameObject>("path");   // keep the wrapper
var instance = prefab.Instantiate();
// Wrong — leaks: bundler.LoadAsset<GameObject>("path").GetRawAsset()
```
`GetRawAsset()` extracts the raw object and shifts `Retain()`/`Release()` responsibility to you.

Built-in property-link extensions auto-track refs: `Image.SetSprite/SetMaterial`, `Renderer.SetMaterial/SetSharedMaterial`, `Projector.SetMaterial`, `SpriteRenderer.SetSprite`, `AudioSource.SetClip`.

For FBX sub-assets or sprite sheets, use `LoadAssetWithSubAssets*`.

## Gotchas
- **Update before Collect** (CRITICAL): call `Bundler.Update()` **before** `Bundler.Collect()` each frame, or assets completed this frame are collected before gameplay uses them.
- **Wrapper, not raw object**: dropping the `Asset` wrapper without `Retain()` leaks the reference.
- **Manifest coverage**: every runtime-loaded asset must match a `MainRules` entry, or it won't appear in `BundlerManifest`.

## Build & Test
```powershell
dotnet build "D:/Workspace/vFrame/vFrame.Bundler/vFrame.Bundler.sln" --no-restore
```
No test assembly in this repo.

## Relationship to VFS
Standalone. To load AssetBundles from VFS packages, use `vFrame.Bundler.VFSAdapter` (implements `IAssetBundleCreateAdapter`).

## Cross-Package Conventions
See workspace-root `.claude/rules/gotchas.md` (Update-before-Collect, ref-counted wrappers).
