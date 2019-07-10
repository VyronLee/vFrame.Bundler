# vBundler

[![License](https://img.shields.io/badge/License-Apache%202.0-brightgreen.svg)](LICENSE)

一款简单易用的 Unity AssetBundle 生成工具。

# 目录

   * [vBundler](#vbundler)
   * [目录](#目录)
   * [特点](#特点)
   * [使用方式](#使用方式)
      * [配置打包规则](#配置打包规则)
      * [工具使用流程](#工具使用流程)
   * [接入说明](#接入说明)
      * [初始化](#初始化)
      * [资源加载及使用](#资源加载及使用)
      * [资源的回收](#资源的回收)
   * [License](#license)



# 特点

1. 打包方式由配置文件控制，提供多种资源过滤方式，可方便、快速地指定每个 AssetBundle 需要的资源文件
2. 能够自动分析所有 AssetBundle 共同依赖的资源，自动分离生成共享 AssetBundle，保证资源不重复
3. 提供统一资源加载接口，支持从 Bundle 模式读取以及 Resource 模式读取（ Resource 模式支持直接使用 AssetDatabase 方式 ）， 可在两个模式间无缝切换
4. 支持 AssetBundle 同步加载以及异步加载
5. 支持场景文件打包
6. 支持自定义文件读取方式，方便接入资源加解密逻辑
7. 自动管理 AssetBundle 引用计数，无需操心 AssetBundle 的回收时机
8. 提供 Editor 界面，可供开发模式下查看 AssetBundle 加载列表以及相关资源引用（TODO）

# 使用方式

可直接查看 [Example](https://github.com/VyronLee/vBundlerExample) 工程，或者根据下面的描述进行操作

## 配置打包规则

配置文件默认使用名字 `BundleRules.json` ，直接存放在 Assets 目录下即可。

配置文件使用 Json 格式：

```json
{
    "rules": [
        {
            "packType": 1,
            "path": "Resources/Prefabs",
            "searchPattern": "*.prefab",
            "depth": 1,
            "excludePattern": "exclude.prefab",
            "shared": false
        }
    ]
}
```

字段 `rules` 为使用的***一组***打包规则，每个规则 Object 包含有字段：

+ **packType** [int] 资源匹配过滤规则类型 ，有3类：
  + 1 => PackByFile，使用 searchPattern 匹配到的单个文件名生成 AssetBundle
  + 2 => PackByDirectory，使用指定的目录名生成 AssetBundle ，仅 searchPattern 匹配到的文件会打进包里
  + 3 => PackBySubDirectory，使用指定目录下的子目录名生成 AssetBundle，仅 searchPattern 匹配到的文件会打进包里
+ **searchPattern** [string] 资源搜索匹配规格，为**通配符**表达式，作用见 packType 定义
+ **excludePattern** [string] 资源搜索排除规则，为**正则**表达式，匹配成功的资源会过滤掉，需要配合 searchPattern 使用
+ **path** [string] 资源搜索路径，配合 searchPattern 使用
+ **depth** [int] 资源搜索路径深度，只有  `packType == 3` （使用子目录匹配规则）时才生效，该字段有两个值：
  + 0，只搜索顶级目录，该值为**默认值**
  + 1，递归搜索所有目录
+ **shared** [bool] 是否强制指定为共享包，默认值 false，一般用于**图集**的打包。在开发阶段，例如 UI 预设的制作，一般会依赖多个图集（共享的图集，如通用按钮；以及非共享图集，如各个功能模块的特有 UI 装饰）。由于工具会自动分离共享包，所以在这类情况下，图集的打包会被破坏掉，分离到多个 AssetBundle 中，造成资源的重复，因此需要强行指定这些图集为同一个 AssetBundle（如：共享的图集）

## 工具使用流程

配置规则文件设计好后，按照如下步骤进行操作：

1. 点击 Assets -> vBundler -> Generate Manifest，会自动分析资源依赖关系，然后生成 Manifest.json 文件。该文件主要用于下面 AssetBundle 的生成规则设置，以及作为运行时的加载配置文件。
2. 点击 Assets -> vBundler -> Generate AssetBundle，选择需要的目标平台，然后等待 AssetBundle 的生成即可。

当然，也可以直接通过 API 调用，方便持续集成的接入。主要 API 有：

1. Generate Manifest:

   ```csharp
   BundlerMenu.GenerateManifest();
   ```

2. Generate AssetBundle:

   ```csharp
   BundlerMenu.GenerateAssetBundlesForWindows();
   // BundlerMenu.GenerateAssetBundlesForMacOS();
   // BundlerMenu.GenerateAssetBundlesForIOS();
   // BundlerMenu.GenerateAssetBundlesForAndroid();
   ```

All things done!

# 接入说明

工具提供一个唯一操作接口：```IBundler```，该接口位于命名空间 ``` vBundler.Interface```下。所有资源操作逻辑必须使用该接口进行。接口使用主要分为3个部分：

## 初始化

示例：

```csharp
// 根据 Preference 设置读取当前的加载模式以及日志等级
var bundleMode = false;
var logLevel = Logger.LogLevel.ERROR;
#if UNITY_EDITOR
bundleMode = EditorPrefs.GetBool("vBundlerModePreferenceKey", false);
logLevel = EditorPrefs.GetInt("vBundlerLogLevelPreferenceKey", 0) + 1;
#endif

// 读取资源 Manifest 文件
var manifestFullPath = Path.Combine(Application.streamingAssetsPath, "Bundles/Manifest.json");
var manifestText = File.ReadAllText(manifestFullPath);
var manifest = JsonUtility.FromJson<BundlerManifest>(manifestText);
var searchPath = Path.Combine(Application.streamingAssetsPath, "Bundles");

// 生成 Bundler 操作对象，以及设置搜索路径、加载模式，日志等级
_vBundler = new Bundler(manifest);
_vBundler.AddSearchPath(searchPath);
_vBundler.SetMode(bundleMode ? BundleModeType.Bundle : BundleModeType.Resource);
_vBundler.SetLogLevel(logLevel);
```

初始化 Bundler 对象有3个要点：

- **Manifest 文件的读取**

  Bundler 对象需要引用到上面工具生成的 Manifest 信息操作 AssetBundle 的加载。由于实际项目打包过程中，一般需要把大部分资源放到 StreamingAssets 目录下才能读取。在 **Editor 模式**下，或者 **Windows**、**MacOS**、**iOS**平台下，可以直接使用 ```File.ReadAllText``` 函数进行文件数据的读入，*但是*在 **Android** 平台下却不可行，因为 Android 平台下 APP 安装后 apk 中的资源并不会解压出来，需要使用其他的方式，如：把 Manifest 文件单独打包成 AssetBundle 再进行加载，或者使用 WWW 单独加载 ，或者使用第三方插件（如 [BetterStreamingAssets](https://github.com/gwiazdorrr/BetterStreamingAssets)）。

- **设置加载模式，有两种：**

  - BundleModeType.Bundle 使用 Bundle 模式，会从 AssetBundle 中读取资源

  - BundleModeType.Resource 使用 Resource 模式，会调用 ```Resouces``` 类直接从工程中读取资源；如果想使用 ```AssetDatabase``` 方式，请设置

    ``` BundlerCustomSettings.kUseAssetDatabaseInsteadOfResources = true; ```

- **设置搜索路径**

  搜索路径是一个很重要的概念，尤其对于需要热更的游戏。vBundler 内部维护一个文件路径的 Root 列表，会优先从列表头部开始，尝试加载资源。如果资源加载失败，再使用下一个路径尝试加载，直至列表使用完。对于需要热更的游戏，一般至少需要设置2个搜索路径：

  1. 外存储搜索路径，如：Path.Combine(Application.persistentDataPath, "Hotfix/Bundles")
  2. 包体内搜索路径，如：Path.Combine(Application.streamingAssetsPath, "Bundles")

## 资源加载及使用

示例：

1. 同步加载：

   ```csharp
   var request = _vBundler.Load("Assets/Resources/Prefabs/Balls/BlueBall.prefab");
   var asset = request.GetAsset<GameObject>();
   var ball = asset.InstantiateGameObject();
   ```

2. 异步加载：

   ```csharp
   var request = _vBundler.LoadAsync("Assets/Resources/Prefabs/Balls/BlueBall.prefab");
   yield return request;
   var asset = request.GetAssetAsync<GameObject>();
   yield return asset;
   var ball = asset.InstantiateGameObject();
   ```

为了统一各个模式下资源的加载，现定义加载流程主要有3个步骤：

1. **文件的加载**

   该“文件”是一个概念上的文件，开发过程中是真实存在的，但实际包体中未必有（因为可能在 AssetBundle 中了）。主要涉及函数有：

   ```csharp
   ILoadRequest Load(string path);
   ILoadRequestAsync LoadAsync(string path);
   ```

   调用 Load / LoadAsync 后会返回一个加载请求，下面的资源获取是通过该加载请求操作的。

2. **资源的获取**

   从上面步骤中我们拿到一个 ILoadRequest 后，即可取出相关资源了，主要涉及函数有：

   ```csharp
   IAsset GetAsset<T>() where T: Object;
   IAsset GetAsset(Type type);
   IScene GetScene(LoadSceneMode mode);
   
   IAssetAsync GetAssetAsync<T>() where T : Object;
   IAssetAsync GetAssetAsync(Type type);
   ISceneAsync GetSceneAsync(LoadSceneMode mode);
   ```

   注意到 GetAsset / GetAssetAsync 返回的并不是资源类型 T，而是一个接口 IAsset / IScene。这么设计是为了 AssetBundle 的计数统一管理，使用者一般不应该直接操作资源类型 T，而是通过设计扩展 IAsset 来达到相应的目的。

3. **资源的使用**

   目前我们拿到了一个 IAsset 接口，该接口中已经包含了加载后的实际资源。唯一的使用方式为调用对应资源类型的扩展接口，如:

   实例化一个 GameObject

   ```csharp
   var ball = asset.InstantiateGameObject();
   ```

   销毁一个 GameObject

   ```csharp
   asset.DestroyGameObject(ball);
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

   像上面的 SetMaterial / SetSprite，对应的类型中默认是没有这些方法的，而我们通过**扩展**的方式让它们具备了这种能力。vBundler 中所有的扩展在 ``` vBundler.Extension ``` 命名空间下，可在代码中查看支持的实际功能。如果发现缺少了你所需要的功能，可以自行扩展，简单就能实现。

## 资源的回收

vBundler 的设计目的之一是使用者永远不需要操心 AssetBundle 的回收与释放。为了达到这个目的，于是产生了上面一套的 ILoadRequest 以及 IAsset 流程。为了驱动 vBundler 自行回收无用的 AssetBundler，还需要在相应的地方进行以下代码调用：

```csharp
private void Update()
{
    _vBundler.Collect();
}
```

一般在 Update 函数中调用即可。如果不需要实时的资源回收，可以在场景切换后自行调用 Collect 方法。

另外特别说明，由于 Unity 的消息机制，会导致未曾激活过的 GameObject 无法接收消息（*OnDestroy will only be called on game objects that have previously been active.*）。因此在场景切换后，为了保证引用计数的正确性，还需要进行以下调用：

```csharp
_vBundler.DeepCollect();
```

该函数会进行资源有效性的深度检测，会消耗一定的性能，因此不宜频繁调用。

# License

Apache License 2.0.