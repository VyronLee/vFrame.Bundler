// ------------------------------------------------------------
//         File: AssetBundleLoader.cs
//        Brief: AssetBundleLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 22:37
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    internal class AssetBundleLoaderSync : AssetBundleLoader
    {
        private AssetBundle _assetBundle;

        public AssetBundleLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts, string bundlePath)
            : base(bundlerContexts, loaderContexts, bundlePath) {

        }

        public override float Progress => IsDone ? 1f : 0f;

        protected override void OnStart() {
            try {
                _assetBundle = Adapter.CreateAssetBundle(BundlePath);
                if (_assetBundle) {
                    Finish();
                    return;
                }
            }
            catch (System.Exception e) {
                Facade.GetSystem<LogSystem>().LogException(e);
                Abort();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Load AssetBundle failed: {0}", BundlePath);
            Abort();
        }

        protected override void OnStop() {
            if (_assetBundle) {
                _assetBundle.Unload(true);
            }
            _assetBundle = null;
        }

        protected override void OnUpdate() {
            Finish();
        }

        protected override void OnForceComplete() {
            Finish();
        }

        public override AssetBundle AssetBundle => _assetBundle;
    }
}