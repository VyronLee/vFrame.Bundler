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
using System.Linq;
using vFrame.Bundler.Assets.Bundle;
using vFrame.Bundler.Base.Pools;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.LoadRequests;
using vFrame.Bundler.Logs;
using vFrame.Bundler.Messengers;
using vFrame.Bundler.Utils;

namespace vFrame.Bundler.Modes
{
    public class BundleMode : ModeBase
    {
        private readonly Dictionary<string, BundleLoaderBase> _loaderCache
            = new Dictionary<string, BundleLoaderBase>(); // Bundle name <=> Loader

        private readonly Dictionary<string, LoadRequest> _loadRequestCache
            = new Dictionary<string, LoadRequest>(); // Bundle name <=> Load request

        private readonly Dictionary<string, LoadRequestAsync> _loadRequestAsyncCache
            = new Dictionary<string, LoadRequestAsync>(); // Bundle name <=> Load request

        private readonly Dictionary<ILoadRequest, Dictionary<Type, IAsset>> _assetCache
            = new Dictionary<ILoadRequest, Dictionary<Type, IAsset>>(); // Load request <=> Asset typed dict.

        private readonly Dictionary<ILoadRequest, Dictionary<Type, IAssetAsync>> _assetAsyncCache
            = new Dictionary<ILoadRequest, Dictionary<Type, IAssetAsync>>(); // Load request <=> Asset typed dict.

        public BundleMode(BundlerManifest manifest, List<string> searchPaths, BundlerOptions options)
            : base(manifest, searchPaths, options) {
        }

        public override ILoadRequest Load(string path) {
            LoadRequest loadRequest;
            if (_loadRequestCache.TryGetValue(path, out loadRequest)) {
                Logger.LogInfo("Get sync load request from cache: {0}", path);
                return loadRequest;
            }

            var loader = CreateLoaderByAssetPath(path, false);
            loadRequest = _loadRequestCache[path] = new LoadRequestSync(this, _options, path, loader);
            Logger.LogInfo("Add sync load request to cache: {0}", path);
            return loadRequest;
        }

        public override ILoadRequestAsync LoadAsync(string path) {
            LoadRequestAsync loadRequestAsync;
            if (_loadRequestAsyncCache.TryGetValue(path, out loadRequestAsync)) {
                Logger.LogInfo("Get async load request from cache: {0}", path);
                return loadRequestAsync;
            }

            var loader = CreateLoaderByAssetPath(path, true);
            loadRequestAsync = _loadRequestAsyncCache[path] = new LoadRequestAsync(this, _options, path, loader);
            Logger.LogInfo("Add async load request to cache: {0}", path);
            return loadRequestAsync;
        }

        private BundleLoaderBase CreateLoaderByAssetPath(string assetPath, bool async) {
            assetPath = PathUtility.NormalizePath(assetPath);

            if (!_manifest.assets.ContainsKey(assetPath))
                throw new BundleNoneConfigurationException("Asset path not specified: " + assetPath);

            var assetData = _manifest.assets[assetPath];
            return CreateLoader(assetData.bundle, async);
        }

        private BundleLoaderBase CreateLoader(string bundlePath, bool async) {
            BundleLoaderBase bundleLoader;
            if (!_loaderCache.TryGetValue(bundlePath, out bundleLoader)) {
                var bundleData = _manifest.bundles[bundlePath];
                var dependencies = new List<BundleLoaderBase>();
                bundleData.dependencies.ForEach(v => dependencies.Add(CreateLoader(v, async)));

                if (async)
                    bundleLoader = _options.LoaderFactory.CreateLoaderAsync();
                else
                    bundleLoader = _options.LoaderFactory.CreateLoader();
                bundleLoader.Initialize(bundlePath, _searchPaths, _options);
                bundleLoader.Dependencies = dependencies;

                _loaderCache.Add(bundlePath, bundleLoader);

                Logger.LogInfo("Add loader to cache: {0}", bundlePath);
            }
            else {
                Logger.LogInfo("Get loader from cache: {0}", bundlePath);
            }

            return bundleLoader;
        }

        public override void Collect() {
            var unused = ListPool<string>.Get();

            foreach (var kv in _loaderCache) {
                var loader = kv.Value;
                if (loader.GetReferences() <= 0)
                    unused.Add(kv.Key);
            }

            foreach (var name in unused) {
                var loader = _loaderCache[name];
                if (loader.IsLoading)
                    continue;

                loader.Unload();

                if (_loaderCache.ContainsKey(name)) {
                    Logger.LogInfo("Remove loader from cache: {0}", name);
                    _loaderCache.Remove(name);
                }

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
                    if (_loadRequestCache.ContainsKey(v)) {
                        Logger.LogInfo("Remove sync load request from cache: {0}", v);
                        _loadRequestCache.Remove(v);
                    }

                    if (_loadRequestAsyncCache.ContainsKey(v)) {
                        Logger.LogInfo("Remove async load request from cache: {0}", v);
                        _loadRequestAsyncCache.Remove(v);
                    }
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

        public override void DeepCollect() {
            var deadMessengers = ListPool<BundlerMessenger>.Get();

            // "OnDestroy will only be called on game objects that have previously been active."
            // In this case we can only use "NullReference" to check whether game object is alive or not
            foreach (var messenger in BundlerMessenger.Messengers) {
                if (messenger.Alive)
                    continue;
                deadMessengers.Add(messenger);
            }

            foreach (var messenger in deadMessengers)
                messenger.ReleaseRef();
            deadMessengers.Clear();

            ListPool<BundlerMessenger>.Return(deadMessengers);

            Collect();
        }

        public override void Destroy() {
            DeepCollect();
            Collect();

            ForceUnloadLoaders();
        }

        private void ForceUnloadLoaders() {
            foreach (var kv in _loaderCache)
                kv.Value.ForceUnload();
            _loaderCache.Clear();
            _loadRequestCache.Clear();
            _loadRequestAsyncCache.Clear();
            _assetCache.Clear();
            _assetAsyncCache.Clear();
        }

        public override IAsset GetAsset(LoadRequest request, Type type) {
            Dictionary<Type, IAsset> assetCache;
            if (!_assetCache.TryGetValue(request, out assetCache))
                assetCache = _assetCache[request] = new Dictionary<Type, IAsset>();

            IAsset asset;
            if (!assetCache.TryGetValue(type, out asset))
                asset = assetCache[type] = new BundleAssetSync(request.AssetPath, type, request.Loader, _options);
            return asset;
        }

        public override IAssetAsync GetAssetAsync(LoadRequest request, Type type) {
            Dictionary<Type, IAssetAsync> assetCache;
            if (!_assetAsyncCache.TryGetValue(request, out assetCache))
                assetCache = _assetAsyncCache[request] = new Dictionary<Type, IAssetAsync>();

            IAssetAsync asset;
            if (!assetCache.TryGetValue(type, out asset))
                asset = assetCache[type] = new BundleAssetAsync(request.AssetPath, type, request.Loader, _options);
            return asset;
        }

        public override List<BundleLoaderBase> GetLoaders() {
            return _loaderCache.Select(v => v.Value).ToList();
        }
    }
}