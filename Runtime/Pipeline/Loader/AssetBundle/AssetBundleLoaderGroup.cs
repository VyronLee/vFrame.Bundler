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

            _loaders = CreateBundleLoaders();
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

        public AssetBundleLoader MainBundleLoader {
            get {
                if (null == _loaders || _loaders.Count <= 0) {
                    return null;
                }
                return _loaders[0];
            }
        }

        public string MainAssetPath => LoaderContexts.AssetPath;
        public string MainBundlePath => MainBundleLoader?.BundlePath;

        public AssetBundle AssetBundle {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return MainBundleLoader?.AssetBundle;
            }
        }

        private List<AssetBundleLoader> CreateBundleLoaders() {
            if (!BundlerContexts.Manifest.Assets.TryGetValue(LoaderContexts.AssetPath, out var mainBundle)) {
                Facade.GetSystem<LogSystem>().LogError("Bundle data not found for asset: {0}", LoaderContexts.AssetPath);
                return null;
            }

            if (!BundlerContexts.Manifest.Bundles.TryGetValue(mainBundle, out var dependencySet)) {
                Facade.GetSystem<LogSystem>().LogError("Dependency data not found for bundle: {0}", mainBundle);
                return null;
            }

            var ret = new List<AssetBundleLoader>();
            ret.Add(GetOrCreateAssetBundleLoader(mainBundle));
            ret.AddRange(dependencySet.Values.Select(GetOrCreateAssetBundleLoader));
            return ret;
        }

        private AssetBundleLoader GetOrCreateAssetBundleLoader(string bundlePath) {
            if (BundlerContexts.TryGetLoader(bundlePath, out AssetBundleLoader bundleLoader)) {
                return bundleLoader;
            }
            var ret = CreateAssetBundleLoader(bundlePath);
            BundlerContexts.AddLoader(ret);
            return ret;
        }

        public override float Progress {
            get {
                var ret = 0f;
                foreach (var loader in _loaders) {
                    ret += loader.Progress;
                }
                return ret / _loaders.Count;
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
            var finished = true;
            foreach (var loader in _loaders) {
                loader.ForceComplete();
                finished &= loader.IsDone;
            }
            if (finished) {
                Finish();
            }
        }

        public override void Retain() {
            foreach (var loader in _loaders) {
                loader.Retain();
            }
            base.Retain();
        }

        public override void Release() {
            foreach (var loader in _loaders) {
                loader.Release();
            }
            base.Release();
        }

        public override string ToString() {
            return $"[Type: {GetType().Name}, MainBundlePath: {MainBundlePath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}