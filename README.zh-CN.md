[English](README.md) | [简体中文](README.zh-CN.md)

# vFrame Bundler

面向 Unity 的 AssetBundle 构建与加载框架，提供统一的运行时加载 API，并自动管理 Bundle 依赖、引用关系与回收时机。

## 特性

- 统一 `AssetDatabase`、`Resources`、`AssetBundle` 三种模式的资源访问接口
- 通过 `BundleBuildRules` 和 `BundleBuildSettings` 配置 AssetBundle 构建流程
- 使用 `MainRules` 与 `GroupRules` 分析主资源和共享依赖
- 生成 `BundlerManifest`，记录资源路径、Bundle 路径和 Bundle 依赖关系
- `LoadAsset` / `LoadAssetAsync` 返回 `Asset` / `AssetAsync` 包装类型，而不是裸 `UnityEngine.Object`
- 支持同步与异步加载资源、子资源和场景
- 通过 `Instantiate()`、`SetTo()` 和属性链接扩展自动维护引用关系
- 支持 `IAssetBundleCreateAdapter` 扩展自定义 Bundle 创建或解密逻辑
- 提供 `Tools/vFrame/Bundler/Profiler` 编辑器窗口查看运行时加载状态

## 环境要求

- Unity `2022.3.62f3`（包声明的最低版本为 `2019.4`）
- UPM 包名：`com.vyronlee.vframe.bundler`
- 运行时程序集：`vFrame.Bundler`
- 编辑器程序集：`vFrame.Bundler.Editor`（仅 `Editor` 平台）

## 安装

将下面的依赖添加到项目的 `Packages/manifest.json`：

```json
{
  "dependencies": {
    "com.vyronlee.vframe.bundler": "https://github.com/VyronLee/vFrame.Bundler.git?path=/Assets/vFrame.Bundler"
  }
}
```

也可以在 Unity 中通过 `Window > Package Manager > Add package from git URL...` 添加同一个地址。

## 快速开始

下面的示例演示如何读取 `BundlerManifest`、初始化 `Bundler`、加载一个预制体，并在每帧推进异步流程与自动回收。

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

## 使用说明

### 1. 配置打包规则

`BundleBuildRules` 由两组规则组成：

- `MainRules`：定义代码会直接动态加载的主资源
- `GroupRules`：定义共享依赖的自动分组规则

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

可用的 `PackType` 包括：`PackBySingleFile`、`PackByAllFiles`、`PackByTopDirectory`、`PackByAllDirectories`。

### 2. 构建流程

正式构建入口是 `BundleGenerator.BuiltinPipeline`：

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

`BuiltinPipeline` 的内部步骤依次包括：`AnalyzeMainAssetsTask`、`AnalyzeDependencyAssetsTask`、`AutoGroupingDependenciesTask`、`BuildBundlesInfoTask`、`BuildAssetBundleTask`、`ValidateBuildOutcomesTask`、`BuildBundlerManifestTask`。

如果只想在编辑器开发阶段生成模拟 Manifest，可使用 `BundleGenerator.SimulationPipeline.Build(...)`。该流程只执行 `AnalyzeMainAssetsTask` 和 `BuildBundlerManifestTask`，不会产出真实 AssetBundle 文件。

### 3. 初始化运行时加载器

`BundlerOptions` 的关键配置项包括：

- `Mode`：`AssetDatabase`、`Resources`、`AssetBundle`
- `SearchPaths`：Bundle 搜索根目录数组，按顺序查找
- `AssetBundleCreateAdapter`：自定义 `AssetBundle` 创建方式
- `LogHandler`：自定义日志输出
- `MinAsyncFrameCountOnSimulation` / `MaxAsyncFrameCountOnSimulation`：编辑器模拟异步延迟
- `ListenAddress`：运行时 Profiler 的 JSON-RPC 地址

通常应把热更新目录放在 `SearchPaths` 前面，把内置目录放在后面。

### 4. 运行时加载资源

`IBundler` 提供以下主要 API：

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

同步加载预制体：

```csharp
var prefab = bundler.LoadAsset<GameObject>("Assets/Bundles/Prefabs/Enemy.prefab");
var enemy = prefab.Instantiate();
```

异步加载图片并绑定到 UI：

```csharp
var iconAsset = bundler.LoadAssetAsync<Sprite>("Assets/Bundles/UI/Icons/Sword.png");
yield return iconAsset;
image.SetSprite(iconAsset);
```

异步加载场景：

```csharp
var scene = bundler.LoadSceneAsync("Assets/Bundles/Scene/Battle.unity", LoadSceneMode.Single);
yield return scene;
scene.Activate();
```

### 5. 理解 `Asset` / `AssetAsync` 包装类型

`LoadAsset` / `LoadAssetAsync` 返回的是 `Asset`、`Asset<T>`、`AssetAsync`、`AssetAsync<T>`，不是裸 `UnityEngine.Object`。这些包装类型允许 Bundler 跟踪底层加载器和引用计数。

常用成员包括：

- `GetRawAsset()` / `GetAllRawAssets()`
- `Instantiate()`
- `Retain()` / `Release()`
- `SetTo<TComponent, ...>()`
- `Unload()`
- `IsDone` / `Progress`（异步类型）

内置属性链接扩展包括：`Image.SetSprite(...)`、`Image.SetMaterial(...)`、`Renderer.SetMaterial(...)`、`Renderer.SetSharedMaterial(...)`、`Projector.SetMaterial(...)`、`SpriteRenderer.SetSprite(...)`、`AudioSource.SetClip(...)`。

### 6. Profiler

通过菜单 `Tools/vFrame/Bundler/Profiler` 打开 `BundleProfiler`。Profiler 使用 JSON-RPC 与运行时通信，可查看：

- Loaders
- Pipelines
- Handlers
- Links

运行时监听地址来自 `BundlerOptions.ListenAddress`，默认值为 `http://127.0.0.1:16667/`。

## 架构概览

### Runtime

- `Bundler`：实现 `IBundler` 的外部主入口
- `BundlerManifest`：保存资源到 Bundle 的映射和 Bundle 依赖关系
- `LoadSystem`：调度资源与场景加载
- `LinkSystem`：管理实例化对象和属性链接
- `CollectSystem`：回收无引用资源
- `ProfileSystem`：向编辑器 Profiler 暴露运行时状态

### Editor

- `BundleBuildRules` / `BundleBuildSettings`：构建输入模型
- `BundleGenerator`：构建管线入口
- `BuiltinPipeline`：正式构建流程
- `SimulationPipeline`：模拟构建流程
- `BundleProfiler`：编辑器调试窗口

## 注意事项

- `Bundler.Update()` 必须先于 `Bundler.Collect()` 调用，否则本帧刚完成的资源可能被提前回收。
- `LoadAsset` / `LoadAssetAsync` 返回包装类型，不是原始 Unity 对象；如果绕过包装类型直接持有原始对象，需要自行平衡 `Retain()` / `Release()`。
- 所有需要动态加载的资源都必须命中某条 `MainRules`，否则不会出现在 `BundlerManifest` 中。
- `SearchPaths` 按顺序查找，通常应把补丁目录放前面。
- `LoadAssetWithSubAssets*` 适合 FBX 子资源、Sprite 集合等一个路径对应多个对象的场景。

## License

本项目基于 [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0) 许可协议发布。
