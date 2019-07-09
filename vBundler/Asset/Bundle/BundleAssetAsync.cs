//------------------------------------------------------------
//        File:  BundleAssetAsync.cs
//       Brief:  Async load assets from bundle.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 19:57
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using UnityEngine;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Loader;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Asset.Bundle
{
    public sealed class BundleAssetAsync : AssetBase, IAssetAsync
    {
        private AssetBundleRequest _request;

        public BundleAssetAsync(string path, Type type, BundleLoaderBase target) : base(path, type, target)
        {
        }

        public bool MoveNext()
        {
            if (_asset)
                return false;

            if (_request == null || _request.progress < 1f || !_request.isDone)
                return true;

            _asset = _request.asset;

            Logger.LogInfo("End asynchronously loading asset from bundle: {0}", _path);

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

        protected override void LoadAssetInternal()
        {
            Logger.LogInfo("Start asynchronously loading asset from bundle: {0}", _path);

            var name = PathUtility.GetAssetName(_path);
            _request = _target.AssetBundle.LoadAssetWithSubAssetsAsync(name, _type);
            _request.allowSceneActivation = true;

            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from bundle: {1}", name, _path));
        }
    }
}