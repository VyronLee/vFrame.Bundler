// ------------------------------------------------------------
//         File: AssetBundleAssetLoader.cs
//        Brief: AssetBundleAssetLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 18:6
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class AssetBundleAssetLoaderAsync : AssetBundleAssetLoader
    {
        private AssetBundleRequest _bundleRequest;
        private Object _assetObject;
        private Object[] _assetObjects;

        public AssetBundleAssetLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

        }

        [JsonSerializableProperty("F3")]
        public override float Progress {
            get {
                if (null == _bundleRequest) {
                    return 0f;
                }
                if (!_bundleRequest.isDone) {
                    return _bundleRequest.progress;
                }
                return 1f;
            }
        }

        protected override void OnStart() {
            var assetBundle = BundleLoader.AssetBundle;
            if (!assetBundle) {
                Abort();
                return;
            }

            switch (AssetLoadType) {
                case AssetLoadType.LoadAsset:
                case AssetLoadType.LoadAllAssets:
                    _bundleRequest = assetBundle.LoadAssetAsync(AssetPath, AssetType);
                    break;
                case AssetLoadType.LoadAssetWithSubAsset:
                    _bundleRequest = assetBundle.LoadAssetWithSubAssetsAsync(AssetPath, AssetType);
                    break;
                default:
                    Facade.GetSystem<LogSystem>().LogError("Unsupported load type: {0}", AssetLoadType);
                    break;
            }

            if (null != _bundleRequest) {
                _bundleRequest.allowSceneActivation = true;
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Create AssetBundleRequest failed: {0}, type: {1}",
                AssetPath, AssetType);

            Abort();
        }

        protected override void OnStop() {
            _assetObject = null;
            _assetObjects = null;
            _bundleRequest = null;
        }

        protected override void OnUpdate() {
            if (null == _bundleRequest) {
                return;
            }
            if (!_bundleRequest.isDone) {
                return;
            }
            ObtainAssetObjectFromBundleRequest();
        }

        protected override void OnForceComplete() {
            if (null == _bundleRequest) {
                return;
            }
            ObtainAssetObjectFromBundleRequest();
        }

        private void ObtainAssetObjectFromBundleRequest() {
            _assetObject = _bundleRequest.asset;
            _assetObjects = _bundleRequest.allAssets;

            if (_assetObject) {
                Finish();
                return;
            }

            Abort();

            Facade.GetSystem<LogSystem>().LogError(
                "Get asset from AssetBundleRequest[isDone: {0}, progress: {1}] failed: {2}",
                _bundleRequest.isDone,
                _bundleRequest.progress,
                AssetPath);
        }

        public override Object AssetObject {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetObject;
            }
        }

        public override Object[] AssetObjects {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetObjects;
            }
        }
    }
}