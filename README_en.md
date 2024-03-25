# vFrame.Bundler

[![License](https://img.shields.io/badge/License-Apache%202.0-brightgreen.svg)](LICENSE)

A simple and easy-to-use Unity resource loading tool and AssetBundle generation tool. It uses a unified loading interface for Resources, AssetDatabase, or AssetBundle modes. Resource packaging rules are configured using Json format files, allowing you to set main resource rules and dependency resource grouping rules. It also provides related Editor Profiler tools to facilitate viewing loader time consumption, where resources are referenced, which resources are persistent, etc.

# Folders

* [Features](#features)
* [Editor Integration Instructions](#editor-integration-instructions)
    * [Configuring Packaging Rules](#configuring-packaging-rules)
        * [MainRules](#mainrules)
        * [GroupRules](#grouprules)
    * [Building AssetBundles](#building-assetbundles)
* [Runtime Integration Instructions](#runtime-integration-instructions)
    * [Initialization](#initialization)
    * [Resource Loading and Usage](#resource-loading-and-usage)
    * [Driving Updates and Resource Recycling](#driving-updates-and-resource-recycling)
    * [Destruction](#destruction)
* [Using the Profiler](#using-the-profiler)
* [License](#license)

# Features

1. Packaging methods are controlled by configuration files, offering various resource filtering methods to quickly and conveniently specify the necessary resource files for each AssetBundle.
2. Capable of automatically analyzing resources that are common dependencies of all AssetBundles, **automatically separating shared AssetBundles** to ensure resources are not duplicated.
3. **Provides a unified resource loading interface**, supporting 3 resource loading modes: AssetDatabase, Resources, and AssetBundle.
4. Supports synchronous and asynchronous loading of AssetBundles, and **supports mixed use of synchronous and asynchronous loading**.
5. Supports packaging of scene files, **automatically separating independent scene AssetBundles** without additional configuration rules.
6. Supports automatic collection of Shader resources during packaging, **automatically separating out an independent Shader AssetBundle**.
7. Supports custom file reading methods, facilitating the integration of resource encryption and decryption logic.
8. **Automatically manages AssetBundle reference counting**, no need to worry about the timing of AssetBundle recycling.
9. Provides Profiler interface tools for local or remote debugging of loader information, resource reference count information, current resource usage, etc.

# Editor Integration Instructions

## Configuring Packaging Rules

The configuration file uses a Json format and can be referenced as follows:

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
    { "Include": "(Assets/Bundles/.+/Mesh/).+" },
    { "Include": "(Assets/Bundles/.+/Prefab/).+" },
    { "Include": "(Assets/Bundles/.+/Texture/).+" },
    { "Include": "(Assets/Bundles/.+/Animation/).+" },
    { "Include": "(Assets/Bundles/Scene/.+/).+" }
  ]
}
```

### MainRules

The `MainRules` field represents a set of **main resource** packaging rules. Main resources refer to resources that are dynamically loaded in the code.

Note: **All** dynamically loaded resources must have a rule in `MainRules` that can be hit; otherwise, the resource loading interface call will fail.

The various parameter fields are as follows:

+ `PackType` Resource packaging type, which includes several types:
    + `PackBySingleFile` Generates an AssetBundle for **each single file** matched by rules within a **specified folder** (as many files as there are, that many AB packages are generated)
    + `PackByAllFiles` Generates an AssetBundle for **all files** matched by rules within a **specified folder** (only one AB package is generated)
    + `PackByTopDirectory` Generates an AssetBundle for **all files within the top-level folder (excluding subfolders)** matched by rules within a **specified folder** (only one AB package is generated)
    + `PackByAllDirectory` Generates an AssetBundle for **all folders (including top-level folders)** matched by rules within a **specified folder** (as many folders as there are, that many AB packages are generated)
+ `SearchPath` The **folder path** for searching resources (Note: Must be a folder)
+ `Include` A regular expression for the resource file paths to **include**; only matched resource files will be packaged
+ `Exclude` A regular expression for the resource file paths to **exclude**; matched resource files will not be packaged

### GroupRules

The `GroupRules` field represents a set of **dependency resource** automatic grouping rules. Dependency resources refer to resources that are referenced by multiple main resource AssetBundles. To prevent resources from being duplicated in multiple AssetBundles during packaging, the tool automatically performs dependency analysis and collects all dependencies according to **grouping rules** before packaging.

The various parameter fields are as follows:

* `Include` A regular expression for automatic grouping match rules; matched resource files will be grouped. The expression **must contain one capture group** for naming the dependency AssetBundle.
* `Exclude` A regular expression for matching resources to exclude; matched resource files will not be automatically grouped.

If `GroupRules` does not set any rules, it will fall back to the default rules, separating and packaging each resource into its own AssetBundle.

## Building AssetBundles

Refer to the example:
```c#
var buildRuleJson = File.ReadAllText("Your/Bundle/Build/Rules.json");
var buildRule = BundleBuildRules.FromJson(buildRuleJson);

var buildSettings = new BundleBuildSettings {
    DryRun = false,
    BundlePath = "Your/Bundle/Output/Directory",
    ManifestFileName = "YourManifestFileName.json",
    BuildTarget = buildTarget, // Replace with the actual target platform for packaging
    HashAssetBundlePath = false
};
BundleGenerator.BuiltinPipeline.Build(buildRule, buildSettings);
```

The main steps are as follows:
1. Construct `BundleBuildRules` and `BundleBuildSettings` parameter objects
    * `BundleBuildRules` refers to the configuration file mentioned above [Configuring Packaging Rules](#configuring-packaging-rules), which can be read and parsed from Json.
    * `BundleBuildSettings` refers to the packaging setting parameters, among which:
        * `DryRun` Whether to perform a simulation test process
        * `BundlePath` Output path for AssetBundles
        * `ManifestFileName` The name of the generated Manifest file
        * `BuildTarget` The target platform for building
        * `BundleFormatter` The format string for AssetBundle names, default is fine
        * `SharedBundleFormatter` The format string for shared AssetBundle names, default is fine
        * `SceneBundleFormatter` The format string for scene AssetBundle names, default is fine
        * `HashAssetBundlePath` Whether to hash the generated AssetBundle paths, enabled by default
        * `SeparateShaderBundle` Whether to separate the Shader AssetBundle, enabled by default
        * `SeparatedShaderBundlePath` The path for the separated Shader AssetBundle, default is fine
2. Obtain the pipeline construction object `Pipeline` from `BundleGenerator`, which is divided into:
    * Built-in pipeline `BuiltinPipeline`
    * Programmable pipeline `ScriptalbePipeline` (not yet supported)
    * Simulation pipeline `SimulationPipeline`, used for simulating resource loading in Editor development mode
3. Call the pipeline construction command `Pipeline.Build(buildRule, buildSettings)`

All things done!

# Runtime Integration Instructions

The tool provides a single operation interface: `IBundler`, located in the namespace `vFrame.Bundler`. All resource operation logic must use this interface. The main parts of the interface use are as follows:

## Initialization

Example:

```c#
// Read the resource Manifest file
var manifestFullPath = Path.Combine(Application.streamingAssetsPath, "Bundles/Manifest.json");
var manifestText = File.ReadAllText(manifestFullPath);
var manifest = BundlerManifest.FromJson(manifestText);

var hotfixPath = Path.Combine(Application.persistentDataPath, "Patches");
var searchPath = Path.Combine(Application.streamingAssetsPath, "Bundles");

// Generate Bundler settings
var options = new BundlerOptions {
    Mode = BundlerMode.AssetBundle,
    SearchPaths = new []{ hotfixPath, searchPath },
};
var logLevel = LogLevel.Error;

// Generate the Bundler operation object
_bundler = new Bundler(manifest);
_bundler.SetLogLevel(logLevel);
```

There are two key points in the initialization of Bundler:

1. **Manifest File (BundlerManifest)**

   The Bundler object needs to use the Manifest information generated during pipeline construction to operate AssetBundle loading. Since in the actual project packaging process, most resources need to be placed in the StreamingAssets folder to be read. In **Editor mode**, or on **Windows**, **MacOS**, or **iOS** platforms, you can directly use the `File.ReadAllText` function to read file data. *However*, on the **Android** platform, this is not feasible because the resources in the apk are not extracted after the APP is installed on Android, and other methods are needed, such as packaging the Manifest file into an AssetBundle for loading, or using WWW to load it separately, or using third-party plugins (like [BetterStreamingAssets](https://github.com/gwiazdorrr/BetterStreamingAssets)).

2. **Settings Object (BundlerOptions)**

   All Bundler parameters are provided by `BundlerOptions`, among which:

    + `Mode` Specifies the loading mode, which can be one of the following three, corresponding to Unity's three resource loading methods:
        - BundlerMode.Resources
        - BundlerMode.AssetDatabase
        - BundlerMode.AssetBundle

    + `SearchPaths` Specifies the resource search paths

      Search paths are an important concept, especially for games that require hotfixes. `Bundler` internally maintains a list of root file paths and will try to load resources from the top of the list first. If resource loading fails, it will try the next path until the list is exhausted. For games that require hotfixes, at least two search paths are generally needed:

        1. External storage search path, such as: Path.Combine(Application.persistentDataPath, "Hotfix/Bundles")
        2. Package internal search path, such as: Path.Combine(Application.streamingAssetsPath, "Bundles")

    + `AssetBundleCreateAdapter` Specifies the adapter for creating AB package objects

      In actual applications, sometimes we need to encrypt the generated AssetBundles before packaging or distribution. In this case, directly using Unity's native AB package loading interface `AssetBundle.LoadFromFile(path)` will not work normally. In this case, you can inherit the `IAssetBundleCreateAdapter` interface and implement the AB package loading logic yourself, then pass in the Adapter object.

    + `LogHandler` Specifies the log handler, if needed, you can inherit the `ILogHandler` interface and then pass in the object.
    + `MinAsyncFrameCountOnSimulation` and `MaxAsyncFrameCountOnSimulation` These two parameters are exclusive to AssetDatabase mode. Since there is no asynchronous loading interface for resources in AssetDatabase mode, these two parameters are provided to set the delay interval to simulate "asynchronous" loading callback.
    + `ListenAddress` The connection address used by remote Profile, note that it starts with `http://` or `https://`

## Resource Loading and Usage

Example:

1. Synchronous loading:

   ```csharp
   var asset = _bundler.LoadAsset("Assets/Resources/Prefabs/Balls/BlueBall.prefab", typeof(GameObject));
   var ball = asset.Instantiate() as GameObject;
   ```

2. Asynchronous loading:

   ```csharp
   var assetAsync = _bundler.LoadAssetAsync("Assets/Resources/Prefabs/Balls/BlueBall.prefab", typeof(GameObject));
   yield return assetAsync;
   var ball = assetAsync.Instantiate() as GameObject;
   ```

To unify the loading of resources across different modes, the loading process is defined mainly in two steps:

1. **Loading of the resource object**

   ```csharp
   Asset LoadAsset(string path, Type type);
   AssetAsync LoadAssetAsync(string path, Type type);
   Asset LoadAssetWithSubAssets(string path, Type type);
   AssetAsync LoadAssetWithSubAssetsAsync(string path, Type type);
   
   Asset<T> LoadAsset<T>(string path) where T : Object;
   AssetAsync<T> LoadAssetAsync<T>(string path) where T : Object;
   Asset<T> LoadAssetWithSubAssets<T>(string path) where T : Object;
   AssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object;
   
   Scene LoadScene(string path, LoadSceneMode mode);
   SceneAsync LoadSceneAsync(string path, LoadSceneMode mode);
   ```

   Note that `LoadAsset`/`LoadAssetWithSubAssets`/`LoadAssetAsync`/`LoadAssetWithSubAssetsAsync` do not return the resource type T, but a structure `Asset`/`AssetAsync`/`Asset<T>`/`AssetAsync<T>`. This design is for the unified management of AssetBundle reference counting. Users should not directly operate the resource type T, but should achieve the desired results by extending `Asset`.

2. **Usage of resources**

   Now that we have an `Asset` / `AssetAsync` structure, which already contains the loaded resource. The recommended way to use it is to call the corresponding resource type extension interface, such as:

   Instantiating a GameObject

   ```csharp
   var ball = asset.Instantiate() as GameObject;
   ```

   For example, setting a material

   ```csharp
   var render = ball.GetComponent<Renderer>();
   render.SetMaterial(asset);
   ```

   Or setting a UI Sprite

   ```csharp
   var image = icon.GetComponent<Image>();
   image.SetSprite(asset);
   ```

   Like the `SetMaterial` / `SetSprite` mentioned above, there are no such methods in the corresponding types by default, but we have given them these capabilities through **extensions**. If you find that you are missing a feature you need, you can extend it yourself, it's simple to implement.

   Of course, you can also get the native Object object through the following interfaces in the `Asset` structure:

   ```c#
   Object GetRawAsset()；
   Object[] GetAllRawAssets()；
   ```

   In this case, the reference count needs to be manually maintained. After obtaining `Asset`, you need to call `Retain()` to maintain the reference, and after the resource is no longer needed, you need to call `Release` to release the reference.

## Driving Updates and Resource Recycling

One of the design goals of vFrame.Bundler is that the user never needs to worry about the recycling and release of AssetBundles. To achieve this, the above set of Asset loading and usage processes was created. To drive the progress of asynchronous loading, you need to call the `Update` method of `Bundler` at the right place. Additionally, for AssetBundles to be automatically recycled, you also need to call Bundler's Collect method at the **appropriate time**, as shown below:

```cs
private IEnumerator LoopUpdateBundler() {
    while (true) {
        yield return new WaitForEndOfFrame();
        if (_autoCollect) {
            _bundler.Collect();
        }
        _bundler.Update();
    }
}
```

The timing of the above `Update` and `Collect` calls is required. Consider the following situation:

1. Most of the game's update logic is in the `Update` and `LateUpdate` logic loops (of course, exceptions are not ruled out).

2. If we call `Bundler.Update()` in the `Update` loop, some asynchronous loading requests may be completed in this call, and the `AssetAsync` is in a usable state with a reference count of 0. Two situations may occur:

   a) Game logic Update precedes Bundler Update

   ​	In this case, the completion of asynchronous resource loading occurs after the game logic update, and the reference count does not change.

   b) Bundler Update precedes the game logic Update

   ​	In this case, after the asynchronous resource loading is completed, it can be consumed by the game logic Update in the same frame, and the reference count +1 (provided there is actual use).

3. If you are in the situation described in a above, calling the `Collect` method at the end of the frame **will cause the resources just loaded in that frame to be recycled before they have a chance to be used**.

Therefore, the correct sequence should be one of the following two (essentially the same):

1. Bundler.Update -> Game logic Update uses resources -> Bundler.Collect
2. Game logic Update uses resources -> Bundler.Collect -> Bundler.Update

## Destruction

Before exiting the game, or when restarting the game, it is recommended to execute the Bundler's destruction logic to recycle all resources:

```c#
_bundler.Destroy();
```

# Using the Profiler

The Profiler window can be opened through the Tools -> vFrame -> Profiler menu, and the window looks like this:

![Loaders](https://i.imgur.com/UvCtw0u.png)

![Pipeline](https://i.imgur.com/irOVWQ6.png)

![Handlers](https://i.imgur.com/fxo7sYV.png)

![Links](https://i.imgur.com/ZA4DuMP.png)

# License

Apache License 2.0.