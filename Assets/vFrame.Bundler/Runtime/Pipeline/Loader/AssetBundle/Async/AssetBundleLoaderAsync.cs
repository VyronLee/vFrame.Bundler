// ------------------------------------------------------------
//         File: AssetBundleLoaderAsync.cs
//        Brief: Async loader for a single AssetBundle: drives the adapter's create request
//               until the bundle opens, and unloads it on stop.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:29:13
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Asynchronous loader that opens a single AssetBundle through the adapter's
    /// <see cref="AssetBundleCreateRequest"/> and unloads it when the loader is stopped.
    /// </summary>
    internal class AssetBundleLoaderAsync : AssetBundleLoader
    {
        /// <summary>Pending create request issued by the adapter; null before start and after being consumed.</summary>
        private AssetBundleCreateRequest _createRequest;

        /// <summary>The opened AssetBundle; null until the create request completes successfully.</summary>
        private AssetBundle _assetBundle;

        /// <summary>
        /// Initializes the loader with the bundle path to open asynchronously.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler state and options.</param>
        /// <param name="loaderContexts">Per-load context for this loader.</param>
        /// <param name="bundlePath">Path of the AssetBundle this loader drives.</param>
        public AssetBundleLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts, string bundlePath)
            : base(bundlerContexts, loaderContexts, bundlePath)
        {

        }

        /// <summary>Progress of the create request: 0 before it is issued, 1 once it completes.</summary>
        [JsonSerializableProperty]
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

        /// <summary>
        /// Gets the loaded AssetBundle, blocking until the create request completes.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader did not finish successfully.</exception>
        public override AssetBundle AssetBundle {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetBundle;
            }
        }

        /// <summary>Issues the asynchronous create request via the adapter and aborts the loader if creation fails.</summary>
        protected override void OnStart()
        {
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

        /// <summary>Unloads the AssetBundle together with all loaded assets and clears pending state.</summary>
        protected override void OnStop()
        {
            if (_assetBundle) {
                _assetBundle.Unload(true);
            }
            _assetBundle = null;
            _createRequest = null;
        }

        /// <summary>Pumps the create request and consumes the AssetBundle once the request is done.</summary>
        protected override void OnUpdate()
        {
            if (null == _createRequest) {
                return;
            }
            if (!_createRequest.isDone) {
                return;
            }
            ObtainAssetBundleFromCreateRequest();
        }

        /// <summary>Consumes the create request immediately, blocking until the pending request completes.</summary>
        protected override void OnForceComplete()
        {
            if (null == _createRequest) {
                return;
            }
            ObtainAssetBundleFromCreateRequest();
        }

        /// <summary>
        /// Extracts the AssetBundle from the completed create request;
        /// finishes the loader on success, aborts it otherwise.
        /// </summary>
        private void ObtainAssetBundleFromCreateRequest()
        {
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