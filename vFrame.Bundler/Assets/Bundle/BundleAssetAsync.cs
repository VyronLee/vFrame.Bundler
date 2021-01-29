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

            Logger.LogInfo("End asynchronously loading asset from bundle: {0}, object: {1}", _path, _request.asset);

            // Must release reference after assets loaded.
            _loader.Release();

            IsDone = true;
        }

        public bool IsStarted { get; private set; }
        public override bool IsDone { get; set; }

        public float Progress {
            get { return _request == null ? 0f : _request.progress; }
        }

        public override Object GetAsset() {
            if (null == _request) {
                _request = CreateLoadRequest();
            }

            if (!IsDone) {
                Logger.LogError("Asset not loaded, force load complete: {0}", _path);
            }
            return _request.asset;
        }

        protected override void LoadAssetInternal() {
            Logger.LogInfo("Start asynchronously loading asset from bundle: {0}", _path);

            _request = CreateLoadRequest();
            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from bundle: {1}", GetAssetName(), _path));

            // Avoid releasing reference when loading assets.
            _loader.Retain();
        }

        private AssetBundleRequest CreateLoadRequest() {
            var name = GetAssetName();
            var request = _loader.AssetBundle.LoadAssetWithSubAssetsAsync(name, _type);
            request.allowSceneActivation = true;
            return request;
        }
    }
}