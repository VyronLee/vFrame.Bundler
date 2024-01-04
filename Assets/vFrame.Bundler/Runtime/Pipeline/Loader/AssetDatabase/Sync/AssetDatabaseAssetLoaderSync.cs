// ------------------------------------------------------------
//         File: AssetDatabaseAssetLoaderSync.cs
//        Brief: AssetDatabaseAssetLoaderSync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 19:47
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using UnityEditor;
using vFrame.Bundler.Exception;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class AssetDatabaseAssetLoaderSync : AssetLoader
    {
        private Object _assetObject;
        private Object[] _assetObjects;

        public AssetDatabaseAssetLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {
        }

        public override float Progress => IsDone ? 1f : 0f;

        protected override void OnStart() {
            switch (AssetLoadType) {
                case AssetLoadType.LoadAsset:
                case AssetLoadType.LoadAssetWithSubAsset:
                    _assetObject = AssetDatabase.LoadAssetAtPath(AssetPath, AssetType);
                    break;
                case AssetLoadType.LoadAllAssets:
                    _assetObjects = AssetDatabase.LoadAllAssetsAtPath(AssetPath);
                    if (_assetObjects.Length > 0) {
                        _assetObject = _assetObjects[0];
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

            Facade.GetSystem<LogSystem>().LogError("Load asset from AssetDatabase failed: {0}", AssetPath);
            Abort();
        }

        protected override void OnStop() {
            _assetObject = null;
            _assetObjects = null;
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