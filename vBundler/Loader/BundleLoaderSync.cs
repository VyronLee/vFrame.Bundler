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
using vBundler.Exception;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Loader
{
    public class BundleLoaderSync : BundleLoaderBase
    {
        protected override bool LoadProcess()
        {
            Logger.LogInfo("Start synchronously loading process: {0}", _path);

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
                        Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        continue;
                    }

                    if (BundlerCustomSettings.kCustomFileReader == null)
                    {
                        _assetBundle = AssetBundle.LoadFromFile(path);
                    }
                    else
                    {
                        var bytes = BundlerCustomSettings.kCustomFileReader.ReadAllBytes(path);
                        _assetBundle = AssetBundle.LoadFromMemory(bytes);
                    }

                    if (_assetBundle)
                    {
                        IsDone = true;
                        Logger.LogInfo("AssetBundle synchronously loading finished, path: {0}", _path);
                        break;
                    }

                    IsDone = false;
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
                catch (System.Exception)
                {
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
            }

            if (!IsDone)
                throw new BundleNotFoundException("AssetBundle synchronously loading failed, path: " + _path);

            Logger.LogInfo("End synchronously loading process: {0}", _path);

            return IsDone;
        }
    }
}