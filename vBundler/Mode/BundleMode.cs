//------------------------------------------------------------
//        File:  BundleMode.cs
//       Brief:  BundleMode
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:12
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using vBundler.Asset.Bundle;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Loader;
using vBundler.LoadRequests;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Mode
{
    public class BundleMode : ModeBase
    {
        private static readonly Dictionary<string, BundleLoaderBase> LoaderCache
            = new Dictionary<string, BundleLoaderBase>();

        public BundleMode(BundlerManifest manifest, List<string> searchPaths) : base(manifest, searchPaths)
        {
        }

        public override ILoadRequest LoadAsset(string path)
        {
            var loader = CreateLoaderByAssetPath<BundleLoaderSync>(path);
            return new LoadRequestSync(this, path, loader);
        }

        public override ILoadRequestAsync LoadAssetAsync(string path)
        {
            var loader = CreateLoaderByAssetPath<BundleLoaderAsync>(path);
            return new LoadRequestAsync(this, path, loader);
        }

        private BundleLoaderBase CreateLoaderByAssetPath<TLoader>(string assetPath)
            where TLoader : BundleLoaderBase, new()
        {
            assetPath = PathUtility.NormalizePath(assetPath);

            if (!_manifest.assets.ContainsKey(assetPath))
                throw new BundleNoneConfigurationException("Asset path not specified: " + assetPath);

            var assetData = _manifest.assets[assetPath];
            return CreateLoader<TLoader>(assetData.bundle);
        }

        private BundleLoaderBase CreateLoader<TLoader>(string bundlePath) where TLoader : BundleLoaderBase, new()
        {
            BundleLoaderBase bundleLoader;
            if (!LoaderCache.TryGetValue(bundlePath, out bundleLoader))
            {
                var bundleData = _manifest.bundles[bundlePath];
                var dependencies = new List<BundleLoaderBase>();
                bundleData.dependencies.ForEach(v => dependencies.Add(CreateLoader<TLoader>(v)));

                bundleLoader = new TLoader();
                bundleLoader.Initialize(bundlePath, _searchPaths);
                bundleLoader.Dependencies = dependencies;

                LoaderCache.Add(bundlePath, bundleLoader);
            }

            bundleLoader.Retain();
            return bundleLoader;
        }

        public override void GarbageCollect()
        {
            var unused = new LinkedList<string>();
            foreach (var kv in LoaderCache)
            {
                var loader = kv.Value;
                if (loader.GetReferences() <= 0)
                    unused.AddLast(kv.Key);
            }

            foreach (var name in unused)
            {
                var loader = LoaderCache[name];
                if (loader.IsLoading)
                    continue;

                Logger.LogVerbose(string.Format("Garbage collecting[{0}] - unloading {1}", Time.frameCount, name));

                loader.Unload();
                LoaderCache.Remove(name);
            }
        }

        public override IAsset GetAsset(LoadRequest request, Type type)
        {
            return new BundleAssetSync(request.AssetPath, type, request.Loader);
        }

        public override IAssetAsync GetAssetAsync(LoadRequest request, Type type)
        {
            return new BundleAssetAsync(request.AssetPath, type, request.Loader);
        }
    }
}