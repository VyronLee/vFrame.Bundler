//------------------------------------------------------------
//        File:  ResourceAssetAsync.cs
//       Brief:  Async load assets from resources.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 19:59
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.IO;
using UnityEngine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Utils;
using vFrame.Bundler.Utils.Pools;
using Logger = vFrame.Bundler.Logs.Logger;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vFrame.Bundler.Assets.Resource
{
    public sealed class ResourceAssetAsync : AssetBase, IAssetAsync
    {
        private ResourceRequest _request;

        public ResourceAssetAsync(string assetName, Type type, BundleLoaderBase target = null) : base(assetName, type,
            target)
        {
        }

        public bool MoveNext()
        {
            if (_asset)
                return false;

            if (_request == null || !_request.isDone)
                return true;

            _asset = _request.asset;

            Logs.Logger.LogInfo("End asynchronously loading asset: {0}", _path);

            return false;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        public override bool IsDone
        {
            get { return !MoveNext(); }
        }

        public float Progress
        {
            get { return _request == null ? 0f : _request.progress; }
        }

        public override Object GetAsset()
        {
            return _asset;
        }

        protected override void LoadAssetInternal()
        {
            Logs.Logger.LogInfo("Start asynchronously loading asset: {0}", _path);

#if UNITY_EDITOR
            if (BundlerCustomSettingsInEditorMode.kUseAssetDatabaseInsteadOfResources)
            {
                _asset = AssetDatabase.LoadAssetAtPath(_path, _type);
                if (!_asset)
                    throw new BundleAssetLoadFailedException(
                        string.Format("Cannot load asset {0} from AssetDatabase: ", _path));
                return;
            }
#endif

            var resPath = PathUtility.RelativeProjectPathToRelativeResourcesPath(_path);

            var sb = StringBuilderPool.Get();
            sb.Append(Path.GetDirectoryName(resPath));
            sb.Append("/");
            sb.Append(Path.GetFileNameWithoutExtension(resPath));

            _request = Resources.LoadAsync(sb.ToString(), _type);
            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from resources: ", sb));

            StringBuilderPool.Return(sb);
        }
    }
}