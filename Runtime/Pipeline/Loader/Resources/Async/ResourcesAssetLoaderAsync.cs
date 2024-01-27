// ------------------------------------------------------------
//         File: ResourcesAssetLoaderAsync.cs
//        Brief: ResourcesAssetLoaderAsync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 20:4
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class ResourcesAssetLoaderAsync : AssetLoader
    {
        private ResourceRequest _resourcesRequest;
        private Object _assetObject;
        private Object[] _assetObjects;

        public ResourcesAssetLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

        }

        [JsonSerializableProperty("F3")]
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

        protected override void OnStart() {
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

        protected override void OnStop() {
            _assetObject = null;
            _resourcesRequest = null;
        }

        protected override void OnUpdate() {
            if (null == _resourcesRequest) {
                return;
            }
            if (!_resourcesRequest.isDone) {
                return;
            }
            ObtainAssetObjectFromResourcesRequest();
        }

        private void ObtainAssetObjectFromResourcesRequest() {
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

        protected override void OnForceComplete() {
            if (null == _resourcesRequest) {
                return;
            }
            ObtainAssetObjectFromResourcesRequest();
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