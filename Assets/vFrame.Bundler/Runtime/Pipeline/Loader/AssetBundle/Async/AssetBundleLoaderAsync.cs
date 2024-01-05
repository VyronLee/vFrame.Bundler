// ------------------------------------------------------------
//         File: AssetBundleLoader.cs
//        Brief: AssetBundleLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 23:0
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
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
            try {
                _createRequest = Adapter.CreateRequest(BundlePath);
                if (null != _createRequest) {
                    _createRequest.allowSceneActivation = true;
                    return;
                }
            }
            catch (System.Exception e) {
                Facade.GetSystem<LogSystem>().LogException(e);
                Abort();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Create AssetBundleCreateRequest failed: {0}", BundlePath);
            Abort();
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
    }
}