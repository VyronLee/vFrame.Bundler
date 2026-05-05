[English](README.md) | [简体中文](README.zh-CN.md)

# vFrame Bundler

A Unity AssetBundle build and loading framework with a unified runtime API and automatic Bundle dependency, reference, and collection management.

## Features

- Exposes one runtime loading API across `AssetDatabase`, `Resources`, and `AssetBundle` modes
- Uses `BundleBuildRules` and `BundleBuildSettings` to drive AssetBundle builds
- Splits build input into `MainRules` and `GroupRules` for main assets and shared dependencies
- Generates `BundlerManifest` with asset-to-bundle mappings and bundle dependency data
- Returns `Asset` / `AssetAsync` wrappers from `LoadAsset` / `LoadAssetAsync` instead of raw `UnityEngine.Object`
- Supports synchronous and asynchronous loading for assets, sub-assets, and scenes
- Tracks references through `Instantiate()`, `SetTo()`, and built-in property-link extensions
- Supports custom bundle creation or decryption through `IAssetBundleCreateAdapter`
- Includes a runtime profiler window at `Tools/vFrame/Bundler/Profiler`

## Requirements

- Unity `2022.3.62f3` in this repository, with package metadata declaring `2019.4` as the minimum version
- UPM package name: `com.vyronlee.vframe.bundler`
- Runtime assembly: `vFrame.Bundler`
- Editor assembly: `vFrame.Bundler.Editor` (Editor platform only)

## Installation

Add this dependency to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.vyronlee.vframe.bundler": "https://github.com/VyronLee/vFrame.Bundler.git?path=/Assets/vFrame.Bundler"
  }
}
```

You can also install it from `Window > Package Manager > Add package from git URL...` with the same URL.

## Quick Start

This example reads `BundlerManifest`, creates a `Bundler`, loads one prefab, and drives async progress and automatic collection every frame.

```csharp
using System.IO;
using UnityEngine;
using vFrame.Bundler;

public class BundlerBootstrap : MonoBehaviour
{
    private IBundler _bundler;

    private void Start()
    {
        var manifestPath = Path.Combine(Application.streamingAssetsPath, "Bundles", "BundlerManifest.json");
        var manifestJson = File.ReadAllText(manifestPath);
        var manifest = BundlerManifest.FromJson(manifestJson);

        var options = new BundlerOptions {
            Mode = BundlerMode.AssetBundle,
            SearchPaths = new[] {
                Path.Combine(Application.persistentDataPath, "Patches"),
                Path.Combine(Application.streamingAssetsPath, "Bundles")
            }
        };

        _bundler = new Bundler(manifest, options);
        _bundler.SetLogLevel(LogLevel.Error);

        var prefab = _bundler.LoadAsset<GameObject>("Assets/Bundles/Prefabs/Player.prefab");
        prefab.Instantiate();
    }

    private void LateUpdate()
    {
        _bundler.Update();
        _bundler.Collect();
    }

    private void OnDestroy()
    {
        _bundler?.Destroy();
    }
}
```

## Usage

### 1. Define build rules

`BundleBuildRules` contains two rule groups:

- `MainRules`: assets your game loads directly at runtime
- `GroupRules`: rules for grouping shared dependencies into shared bundles

```json
{
  "MainRules": [
    {
      "PackType": "PackBySingleFile",
      "SearchPath": "Assets/Bundles/Scene",
      "Include": ".+\\.unity$",
      "Exclude": ""
    },
    {
      "PackType": "PackByAllDirectories",
      "SearchPath": "Assets/Bundles/Avatar",
      "Include": ".+\\.prefab$",
      "Exclude": ".+/_Motion/.+"
    }
  ],
  "GroupRules": [
    { "Include": "(Assets/Bundles/.+/Material/).+" },
    { "Include": "(Assets/Bundles/.+/Texture/).+" }
  ]
}
```

Supported `PackType` values are `PackBySingleFile`, `PackByAllFiles`, `PackByTopDirectory`, and `PackByAllDirectories`.

### 2. Build AssetBundles

Use `BundleGenerator.BuiltinPipeline` for the formal build flow:

```csharp
using System.IO;
using UnityEditor;
using vFrame.Bundler;

var buildRules = BundleBuildRules.FromJson(File.ReadAllText("Assets/BundleRules.json"));
var buildSettings = new BundleBuildSettings {
    BundlePath = "Bundles/StandaloneWindows64",
    ManifestFileName = "BundlerManifest.json",
    BuildTarget = BuildTarget.StandaloneWindows64,
    HashAssetBundlePath = true,
    SeparateShaderBundle = true
};

BundleGenerator.BuiltinPipeline.Build(buildRules, buildSettings);
```

The built-in pipeline runs these tasks in order: `AnalyzeMainAssetsTask`, `AnalyzeDependencyAssetsTask`, `AutoGroupingDependenciesTask`, `BuildBundlesInfoTask`, `BuildAssetBundleTask`, `ValidateBuildOutcomesTask`, and `BuildBundlerManifestTask`.

For editor-only simulation, use `BundleGenerator.SimulationPipeline.Build(...)`. It only runs `AnalyzeMainAssetsTask` and `BuildBundlerManifestTask`, so it generates a manifest without producing real AssetBundle files.

### 3. Initialize the runtime loader

Important `BundlerOptions` members include:

- `Mode`: `AssetDatabase`, `Resources`, or `AssetBundle`
- `SearchPaths`: ordered bundle root paths
- `AssetBundleCreateAdapter`: custom `AssetBundle` creation logic
- `LogHandler`: custom logging sink
- `MinAsyncFrameCountOnSimulation` / `MaxAsyncFrameCountOnSimulation`: simulated async delay in editor mode
- `ListenAddress`: JSON-RPC address for the runtime profiler

In most projects, put patch or hotfix directories before built-in bundle directories in `SearchPaths`.

### 4. Load assets at runtime

`IBundler` exposes these main APIs:

```csharp
Asset LoadAsset(string path, Type type);
AssetAsync LoadAssetAsync(string path, Type type);
Asset<T> LoadAsset<T>(string path) where T : Object;
AssetAsync<T> LoadAssetAsync<T>(string path) where T : Object;
Asset LoadAssetWithSubAssets(string path, Type type);
AssetAsync LoadAssetWithSubAssetsAsync(string path, Type type);
Scene LoadScene(string path, LoadSceneMode mode);
SceneAsync LoadSceneAsync(string path, LoadSceneMode mode);
```

Load a prefab synchronously:

```csharp
var prefab = bundler.LoadAsset<GameObject>("Assets/Bundles/Prefabs/Enemy.prefab");
var enemy = prefab.Instantiate();
```

Load a sprite asynchronously and bind it to UI:

```csharp
var iconAsset = bundler.LoadAssetAsync<Sprite>("Assets/Bundles/UI/Icons/Sword.png");
yield return iconAsset;
image.SetSprite(iconAsset);
```

Load a scene asynchronously:

```csharp
var scene = bundler.LoadSceneAsync("Assets/Bundles/Scene/Battle.unity", LoadSceneMode.Single);
yield return scene;
scene.Activate();
```

### 5. Work with `Asset` and `AssetAsync` wrappers

`LoadAsset` and `LoadAssetAsync` do not return raw Unity objects. They return `Asset`, `Asset<T>`, `AssetAsync`, or `AssetAsync<T>` so Bundler can track loader state and reference counts.

Common wrapper members:

- `GetRawAsset()` / `GetAllRawAssets()`
- `Instantiate()`
- `Retain()` / `Release()`
- `SetTo<TComponent, ...>()`
- `Unload()`
- `IsDone` / `Progress` on async wrappers

Built-in property-link extensions include `Image.SetSprite(...)`, `Image.SetMaterial(...)`, `Renderer.SetMaterial(...)`, `Renderer.SetSharedMaterial(...)`, `Projector.SetMaterial(...)`, `SpriteRenderer.SetSprite(...)`, and `AudioSource.SetClip(...)`.

### 6. Use the profiler

Open `BundleProfiler` from `Tools/vFrame/Bundler/Profiler`. It communicates with the runtime over JSON-RPC and shows:

- Loaders
- Pipelines
- Handlers
- Links

The runtime endpoint comes from `BundlerOptions.ListenAddress`, which defaults to `http://127.0.0.1:16667/`.

## Architecture Overview

### Runtime

- `Bundler`: main external entry point implementing `IBundler`
- `BundlerManifest`: serialized asset-to-bundle mapping and bundle dependency data
- `LoadSystem`: orchestrates asset and scene loading
- `LinkSystem`: tracks instantiated objects and property links
- `CollectSystem`: releases unreferenced resources
- `ProfileSystem`: exposes runtime state to the editor profiler

### Editor

- `BundleBuildRules` / `BundleBuildSettings`: build input models
- `BundleGenerator`: pipeline entry point
- `BuiltinPipeline`: formal build flow
- `SimulationPipeline`: simulation-only flow
- `BundleProfiler`: editor debugging window

## Notes

- Call `Bundler.Update()` before `Bundler.Collect()`, or assets completed this frame can be collected before gameplay consumes them.
- `LoadAsset` / `LoadAssetAsync` return wrapper types, not raw Unity objects. If you keep raw objects from `GetRawAsset()`, you are responsible for balancing `Retain()` and `Release()`.
- Every dynamically loaded asset must match at least one `MainRules` entry, or it will not appear in `BundlerManifest`.
- `SearchPaths` are evaluated in order, so patch directories usually go first.
- `LoadAssetWithSubAssets*` is useful for assets like FBX sub-assets or sprite collections where one path resolves to multiple objects.

## License

This project is licensed under This project is licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0).
