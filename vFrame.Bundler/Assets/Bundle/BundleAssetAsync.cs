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
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Utils;

namespace vFrame.Bundler.Assets.Bundle
{
    public sealed class BundleAssetAsync : AssetBase, IAssetAsync
    {
        private AssetBundleRequest _request;

        public BundleAssetAsync(string path, Type type, BundleLoaderBase target, BundlerOptions options) : base(path, type, target, options)
        {
        }

        public bool MoveNext()
        {
            if (_asset)
                return false;

            if (_request == null || _request.progress < 1f || !_request.isDone)
                return true;

            _asset = _request.asset;

            Logs.Logger.LogInfo("End asynchronously loading asset from bundle: {0}", _path);

            // Must release reference after assets loaded.
            _loader.Release();

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
            Logs.Logger.LogInfo("Start asynchronously loading asset from bundle: {0}", _path);

            var name = GetAssetName();
            _request = _loader.AssetBundle.LoadAssetWithSubAssetsAsync(name, _type);
            _request.allowSceneActivation = true;

            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from bundle: {1}", name, _path));

            // Avoid releasing reference when loading assets.
            _loader.Retain();
        }
    }
}
