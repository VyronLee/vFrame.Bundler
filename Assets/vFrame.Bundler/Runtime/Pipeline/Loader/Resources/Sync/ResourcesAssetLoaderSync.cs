// ------------------------------------------------------------
//         File: ResourcesAssetLoaderSync.cs
//        Brief: ResourcesAssetLoaderSync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 19:54
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class ResourcesAssetLoaderSync : AssetLoader
    {
        private Object _assetObject;
        private Object[] _assetObjects;

        public ResourcesAssetLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

        }

        public override float Progress => IsDone ? 1f : 0f;

        protected override void OnStart() {
            var resPath = PathUtils.RelativeProjectPathToRelativeResourcesPath(AssetPath);

            var sb = StringBuilderPool.Get();
            sb.Append(Path.GetDirectoryName(resPath));
            sb.Append("/");
            sb.Append(Path.GetFileNameWithoutExtension(resPath));
            resPath = sb.ToString();
            StringBuilderPool.Return(sb);

            _assetObject = Resources.Load(resPath, AssetType);
            _assetObjects = new[] { _assetObject };
            if (_assetObject) {
                Finish();
                return;
            }

            Facade.GetSystem<LogSystem>().LogError("Load asset from Resources failed: {0}", AssetPath);
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

        public override Object[] AssetObjects {
            get {
                ThrowIfNotFinished();
                return _assetObjects;
            }
        }
    }
}