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
using vFrame.Bundler.Assets.Bundle;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.LoadRequests;
using vFrame.Bundler.Messengers;
using vFrame.Bundler.Utils;
using vFrame.Bundler.Utils.Pools;

namespace vFrame.Bundler.Modes
{
    public class BundleMode : ModeBase
    {
        private static readonly Dictionary<string, BundleLoaderBase> LoaderCache
            = new Dictionary<string, BundleLoaderBase>();

        public BundleMode(BundlerManifest manifest, List<string> searchPaths) : base(manifest, searchPaths)
        {
        }

        public override ILoadRequest Load(string path)
        {
            var loader = CreateLoaderByAssetPath<BundleLoaderSync>(path);
            return new LoadRequestSync(this, path, loader);
        }

        public override ILoadRequestAsync LoadAsync(string path)
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

        public override void Collect()
        {
            var unused = ListPool<string>.Get();

            foreach (var kv in LoaderCache)
            {
                var loader = kv.Value;
                if (loader.GetReferences() <= 0)
                    unused.Add(kv.Key);
            }

            foreach (var name in unused)
            {
                var loader = LoaderCache[name];
                if (loader.IsLoading)
                    continue;

                loader.Unload();
                LoaderCache.Remove(name);
            }

            ListPool<string>.Return(unused);
        }

        public override void DeepCollect()
        {
            var deadMessengers = ListPool<BundlerMessenger>.Get();

            // "OnDestroy will only be called on game objects that have previously been active."
            // In this case we can only use "NullReference" to check whether game object is alive or not
            foreach (var messenger in BundlerMessenger.Messengers)
            {
                if (messenger == null)
                    deadMessengers.Add(messenger);
            }

            foreach (var messenger in deadMessengers)
                messenger.ReleaseRef();
            deadMessengers.Clear();

            ListPool<BundlerMessenger>.Return(deadMessengers);

            Collect();
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