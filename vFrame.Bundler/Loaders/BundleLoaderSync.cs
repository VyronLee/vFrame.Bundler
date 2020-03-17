//------------------------------------------------------------
//        File:  BundleLoaderSync.cs
//       Brief:  BundleLoaderSync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:09
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.IO;
using UnityEngine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Utils;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Loaders
{
    public class BundleLoaderSync : BundleLoaderBase
    {
        protected override bool LoadProcess()
        {
            Logs.Logger.LogInfo("Start synchronously loading process: {0}", _path);

            foreach (var basePath in _searchPaths)
            {
                var path = Path.Combine(basePath, _path);
                path = PathUtility.NormalizePath(path);

                try
                {
                    // Avoid throwing error messages.
                    if (PathUtility.IsFileInPersistentDataPath(path) && !File.Exists(path))
                    {
                        IsDone = false;
                        Logs.Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        continue;
                    }

                    if (BundlerCustomSettings.kCustomFileReader == null)
                    {
                        _assetBundle = AssetBundle.LoadFromFile(path);
                    }
                    else
                    {
//#if UNITY_5
                        var bytes = BundlerCustomSettings.kCustomFileReader.ReadAllBytes(path);
                        _assetBundle = AssetBundle.LoadFromMemory(bytes);
//#else                 // Load form stream will always crash at this time
//                      _fileStream = BundlerCustomSettings.kCustomFileReader.GetStream(path);
//                      _assetBundle = AssetBundle.LoadFromStream(_fileStream);
//#endif
                    }

                    if (_assetBundle)
                    {
                        IsDone = true;
                        Logs.Logger.LogInfo("AssetBundle synchronously loading finished, path: {0}", _path);
                        break;
                    }

                    IsDone = false;
                    Logs.Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
                catch (System.Exception)
                {
                    Logs.Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
            }

            if (!IsDone)
                throw new BundleNotFoundException("AssetBundle synchronously loading failed, path: " + _path);

            Logs.Logger.LogInfo("End synchronously loading process: {0}", _path);

            return IsDone;
        }
    }
}