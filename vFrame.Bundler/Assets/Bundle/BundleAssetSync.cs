//------------------------------------------------------------
//        File:  BundleAssetSync.cs
//       Brief:  Sync load assets from bundles.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 19:58
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Logs;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Assets.Bundle
{
    public sealed class BundleAssetSync : AssetBase
    {
        public BundleAssetSync(string path, Type type, BundleLoaderBase target, BundlerOptions options)
            : base(path, type, target, options) {
        }

        protected override Object _asset { get; set; }

        protected override void LoadAssetInternal() {
            Logger.LogInfo("Start synchronously loading asset from bundle: {0}", _path);

            var name = GetAssetName();
            var assets = _loader.AssetBundle.LoadAssetWithSubAssets(name, _type);
            if (assets.Length <= 0)
                throw new BundleAssetLoadFailedException(
                    string.Format("Could not load asset {0} from assetbundle: ", name));
            _asset = assets[0];
            IsDone = true;

            Logger.LogInfo("End synchronously loading asset from bundle: {0}", _path);
        }
    }
}