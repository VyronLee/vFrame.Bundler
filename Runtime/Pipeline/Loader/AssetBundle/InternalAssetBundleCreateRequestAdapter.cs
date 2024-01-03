// ------------------------------------------------------------
//         File: InternalAssetBundleCreateRequestAdapter.cs
//        Brief: InternalAssetBundleCreateRequestAdapter.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 16:2
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace vFrame.Bundler
{
    internal class InternalAssetBundleCreateRequestAdapter : BundlerObject, IAssetBundleCreateRequestAdapter
    {
        public InternalAssetBundleCreateRequestAdapter(BundlerContexts bundlerContexts) : base(bundlerContexts) {

        }

        protected override void OnDestroy() {

        }

        public AssetBundleCreateRequest CreateRequestAsync(string bundlePath) {
            Facade.GetSystem<LogSystem>().LogDebug("Create AssetBundleCreateRequest: {0}", bundlePath);

            var searchPaths = BundlerContexts.Options.SearchPaths;
            foreach (var basePath in searchPaths) {
                var path = PathUtils.Combine(basePath, bundlePath);
                if (PathUtils.IsFileInPersistentDataPath(path) && !File.Exists(path)) {
                    continue;
                }

                Profiler.BeginSample("InternalAssetBundleCreateRequestAdapter - LoadFromFileAsync");
                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
                Profiler.EndSample();

                if (bundleLoadRequest == null) {
                    continue;
                }
                Facade.GetSystem<LogSystem>().LogDebug("AssetBundleCreateRequest created: {0}", path);
                return bundleLoadRequest;
            }
            return null;
        }

        public AssetBundle CreateRequest(string bundlePath) {
            Facade.GetSystem<LogSystem>().LogDebug("Load AssetBundle: {0}", bundlePath);

            var searchPaths = BundlerContexts.Options.SearchPaths;
            foreach (var basePath in searchPaths) {
                var path = PathUtils.Combine(basePath, bundlePath);
                if (PathUtils.IsFileInPersistentDataPath(path) && !File.Exists(path)) {
                    continue;
                }

                Profiler.BeginSample("InternalAssetBundleCreateRequestAdapter - LoadFromFile");
                var assetBundle = AssetBundle.LoadFromFile(path);
                Profiler.EndSample();

                if (assetBundle == null) {
                    continue;
                }
                Facade.GetSystem<LogSystem>().LogDebug("AssetBundle loaded: {0}", path);
                return assetBundle;
            }
            return null;
        }
    }
}