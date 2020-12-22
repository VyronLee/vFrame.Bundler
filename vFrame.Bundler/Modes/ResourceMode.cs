//------------------------------------------------------------
//        File:  ResourceMode.cs
//       Brief:  ResourceMode
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:13
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using vFrame.Bundler.Assets.Resource;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.LoadRequests;

namespace vFrame.Bundler.Modes
{
    public class ResourceMode : ModeBase
    {
        public ResourceMode(BundlerManifest manifest, List<string> searchPaths, BundlerOptions options) : base(manifest,
            searchPaths, options) {
        }

        public override ILoadRequest Load(string path) {
            if (null != _manifest && !_manifest.assets.ContainsKey(path))
                throw new BundleNoneConfigurationException("Asset path not specified: " + path);
            return new LoadRequest(this, _options, path, null);
        }

        public override ILoadRequestAsync LoadAsync(string path) {
            if (null != _manifest && !_manifest.assets.ContainsKey(path))
                throw new BundleNoneConfigurationException("Asset path not specified: " + path);
            return new LoadRequestAsync(this, _options, path, null);
        }

        public override void Destroy() {
        }

        public override IAsset GetAsset(LoadRequest request, Type type) {
            return new ResourceAssetSync(request.AssetPath, type, _options);
        }

        public override IAssetAsync GetAssetAsync(LoadRequest request, Type type) {
            return new ResourceAssetAsync(request.AssetPath, type, _options);
        }
    }
}