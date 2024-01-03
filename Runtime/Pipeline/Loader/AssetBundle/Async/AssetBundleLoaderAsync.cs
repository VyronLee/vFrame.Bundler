// ------------------------------------------------------------
//         File: AssetBundleLoader.cs
//        Brief: AssetBundleLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 23:0
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    internal class AssetBundleLoaderAsync : AssetBundleLoader
    {
        private AssetBundleCreateRequest _createRequest;
        private AssetBundle _assetBundle;

        public AssetBundleLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts, string bundlePath)
            : base(bundlerContexts, loaderContexts, bundlePath) {

        }

        public override float Progress {
            get {
                if (_createRequest == null) {
                    return 0f;
                }
                if (!_createRequest.isDone) {
                    return _createRequest.progress;
                }
                return 1f;
            }
        }

        public override AssetBundle AssetBundle {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetBundle;
            }
        }

        protected override void OnStart() {
            _createRequest = Adapter.CreateRequestAsync(BundlePath);
            if (null == _createRequest) {
                Facade.GetSystem<LogSystem>().LogError("Create AssetBundleCreateRequest failed: {0}", BundlePath);
                Abort();
                return;
            }
            _createRequest.allowSceneActivation = true;
        }

        protected override void OnStop() {
            if (_assetBundle) {
                _assetBundle.Unload(true);
            }
            _assetBundle = null;
            _createRequest = null;
        }

        protected override void OnUpdate() {
            if (null == _createRequest) {
                return;
            }
            if (!_createRequest.isDone) {
                return;
            }
            ObtainAssetBundleFromCreateRequest();
        }

        protected override void OnForceComplete() {
            if (null == _createRequest) {
                return;
            }
            ObtainAssetBundleFromCreateRequest();
        }

        private void ObtainAssetBundleFromCreateRequest() {
            _assetBundle = _createRequest.assetBundle;

            if (_assetBundle) {
                Finish();
                return;
            }
            Abort();

            Facade.GetSystem<LogSystem>().LogError(
                "Get AssetBundle from BundleLoadRequest[isDone: {0}, progress: {1}] failed: {2}",
                _createRequest.isDone,
                _createRequest.progress,
                BundlePath);
        }

        public override string ToString() {
            return $"[Type: {GetType().Name}, BundlePath: {BundlePath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}