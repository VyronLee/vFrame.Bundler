//------------------------------------------------------------
//        File:  BundleLoaderAsync.cs
//       Brief:  BundleLoaderAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:08
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Utils;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Loaders
{
    public class BundleLoaderAsync : BundleLoaderBase, IAsync
    {
        private AssetBundleCreateRequest _bundleLoadRequest;

        public override AssetBundle AssetBundle {
            get {
                if (!_assetBundle) {
                    throw new BundleLoadNotFinishedException(
                        string.Format("Bundle Loader has not finished: {0}", _path));
                }
                return _assetBundle;
            }
        }

        public float Progress {
            get {
                if (_bundleLoadRequest == null)
                    return 0f;

                if (!_bundleLoadRequest.isDone)
                    return _bundleLoadRequest.progress;

                return 1f;
            }
        }

        public IEnumerator Await() {
            if (_assetBundle)
                yield break;

            Profiler.BeginSample("BundleLoaderAsync:Await");

            Logger.LogInfo("Bundle load async start: {0}", _path);

            if (_bundleLoadRequest == null) {
                _bundleLoadRequest = CreateBundleLoadRequest();
            }

            yield return _bundleLoadRequest;

            Logger.LogInfo("Bundle load request finished: {0}", _path);

            LoadAndCache(_bundleLoadRequest);

            Logger.LogInfo("AssetBundle asynchronously loading finished, path: {0}", _path);

            Profiler.EndSample();
        }

        public void ForceLoadComplete() {
            if (_assetBundle) {
                return;
            }

            Logger.LogInfo("Force load asset bundle: {0}", _path);

            if (null == _bundleLoadRequest) {
                Logger.LogInfo("Force load asset bundle, but load request not exist, create it: {0}", _path);
                _bundleLoadRequest = CreateBundleLoadRequest();
            }
            LoadAndCache(_bundleLoadRequest);
        }

        private void LoadAndCache(AssetBundleCreateRequest assetBundleCreateRequest) {
            _assetBundle = assetBundleCreateRequest.assetBundle;

            if (!AssetBundleCache.ContainsKey(_path)) {
                AssetBundleCache.Add(_path, _assetBundle);
                Logger.LogInfo("Add assetbundle to cache: {0}", _path);
            }

            IsLoading = false;
            IsDone = true;
        }

        protected override void OnLoadProcess() {
            Logger.LogInfo("Start asynchronously loading process: {0}", _path);

            IsLoading = true;
        }

        protected override void OnUnloadProcess() {

        }

        private AssetBundleCreateRequest CreateBundleLoadRequest() {
            Logger.LogInfo("Bundle load request does not exist, create it from file: {0}", _path);

            foreach (var basePath in _searchPaths) {
                var path = PathUtility.Combine(basePath, _path);
                try {
                    // Avoid throwing error messages.
                    if (PathUtility.IsFileInPersistentDataPath(path) && !File.Exists(path)) {
                        Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        continue;
                    }

                    Profiler.BeginSample("BundleLoaderAsync:CreateBuiltinBundleLoadRequest - AssetBundle.LoadFromFileAsync");
                    var bundleLoadRequest = LoadAssetBundleAsync(path);
                    Profiler.EndSample();

                    if (bundleLoadRequest != null)
                        return bundleLoadRequest;

                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
                catch {
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
            }

            throw new BundleLoadFailedException("Cannot load assetbundle: " + _path);
        }

        protected virtual AssetBundleCreateRequest LoadAssetBundleAsync(string path) {
            return AssetBundle.LoadFromFileAsync(path);
        }
    }
}