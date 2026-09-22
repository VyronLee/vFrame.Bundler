// ------------------------------------------------------------
//         File: ResourcesAssetLoaderAsync.cs
//        Brief: Asynchronously loads assets from Unity Resources, mapping project-relative paths to Resources paths.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:50:32
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Asynchronously loads an asset from Unity Resources via <see cref="ResourceRequest"/>.
    /// Maps project-relative asset paths to Resources-relative paths before issuing the request.
    /// </summary>
    internal class ResourcesAssetLoaderAsync : AssetLoader
    {
        /// <summary>Underlying Resources request driving the load; null before start or after stop.</summary>
        private ResourceRequest _resourcesRequest;

        /// <summary>Single asset obtained from the completed request; null until then.</summary>
        private Object _assetObject;

        /// <summary>Array form of <see cref="_assetObject"/> (exactly one entry) served by <see cref="AssetObjects"/>.</summary>
        private Object[] _assetObjects;

        /// <summary>
        /// Initializes the loader with the shared bundler and per-load contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts.</param>
        /// <param name="loaderContexts">Per-load contexts describing the asset to load.</param>
        public ResourcesAssetLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>
        /// Gets the normalized load progress: 0 before the request exists, the request's progress
        /// while running, and 1 once done.
        /// </summary>
        [JsonSerializableProperty]
        public override float Progress {
            get {
                if (null == _resourcesRequest) {
                    return 0f;
                }
                if (!_resourcesRequest.isDone) {
                    return _resourcesRequest.progress;
                }
                return 1f;
            }
        }

        /// <summary>
        /// Maps the asset path to a Resources path and starts the asynchronous Resources request.
        /// Aborts the loader when the load type is unsupported or the request cannot be created.
        /// </summary>
        protected override void OnStart()
        {
            var resPath = PathUtils.RelativeProjectPathToRelativeResourcesPath(AssetPath);

            var sb = StringBuilderPool.Get();
            sb.Append(Path.GetDirectoryName(resPath));
            sb.Append("/");
            sb.Append(Path.GetFileNameWithoutExtension(resPath));
            resPath = sb.ToString();
            StringBuilderPool.Return(sb);

            switch (AssetLoadType) {
                case AssetLoadType.LoadAsset:
                case AssetLoadType.LoadAllAssets:
                case AssetLoadType.LoadAssetWithSubAsset:
                    _resourcesRequest = Resources.LoadAsync(resPath, AssetType);
                    break;
                default:
                    Facade.GetSystem<LogSystem>().LogError("Unsupported load type: {0}", AssetLoadType);
                    break;
            }
            if (null != _resourcesRequest) {
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Create Resources async request failed: {0}", AssetPath);
            Abort();
        }

        /// <summary>
        /// Releases the cached request and asset references when the loader stops.
        /// </summary>
        protected override void OnStop()
        {
            _assetObject = null;
            _resourcesRequest = null;
        }

        /// <summary>
        /// Polls the Resources request each update and completes the loader once it finishes.
        /// </summary>
        protected override void OnUpdate()
        {
            if (null == _resourcesRequest) {
                return;
            }
            if (!_resourcesRequest.isDone) {
                return;
            }
            ObtainAssetObjectFromResourcesRequest();
        }

        /// <summary>
        /// Extracts the loaded asset from the completed request: finishes the loader on success,
        /// or aborts and logs an error when the asset is missing.
        /// </summary>
        private void ObtainAssetObjectFromResourcesRequest()
        {
            _assetObject = _resourcesRequest.asset;
            _assetObjects = new[] { _resourcesRequest.asset };
            if (_assetObject) {
                Finish();
                return;
            }

            Abort();

            Facade.GetSystem<LogSystem>().LogError(
                "Get asset from ResourcesRequest[isDone: {0}, progress: {1}] failed: {2}",
                _resourcesRequest.isDone,
                _resourcesRequest.progress,
                AssetPath);
        }

        /// <summary>
        /// Synchronously harvests the asset from a pending request, bypassing the update loop.
        /// Does nothing when no request exists.
        /// </summary>
        protected override void OnForceComplete()
        {
            if (null == _resourcesRequest) {
                return;
            }
            ObtainAssetObjectFromResourcesRequest();
        }

        /// <summary>
        /// Gets the loaded asset, forcing synchronous completion first.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">The loader did not finish after being forced to complete.</exception>
        public override Object AssetObject {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetObject;
            }
        }

        /// <summary>
        /// Gets the loaded assets as a single-element array, forcing synchronous completion first.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">The loader did not finish after being forced to complete.</exception>
        public override Object[] AssetObjects {
            get {
                ForceComplete();
                ThrowIfNotFinished();
                return _assetObjects;
            }

        }
    }
}