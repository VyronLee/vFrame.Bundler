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
using System.Collections;
using UnityEngine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using Logger = vFrame.Bundler.Logs.Logger;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Assets.Bundle
{
    public sealed class BundleAssetAsync : AssetBase, IAssetAsync
    {
        private AssetBundleRequest _request;

        public BundleAssetAsync(string path, Type type, BundleLoaderBase target, BundlerOptions options)
            : base(path, type, target, options) {
        }

        public IEnumerator Await() {
            if (_request == null)
                yield break;

            if (IsStarted) {
                while (!IsDone) {
                    yield return null;
                }
                yield break;
            }

            IsStarted = true;
            yield return _request;
            _asset = _request.asset;

            Logger.LogInfo("End asynchronously loading asset from bundle: {0}, object: {1}", _path, _asset);

            if (null == _asset)
                Logger.LogError("End asynchronously loading asset, but asset == null! path: {0}", _path);
            else // Must release reference after assets loaded.
                _loader.Release();

            IsDone = true;
        }

        public bool IsStarted { get; private set; }
        public override bool IsDone { get; set; }

        public float Progress {
            get { return _request == null ? 0f : _request.progress; }
        }

        protected override Object _asset { get; set; }

        protected override void LoadAssetInternal() {
            Logger.LogInfo("Start asynchronously loading asset from bundle: {0}", _path);

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