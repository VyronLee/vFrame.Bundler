# vFrame.Bundler

[![License](https://img.shields.io/badge/License-Apache%202.0-brightgreen.svg)](LICENSE)

[English Version (Power by ChatGPT)](./README_en.md)

一款简单易用的 Unity 资源加载工具以及 AssetBundle 生成工具。无论是 Resources，AssetDatabase 还是 AssetBundle 模式，均使用统一的加载接口。使用 Json 格式文件配置资源打包规则，可设置主资源规则以及依赖资源分组规则，并提供相关的Editor Profiler工具，方便查看加载器耗时，资源被引用的地方，常驻资源有哪些，等等。

# 文件夹

* [特点](#特点)
* [Editor接入说明](#Editor接入说明)
    * [配置打包规则](#配置打包规则)
      * [MainRules](#MainRules ) 
      * [GroupRules](#GroupRules)
    * [构建AssetBundle](#构建AssetBundle)
* [Runtime接入说明](#Runtime接入说明)
    * [初始化](#初始化)
    * [资源加载及使用](#资源加载及使用)
    * [驱动更新以及资源的回收](#驱动更新以及资源的回收)
    * [销毁](#销毁)
* [Profiler的使用](#Profiler的使用)
* [License](#license)



# 特点

1. 打包方式由配置文件控制，提供多种资源过滤方式，可方便、快速地指定每个 AssetBundle 需要的资源文件
2. 能够自动分析所有 AssetBundle 共同依赖的资源，**自动分离生成共享 AssetBundle**，保证资源不重复
3. **提供统一资源加载接口**，支持3种资源加载模式：AssetDatabase，Resources 以及 AssetBundle
4. 支持 AssetBundle 同步加载以及异步加载，而且**支持同步、异步加载混用**
5. 支持场景文件打包，会**自动分离出独立的场景 AssetBundle**，无需另外配置规则
6. 支持打包过程自动收集 Shader 资源，会**自动分离出独立的 Shader AssetBundle**
7. 支持自定义文件读取方式，方便自行接入资源加解密逻辑
8. **自动管理 AssetBundle 引用计数**，无需操心 AssetBundle 的回收时机
9. 提供 Profiler 界面工具，可本地或远程调试相关加载器信息，资源引用计数信息，当前使用资源信息等

# Editor接入说明

## 配置打包规则

配置文件使用 Json 格式，可参照如下示例：

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

字段 `MainRules` 为**主资源**打包规则集。所谓主资源，是指用于代码中动态加载的资源。

注意：**所有**需要动态加载的资源，必须在`MainRules`中含有一条规则可以命中到，否则调用资源加载接口时会失败。

各参数字段如下：

+ `PackType` 资源打包类型，分为以下几种类型：
  + `PackBySingleFile` 在**指定文件夹内**使用规则匹配到的**单个文件**生成 AssetBundle（多少个文件就生成多少个 AB 包）
  + `PackByAllFiles` 在**指定文件夹内**使用规则匹配到的**所有文件**生成 AssetBundle（仅生成一个 AB 包）
  + `PackByTopDirectory` 在**指定文件夹内**使用规则匹配到的**顶级文件夹（不包含子文件夹）内的所有文件**生成 AssetBundle（仅生成一个 AB 包）
  + `PackByAllDirectory` 在**指定文件夹内**使用规则匹配到的**所有文件夹（包含顶级文件夹）**生成 AssetBundle（多少个文件夹就生成多少个 AB 包）
+ `SearchPath` 资源搜索的**文件夹路径**（注意：必须是文件夹）
+ `Include` **包含**的资源文件路径正则表达式，命中的资源文件才会进包
+ `Exclude` **排除**的资源文件路径正则表达式，命中的资源文件不会进包

### GroupRules

字段 `GroupRules` 为**依赖资源**自动分组规则集。所谓依赖资源，是指被多个主资源 AssetBundle 引用的资源。为了防止打包时资源被重复打进多个 AssetBundle 中，本工具会自动进行依赖项分析，并把所有依赖项按照**分组规则**进行收集然后打包。

各参数字段如下：

* `Include` 自动分组匹配规则正则表达式，命中的资源文件会进入分组。式中**必须含有一个捕获组**，用作于依赖项 AssetBundle 的命名
* `Exclude` 排除资源匹配规则正则表达式，命中的资源文件不会自动分组

如果`GroupRules`没有设置任何规则，会自行回落到默认规则，按照一个资源一个 AssetBundle 的方式进行分离打包

## 构建AssetBundle

参照示例：
```c#
var buildRuleJson = File.ReadAllText("Your/Bundle/Build/Rules.json");
var buildRule = BundleBuildRules.FromJson(buildRuleJson);

var buildSettings = new BundleBuildSettings {
    DryRun = false,
    BundlePath = "Your/Bundle/Output/Directory",
    ManifestFileName = "YourManifestFileName.json",
    BuildTarget = buildTarget, // 替换为实际打包的目标平台
    HashAssetBundlePath = false
};
BundleGenerator.BuiltinPipeline.Build(buildRule, buildSettings);
```

主要分为以下3个步骤：
1. 构造 `BundleBuildRules` 以及 `BundleBuildSettings`参数对象
   * `BundleBuildRules`为上述[配置打包规则](#配置打包规则)中所述配置文件，读入然后进行Json解析即可
   * `BundleBuildSettings`为打包设置参数，其中：
     * `DryRun` 是否执行模拟测试流程
     * `BundlePath` AssetBundle 输出路径
     * `ManifestFileName` 生成的 Manifest 文件名
     * `BuildTarget` 构建的目标平台
     * `BundleFormatter` AssetBundle 名字格式化字符串，默认即可
     * `SharedBundleFormatter` 共享的 AssetBundle 名字格式化字符串，默认即可
     * `SceneBundleFormatter` 场景 AssetBundle 名字格式化字符串，默认即可
     * `HashAssetBundlePath`  生成的 AssetBundle 路径是否哈希化，默认开启
     * `SeparateShaderBundle` 是否分离 Shader AssetBundle，默认开启
     * `SeparatedShaderBundlePath` 分离的 Shader AssetBundle 路径，默认即可
2. 从 `BundleGenerator` 中获取构造管线对象`Pipeline`，分为：
   * 内置管线 `BuiltinPipeline`
   * 可编程管线 `ScriptalbePipeline` （暂未支持）
   * 模拟管线 `SimulationPipeline`，用于Editor开发模式下模拟加载资源
3. 调用管线构造指令 `Pipeline.Build(buildRule, buildSettings)`

All things done!

# Runtime接入说明

工具提供一个唯一操作接口：```IBundler```，该接口位于命名空间 ``` vFrame.Bundler```下，所有资源操作逻辑必须使用该接口进行。接口使用主要有以下部分：

## 初始化

示例：

```c#
// 读取资源 Manifest 文件
var manifestFullPath = Path.Combine(Application.streamingAssetsPath, "Bundles/Manifest.json");
var manifestText = File.ReadAllText(manifestFullPath);
var manifest = BundlerManifest.FromJson(manifestText);

var hotfixPath = Path.Combine(Application.persistentDataPath, "Patches");
var searchPath = Path.Combine(Application.streamingAssetsPath, "Bundles");

// 生成 Bundler 设置项
var options = new BundlerOptions {
    Mode = BundlerMode.AssetBundle,
    SearchPaths = new []{ hotfixPath, searchPath },
};
var logLevel = LogLevel.Error;

// 生成 Bundler 操作对象
_bundler = new Bundler(manifest);
_bundler.SetLogLevel(logLevel);
```

初始化 Bundler 有2个要点：

1. **Manifest 文件（BundlerManifest）**

    Bundler 对象需要使用管线构造时生成的 Manifest 信息操作 AssetBundle 的加载。由于实际项目打包过程中，一般需要把大部分资源放到 StreamingAssets 文件夹下才能读取。在 **Editor 模式**下，或者 **Windows**、**MacOS**、**iOS**平台下，可以直接使用 ```File.ReadAllText``` 函数进行文件数据的读入，*但是*在 **Android** 平台下却不可行，因为 Android 平台下 APP 安装后 apk 中的资源并不会解压出来，需要使用其他的方式，如：把 Manifest 文件单独打包成 AssetBundle 再进行加载，或者使用 WWW 单独加载 ，或者使用第三方插件（如 [BetterStreamingAssets](https://github.com/gwiazdorrr/BetterStreamingAssets)）。

2. **设置项对象（BundlerOptions）**

    Bundler 参数都由该`BundlerOptions`提供，其中：
    
    + `Mode` 指定加载模式，可为以下3种之一，分别对应 Unity 的3种资源加载方式：
      - BundlerMode.Resources
      - BundlerMode.AssetDatabase
      - BundlerMode.AssetBundle
    
    + `SearchPaths` 指定资源搜索路径

        搜索路径是一个很重要的概念，尤其对于需要热更的游戏。`Bundler` 内部维护一个文件路径的根列表，会优先从列表头部开始，尝试加载资源。如果资源加载失败，再使用下一个路径尝试加载，直至列表使用完。对于需要热更的游戏，一般至少需要设置2个搜索路径：

        1. 外存储搜索路径，如：Path.Combine(Application.persistentDataPath, "Hotfix/Bundles")
        2. 包体内搜索路径，如：Path.Combine(Application.streamingAssetsPath, "Bundles")

    + `AssetBundleCreateAdapter` 指定创建AB包对象的适配器

        实际应用中，有时候我们需要对生成的 AssetBundle 进行加密后再打包或者分发，这种情况下直接使用 Unity 原生的 AB 包加载接口`AssetBundle.LoadFromFile(path)` 是无法正常加载的，这种情况下可以继承`IAssetBundleCreateAdapter`接口自行实现 AB 包的加载逻辑，然后把 Adapter 对象传入即可

    + `LogHandler` 指定日志处理器，如有需要可以继承接口`ILogHandler`然后传入对象即可
    + `MinAsyncFrameCountOnSimulation` 以及 `MaxAsyncFrameCountOnSimulation` 这两个参数为 AssetDatabase 模式专用，由于 AssetDatabase 模式下没有资源的异步加载接口，因此提供两个参数用于设置延迟区间，模拟“异步”加载的延迟回调
    + `ListenAddress` 远程 Profile 使用的连接地址，注意是`http://`或者`https://`开头的地址

## 资源加载及使用

示例：

1. 同步加载：

   ```csharp
   var asset = _bundler.LoadAsset("Assets/Resources/Prefabs/Balls/BlueBall.prefab", typeof(GameObject));
   var ball = asset.Instantiate() as GameObject;
   ```
   
2. 异步加载：

   ```csharp
   var assetAsync = _bundler.LoadAssetAsync("Assets/Resources/Prefabs/Balls/BlueBall.prefab", typeof(GameObject));
   yield return assetAsync;
   var ball = assetAsync.Instantiate() as GameObject;
   ```

为了统一各个模式下资源的加载，现定义加载流程主要有2个步骤：

1. **资源对象的加载**

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

   注意到 `LoadAsset`/ `LoadAssetWithSubAssets`/`LoadAssetAsync`/`LoadAssetWithSubAssetsAsync`返回的并不是资源类型 T，而是一个结构体 `Asset`/ `AssetAsync`/`Asset<T>`/`AssetAsync<T>`。这么设计是为了 AssetBundle 的计数统一管理，使用者一般不应该直接操作资源类型 T，而是通过设计扩展 `Asset`来达到相应的目的。

   

2. **资源的使用**

   目前我们拿到了一个 `Asset` / `AssetAsync` 结构，该结构中已经包含了加载后的实际资源。建议的使用方式为调用对应资源类型的扩展接口，如:

   实例化一个 GameObject

   ```csharp
   var ball = asset.Instantiate() as GameObject;
   ```

   再如：设置一个材质

   ```csharp
   var render = ball.GetComponent<Renderer>();
   render.SetMaterial(asset);
   ```

   又如：设置一个UI Sprite

   ```csharp
   var image = icon.GetComponent<Image>();
   image.SetSprite(asset);
   ```

   像上面的 `SetMaterial` / `SetSprite`，对应的类型中默认是没有这些方法的，而我们通过**扩展**的方式让它们具备了这种能力。如果发现缺少了你所需要的功能，可以自行扩展，简单就能实现。

   当然，你也可以通过`Asset`结构中以下接口来获取原生 Object 对象：

   ```c#
   Object GetRawAsset()；
   Object[] GetAllRawAssets()；
   ```

   这种情况下引用计数需要手动维护。在获取到 `Asset`后需要自行调用 `Retain()`方法来保持引用，资源使用完后（不再需要的时候）需要自行调用`Release`方法来释放引用。

## 驱动更新以及资源的回收

vFrame.Bundler 的设计目的之一是使用者永远不需要操心 AssetBundle 的回收与释放。为了达到这个目的，于是产生了上面的一套  Asset 的加载以及使用流程。为了驱动异步加载流程的进行，需要在合适的地方调用 `Bundler` 的 `Update` 方法。此外，为了 AssetBundle 能够自动回收，也需要在**合适的时机**调用 Bundler 的 Collect 方法，如下所示：

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

以上 `Update` 跟 `Collect` 的调用时序是有要求的。考虑如下情况：

1. 游戏内绝大部分的更新逻辑都处于 `Update` 以及  `LateUpdate` 这两个逻辑循环中（当然不排除有例外情况）

2. 如果我们在 `Update` 循环中调用 `Bundler.Update()`，此时部分的异步加载请求可能会在这一次调用中完成，该`AssetAsync`处于可用状态，引用计数为0。可能会发生以下两种情况：

   a) 游戏逻辑 Update 优先于 Bundler Update

   ​	这种情况下，异步资源的加载完成发生在游戏逻辑更新后，引用计数不变

   b) Bundler Update 优先于游戏逻辑 Update

   ​	这种情况下，异步资源加载完成后能在接下来同一帧内被游戏逻辑 Update 消耗掉，引用计数+1（前提是有实际使用）

3. 如果处于上述 a 所述情况，在该帧末尾调用 `Collect` 方法，**会导致这一帧刚加载完的资源，还没来得及使用就被回收掉**

因此，正确的时序应该是以下两种之一（本质上是一样的）：

1. Bundler.Update -> 游戏逻辑 Update 使用资源 -> Bundler.Collect
2. 游戏逻辑 Update 使用资源 -> Bundler.Collect -> Bundler.Update 

## 销毁

在退出游戏前，或者重启游戏时，建议执行 Bundler 的销毁逻辑，以回收所有资源：

```c#
_bundler.Destroy();
```



# Profiler的使用

Profiler 窗口可通过 Tools -> vFrame -> Profiler 菜单打开，窗口样例如下：

![Loaders](https://i.imgur.com/UvCtw0u.png)

![Pipeline](https://i.imgur.com/irOVWQ6.png)

![Handlers](https://i.imgur.com/fxo7sYV.png)

![Links](https://i.imgur.com/ZA4DuMP.png)

# License

Apache License 2.0.