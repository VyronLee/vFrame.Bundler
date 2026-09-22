// ------------------------------------------------------------
//         File: AssetBundleAssetLoaderAsync.cs
//        Brief: Asynchronous asset loader for AssetBundle mode; issues an AssetBundleRequest
//               (LoadAssetAsync / LoadAssetWithSubAssetsAsync) against the group's bundle and
//               completes once the requested asset(s) become available.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:29:09
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Asynchronous asset loader for AssetBundle mode. Issues an <see cref="AssetBundleRequest"/>
    /// against the parent <see cref="AssetBundleLoaderGroup"/>'s AssetBundle and captures the
    /// resulting asset(s) when the request completes.
    /// </summary>
    internal class AssetBundleAssetLoaderAsync : AssetBundleAssetLoader
    {
        /// <summary>
        /// The in-flight request issued in <see cref="OnStart"/>; null until started and reset on stop.
        /// </summary>
        private AssetBundleRequest _bundleRequest;

        /// <summary>Main asset captured from the completed request.</summary>
        private Object _assetObject;

        /// <summary>All assets (main plus sub-assets) captured from the completed request.</summary>
        private Object[] _assetObjects;

        /// <summary>
        /// Creates a new asynchronous AssetBundle-mode asset loader.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts.</param>
        /// <param name="loaderContexts">Loader contexts; <see cref="LoaderContexts.ParentLoader"/>
        /// must be an <see cref="AssetBundleLoaderGroup"/>.</param>
        /// <exception cref="BundleArgumentException">Thrown when the parent loader is not
        /// an <see cref="AssetBundleLoaderGroup"/>.</exception>
        public AssetBundleAssetLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>
        /// Loading progress: 0 before the request is issued, the live request progress while in
        /// flight, and 1 once the request is done.
        /// </summary>
        [JsonSerializableProperty]
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

        /// <summary>
        /// Issues the <see cref="AssetBundleRequest"/> on the group's bundle according to
        /// <see cref="AssetLoadType"/>, or aborts the loader when the bundle is missing or the
        /// request cannot be created.
        /// </summary>
        protected override void OnStart()
        {
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

        /// <summary>
        /// Clears the cached request and captured assets.
        /// </summary>
        protected override void OnStop()
        {
            _assetObject = null;
            _assetObjects = null;
            _bundleRequest = null;
        }

        /// <summary>
        /// Waits for the request to complete, then captures the asset(s) from it.
        /// </summary>
        protected override void OnUpdate()
        {
            if (null == _bundleRequest) {
                return;
            }
            if (!_bundleRequest.isDone) {
                return;
            }
            ObtainAssetObjectFromBundleRequest();
        }

        /// <summary>
        /// Captures the asset(s) immediately when a request exists, regardless of completion state.
        /// </summary>
        protected override void OnForceComplete()
        {
            if (null == _bundleRequest) {
                return;
            }
            ObtainAssetObjectFromBundleRequest();
        }

        /// <summary>
        /// Captures the main asset and sub-assets from the completed request, finishing the loader
        /// on success; aborts with an error log when the captured asset reference is invalid.
        /// </summary>
        private void ObtainAssetObjectFromBundleRequest()
        {
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

        /// <summary>
        /// Gets the loaded main asset, forcing the load to complete first.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader did not finish
        /// successfully (e.g. it was aborted).</exception>
        public override Object AssetObject {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetObject;
            }
        }

        /// <summary>
        /// Gets all loaded assets (main plus sub-assets), forcing the load to complete first.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader did not finish
        /// successfully (e.g. it was aborted).</exception>
        public override Object[] AssetObjects {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetObjects;
            }
        }
    }
}