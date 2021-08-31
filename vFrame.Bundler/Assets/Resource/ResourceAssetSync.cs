//------------------------------------------------------------
//        File:  ResourceAssetSync.cs
//       Brief:  Sync load assets from resources.
//
//      Author:  VyronLee, lwz_jz@hotmail.co
//
//    Modified:  2019-02-15 20:00
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.IO;
using UnityEngine;
using vFrame.Bundler.Base.Pools;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Utils;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vFrame.Bundler.Assets.Resource
{
    public sealed class ResourceAssetSync : AssetBase
    {
        private Object _asset;

        internal ResourceAssetSync(string assetName, Type type, BundlerContext context)
            : base(assetName, type, null, context) {
        }

        public override Object GetAsset() {
            return _asset;
        }

        protected override void LoadAssetInternal() {
            Logs.Logger.LogInfo("Start synchronously loading asset: {0}", _path);

#if UNITY_EDITOR
            if (_context.Options.UseAssetDatabaseInsteadOfResources) {
                _asset = AssetDatabase.LoadAssetAtPath(_path, _type);
                if (!_asset)
                    throw new BundleAssetLoadFailedException("Could not load asset from AssetDatabase: " + _path);
            }
            else
#endif
            {
                var resPath = PathUtility.RelativeProjectPathToRelativeResourcesPath(_path);

                var sb = StringBuilderPool.Get();
                sb.Append(Path.GetDirectoryName(resPath));
                sb.Append("/");
                sb.Append(Path.GetFileNameWithoutExtension(resPath));

                _asset = Resources.Load(sb.ToString(), _type);
                if (!_asset)
                    throw new BundleAssetLoadFailedException("Could not load asset from resources: " + _path);

                StringBuilderPool.Return(sb);
            }

            IsDone = true;

            Logs.Logger.LogInfo("End synchronously loading asset: {0}", _path);
        }
    }
}