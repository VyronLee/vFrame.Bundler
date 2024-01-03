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
using vFrame.Bundler.Utils;
using Logger = vFrame.Bundler.Logs.Logger;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Assets.Bundle
{
    public sealed class BundleAssetAsync : AssetBase, IAssetAsync, IAsyncProcessor
    {
        private AssetBundleRequest _request;

        internal BundleAssetAsync(string path, Type type, BundleLoaderBase target, BundlerContext context)
            : base(path, type, target, context) {
        }

        public override void Dispose() {
            if (null != _context && null != _context.CoroutinePool) {
                AsyncRequestHelper.Uninstall(_context.CoroutinePool, this);
            }
            base.Dispose();
        }

        public IEnumerator OnAsyncProcess() {
            if (_request == null)
                yield break;

            IsStarted = true;
            yield return _request;

            UnityEngine.Logger.LogInfo("End asynchronously loading asset from bundle: {0}, object: {1}", _path, _request.asset);

            // Must release reference after assets loaded.
            _loader.Release();

            IsDone = true;
        }

        public bool IsStarted { get; private set; }

        public float Progress {
            get { return _request == null ? 0f : _request.progress; }
        }

        public override Object GetAsset() {
            if (null == _request) {
                _request = CreateLoadRequest();
            }

            if (!IsDone) {
                UnityEngine.Logger.LogError("Asset not loaded, force load complete: {0}", _path);
            }
            return _request.asset;
        }

        protected override void LoadAssetInternal() {
            UnityEngine.Logger.LogInfo("Start asynchronously loading asset from bundle: {0}", _path);

            _request = CreateLoadRequest();
            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from bundle: {1}", GetAssetName(), _path));

            // Avoid releasing reference when loading assets.
            _loader.Retain();

            AsyncRequestHelper.Setup(_context.CoroutinePool, this);
        }

        private AssetBundleRequest CreateLoadRequest() {
            var name = GetAssetName();
            var request = _loader.AssetBundle.LoadAssetWithSubAssetsAsync(name, _type);
            request.allowSceneActivation = true;
            return request;
        }

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {
        }

        public object Current { get; private set; }
        public bool IsSetup { get; set; }
        public int ThreadHandle { get; set; }
    }
}