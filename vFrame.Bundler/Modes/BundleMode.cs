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
        private readonly Dictionary<string, BundleLoaderBase> _loaderCache
            = new Dictionary<string, BundleLoaderBase>();

        private readonly Dictionary<string, LoadRequest> _loadRequestCache
            = new Dictionary<string, LoadRequest>();

        private readonly Dictionary<string, LoadRequestAsync> _loadRequestAsyncCache
            = new Dictionary<string, LoadRequestAsync>();

        private readonly Dictionary<ILoadRequest, Dictionary<Type, IAsset>> _assetCache
            = new Dictionary<ILoadRequest, Dictionary<Type, IAsset>>();

        private readonly Dictionary<ILoadRequest, Dictionary<Type, IAssetAsync>> _assetAsyncCache
            = new Dictionary<ILoadRequest, Dictionary<Type, IAssetAsync>>();

        public BundleMode(BundlerManifest manifest, List<string> searchPaths) : base(manifest, searchPaths)
        {
        }

        public override ILoadRequest Load(string path) {
            LoadRequest loadRequest;
            if (_loadRequestCache.TryGetValue(path, out loadRequest))
                return loadRequest;

            var loader = CreateLoaderByAssetPath<BundleLoaderSync>(path);
            loadRequest = _loadRequestCache[path] = new LoadRequestSync(this, path, loader);
            return loadRequest;
        }

        public override ILoadRequestAsync LoadAsync(string path) {
            LoadRequestAsync loadRequestAsync;
            if (_loadRequestAsyncCache.TryGetValue(path, out loadRequestAsync)) {
                return loadRequestAsync;
            }

            var loader = CreateLoaderByAssetPath<BundleLoaderAsync>(path);
            loadRequestAsync = _loadRequestAsyncCache[path] = new LoadRequestAsync(this, path, loader);
            return loadRequestAsync;
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
            if (!_loaderCache.TryGetValue(bundlePath, out bundleLoader))
            {
                var bundleData = _manifest.bundles[bundlePath];
                var dependencies = new List<BundleLoaderBase>();
                bundleData.dependencies.ForEach(v => dependencies.Add(CreateLoader<TLoader>(v)));

                bundleLoader = new TLoader();
                bundleLoader.Initialize(bundlePath, _searchPaths);
                bundleLoader.Dependencies = dependencies;

                _loaderCache.Add(bundlePath, bundleLoader);
            }

            bundleLoader.Retain();
            return bundleLoader;
        }

        public override void Collect()
        {
            var unused = ListPool<string>.Get();

            foreach (var kv in _loaderCache)
            {
                var loader = kv.Value;
                if (loader.GetReferences() <= 0)
                    unused.Add(kv.Key);
            }

            foreach (var name in unused)
            {
                var loader = _loaderCache[name];
                if (loader.IsLoading)
                    continue;

                loader.Unload();
                _loaderCache.Remove(name);

                var toRemoveLoaderRequestName = ListPool<string>.Get();
                var toRemoveLoaderRequest = ListPool<ILoadRequest>.Get();

                foreach (var kv in _loadRequestCache) {
                    if (kv.Value.Loader != loader)
                        continue;
                    toRemoveLoaderRequestName.Add(kv.Key);
                    toRemoveLoaderRequest.Add(kv.Value);
                }

                foreach (var kv in _loadRequestAsyncCache) {
                    if (kv.Value.Loader != loader)
                        continue;
                    toRemoveLoaderRequestName.Add(kv.Key);
                    toRemoveLoaderRequest.Add(kv.Value);
                }

                foreach (var v in toRemoveLoaderRequestName) {
                    _loadRequestCache.Remove(v);
                    _loadRequestAsyncCache.Remove(v);
                }

                foreach (var v in toRemoveLoaderRequest) {
                    _assetCache.Remove(v);
                    _assetAsyncCache.Remove(v);
                }

                ListPool<string>.Return(toRemoveLoaderRequestName);
                ListPool<ILoadRequest>.Return(toRemoveLoaderRequest);
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

        public override IAsset GetAsset(LoadRequest request, Type type) {
            Dictionary<Type, IAsset> assetCache;
            if (!_assetCache.TryGetValue(request, out assetCache)) {
                assetCache = _assetCache[request] = new Dictionary<Type, IAsset>();
            }

            IAsset asset;
            if (!assetCache.TryGetValue(type, out asset)) {
                asset = assetCache[type] = new BundleAssetSync(request.AssetPath, type, request.Loader);
            }
            return asset;
        }

        public override IAssetAsync GetAssetAsync(LoadRequest request, Type type)
        {
            Dictionary<Type, IAssetAsync> assetCache;
            if (!_assetAsyncCache.TryGetValue(request, out assetCache)) {
                assetCache = _assetAsyncCache[request] = new Dictionary<Type, IAssetAsync>();
            }

            IAssetAsync asset;
            if (!assetCache.TryGetValue(type, out asset)) {
                asset = assetCache[type] = new BundleAssetAsync(request.AssetPath, type, request.Loader);
            }
            return asset;
        }
    }
}