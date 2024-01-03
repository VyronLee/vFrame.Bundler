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
using UnityEngine.Profiling;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Utils;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Loaders
{
    public class BundleLoaderSync : BundleLoaderBase
    {
        protected override void OnLoadProcess() {
            UnityEngine.Logger.LogInfo("Start synchronously loading process: {0}", _path);

            Profiler.BeginSample("BundleLoaderSync:LoadProcess");
            foreach (var basePath in _searchPaths) {
                var path = PathUtility.Combine(basePath, _path);
                // Avoid throwing error messages.
                if (PathUtility.IsFileInPersistentDataPath(path) && !File.Exists(path)) {
                    IsDone = false;
                    UnityEngine.Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                    continue;
                }

                Profiler.BeginSample("BundleLoader:LoadProcess - AssetBundle.LoadFromFile");
                _assetBundle = LoadAssetBundle(path);
                Profiler.EndSample();

                if (_assetBundle) {
                    IsDone = true;
                    UnityEngine.Logger.LogInfo("AssetBundle synchronously loading finished, path: {0}", _path);
                    break;
                }

                IsDone = false;
                UnityEngine.Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
            }

            if (!IsDone) {
                throw new BundleNotFoundException(
                    string.Format("AssetBundle synchronously loading failed: {0}", this));
            }

            UnityEngine.Logger.LogInfo("End synchronously loading process: {0}", _path);

            Profiler.EndSample();
        }

        protected override void OnUnloadProcess() {

        }

        protected virtual AssetBundle LoadAssetBundle(string path) {
            return AssetBundle.LoadFromFile(path);
        }
    }
}