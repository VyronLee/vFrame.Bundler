// ------------------------------------------------------------
//         File: AssetBundleLoaderGroup.cs
//        Brief: AssetBundleLoaderGroup.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 22:56
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace vFrame.Bundler
{
    internal abstract class AssetBundleLoaderGroup : Loader
    {
        private AssetBundle _assetBundle;

        private readonly List<AssetBundleLoader> _loaders;

        protected AssetBundleLoaderGroup(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

            _loaders = CreateBundleLoaderGroup(bundlerContexts, loaderContexts);
            if (null != _loaders) {
                return;
            }
            Abort();
        }

        protected abstract AssetBundleLoader CreateAssetBundleLoader(string bundlePath);

        protected override void OnDestroy() {
            _loaders.Clear();
            base.OnDestroy();
        }

        public AssetBundle AssetBundle {
            get {
                ForceComplete();
                ThrowIfNotFinished();

                if (null == _loaders || _loaders.Count <= 0) {
                    return null;
                }
                return _loaders[0].AssetBundle;
            }
        }

        private List<AssetBundleLoader> CreateBundleLoaderGroup(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) {
            if (!bundlerContexts.Manifest.Assets.TryGetValue(loaderContexts.AssetPath, out var mainBundle)) {
                Facade.GetSystem<LogSystem>().LogError("Bundle data not found for asset: {0}", loaderContexts.AssetPath);
                return null;
            }

            if (!bundlerContexts.Manifest.Bundles.TryGetValue(mainBundle, out var dependencySet)) {
                Facade.GetSystem<LogSystem>().LogError("Dependency data not found for bundle: {0}", mainBundle);
                return null;
            }

            var ret = new List<AssetBundleLoader>();
            ret.Add(CreateAssetBundleLoader(mainBundle));
            ret.AddRange(dependencySet.Values.Select(CreateAssetBundleLoader));
            return ret;
        }

        public override float Progress {
            get {
                var ret = 0f;
                foreach (var loader in _loaders) {
                    ret += loader.Progress;
                }
                return ret;
            }
        }

        protected override void OnStart() {
            foreach (var loader in _loaders) {
                loader.Start();
            }
        }

        protected override void OnStop() {
            foreach (var loader in _loaders) {
                loader.Stop();
            }
        }

        protected override void OnUpdate() {
            var finished = true;
            foreach (var loader in _loaders) {
                loader.Update();
                finished &= loader.IsDone;
            }

            if (finished) {
                Finish();
            }
        }

        protected override void OnForceComplete() {
            foreach (var loader in _loaders) {
                loader.ForceComplete();
            }
        }
    }
}