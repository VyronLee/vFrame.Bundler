// ------------------------------------------------------------
//         File: AssetBundleAssetLoaderSync.cs
//        Brief: AssetBundleAssetLoaderSync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 23:13
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class AssetBundleAssetLoaderSync : AssetBundleAssetLoader
    {
        private Object _assetObject;

        public AssetBundleAssetLoaderSync(BundlerContexts bundlerContexts,
            LoaderContexts loaderContexts,
            AssetBundleLoaderGroup bundleLoader) : base(bundlerContexts, loaderContexts, bundleLoader) {

        }

        public override float Progress => IsDone ? 1f : 0f;

        protected override void OnStart() {
            var assetBundle = BundleLoader.AssetBundle;
            if (!assetBundle) {
                Abort();
                return;
            }

            switch (AssetLoadType) {
                case AssetLoadType.LoadAsset:
                    _assetObject = assetBundle.LoadAsset(AssetPath, AssetType);
                    break;
                case AssetLoadType.LoadAssetWithSubAsset:
                    var assets = assetBundle.LoadAssetWithSubAssets(AssetPath, AssetType);
                    if (assets.Length > 0) {
                        _assetObject = assets[0];
                    }
                    break;
                default:
                    Facade.GetSystem<LogSystem>().LogError("Unsupported load type: {0}", AssetLoadType);
                    break;
            }

            if (_assetObject) {
                Finish();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Load asset from AssetBundle failed: {0}, type: {1}",
                AssetPath, AssetType);

            Abort();
        }

        protected override void OnStop() {
            _assetObject = null;
        }

        protected override void OnUpdate() {
            Finish();
        }

        protected override void OnForceComplete() {
            Finish();
        }

        public override Object AssetObject {
            get {
                ThrowIfNotFinished();
                return _assetObject;
            }
        }
    }
}