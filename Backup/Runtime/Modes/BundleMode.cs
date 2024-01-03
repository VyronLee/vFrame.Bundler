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
    internal class BundleMode : ModeBase
    {
        private readonly Dictionary<string, BundleLoaderBase> _loaderCache
            = new Dictionary<string, BundleLoaderBase>(); // Bundle name <=> Loader

        private readonly Dictionary<string, LoadRequest> _loadRequestCache
            = new Dictionary<string, LoadRequest>(); // Bundle name <=> Load request

        private readonly Dictionary<string, LoadRequestAsync> _loadRequestAsyncCache
            = new Dictionary<string, LoadRequestAsync>(); // Bundle name <=> Load request

        private readonly Dictionary<ILoadRequest, Dictionary<Type, IAsset>> _assetCache
            = new Dictionary<ILoadRequest, Dictionary<Type, IAsset>>(); // Load request <=> Asset typed dict.

        private readonly Dictionary<ILoadRequestAsync, Dictionary<Type, IAssetAsync>> _assetAsyncCache
            = new Dictionary<ILoadRequestAsync, Dictionary<Type, IAssetAsync>>(); // Load request <=> Asset typed dict.

        internal BundleMode(BundlerManifest manifest, List<string> searchPaths, BundlerContext context)
            : base(manifest, searchPaths, context) {
        }

        public override ILoadRequest Load(string path) {
            LoadRequest loadRequest;
            if (_loadRequestCache.TryGetValue(path, out loadRequest)) {
                Logger.LogInfo("Get sync load request from cache: {0}", path);
                return loadRequest;
            }

            var loader = CreateLoaderByAssetPath(path, false);
            var loadRequestSync = _loadRequestCache[path] = new LoadRequestSync(this, _context, path, loader);

            Logger.LogInfo("Add sync load request to cache: {0}", path);
            return loadRequestSync;
        }

        public override ILoadRequestAsync LoadAsync(string path) {
            LoadRequestAsync loadRequestAsync;
            if (_loadRequestAsyncCache.TryGetValue(path, out loadRequestAsync)) {
                Logger.LogInfo("Get async load request from cache: {0}", path);
                return loadRequestAsync;
            }

            var loader = CreateLoaderByAssetPath(path, true);
            loadRequestAsync = _loadRequestAsyncCache[path] = new LoadRequestAsync(this, _context, path, loader);

            Logger.LogInfo("Add async load request to cache: {0}", path);
            return loadRequestAsync;
        }

        private BundleLoaderBase CreateLoaderByAssetPath(string assetPath, bool async) {
            assetPath = PathUtility.NormalizePath(assetPath);
            Logger.LogInfo("Create loader by asset path: {0}", assetPath);

            if (!_manifest.Assets.ContainsKey(assetPath))
                throw new BundleNoneConfigurationException("Asset path not specified: " + assetPath);

            var bundle = _manifest.Assets[assetPath];
            return CreateLoader(bundle, async);
        }

        private BundleLoaderBase CreateLoader(string bundlePath, bool async) {
            Logger.LogInfo("Create loader: {0}", bundlePath);
            if (!_loaderCache.TryGetValue(bundlePath, out var bundleLoader)) {
                var bundleData = _manifest.Bundles[bundlePath];

                var dependencies = new List<BundleLoaderBase>();
                bundleData.ForEach(v => dependencies.Add(CreateLoader(v, async)));

                foreach (var dependency in bundleData.Values) {

                }

                if (async)
                    bundleLoader = _context.Options.LoaderFactory.CreateLoaderAsync();
                else
                    bundleLoader = _context.Options.LoaderFactory.CreateLoader();
                bundleLoader.Initialize(bundlePath, _searchPaths, _context);
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
                loader.Dispose();

                if (_loaderCache.ContainsKey(name)) {
                    Logger.LogInfo("Remove loader from cache: {0}", name);
                    _loaderCache.Remove(name);
                }

                var toRemoveLoaderRequestName = ListPool<string>.Get();
                var toRemoveLoaderRequest = ListPool<ILoadRequest>.Get();
                var toRemoveLoaderAsyncRequestName = ListPool<string>.Get();
                var toRemoveLoaderAsyncRequest = ListPool<ILoadRequestAsync>.Get();

                foreach (var kv in _loadRequestCache) {
                    if (kv.Value.Loader != loader)
                        continue;
                    toRemoveLoaderRequestName.Add(kv.Key);
                    toRemoveLoaderRequest.Add(kv.Value);
                }

                foreach (var kv in _loadRequestAsyncCache) {
                    if (kv.Value.Loader != loader)
                        continue;
                    toRemoveLoaderAsyncRequestName.Add(kv.Key);
                    toRemoveLoaderAsyncRequest.Add(kv.Value);
                }

                foreach (var v in toRemoveLoaderRequestName) {
                    if (_loadRequestCache.ContainsKey(v)) {
                        Logger.LogInfo("Remove sync load request from cache: {0}", v);
                        _loadRequestCache[v].Dispose();
                        _loadRequestCache.Remove(v);
                    }
                }

                foreach (var v in toRemoveLoaderRequest) {
                    Dictionary<Type, IAsset> assets;
                    if (_assetCache.TryGetValue(v, out assets)) {
                        foreach (var kv in assets) {
                            kv.Value.Dispose();
                        }
                    }
                    _assetCache.Remove(v);
                }

                foreach (var v in toRemoveLoaderAsyncRequestName) {
                    if (_loadRequestAsyncCache.ContainsKey(v)) {
                        Logger.LogInfo("Remove async load request from cache: {0}", v);
                        _loadRequestAsyncCache[v].Dispose();
                        _loadRequestAsyncCache.Remove(v);
                    }
                }

                foreach (var v in toRemoveLoaderAsyncRequest) {
                    Dictionary<Type, IAssetAsync> assets;
                    if (_assetAsyncCache.TryGetValue(v, out assets)) {
                        foreach (var kv in assets) {
                            kv.Value.Dispose();
                        }
                    }
                    _assetAsyncCache.Remove(v);
                }

                ListPool<string>.Return(toRemoveLoaderRequestName);
                ListPool<ILoadRequest>.Return(toRemoveLoaderRequest);
                ListPool<string>.Return(toRemoveLoaderAsyncRequestName);
                ListPool<ILoadRequestAsync>.Return(toRemoveLoaderAsyncRequest);
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
                Logger.LogVerbose("Messenger has ben dead: {0}", messenger);
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
            foreach (var kv in _assetCache) {
                foreach (var kv2 in kv.Value) {
                    kv2.Value.Dispose();
                }
            }
            _assetCache.Clear();

            foreach (var kv in _assetAsyncCache) {
                foreach (var kv2 in kv.Value) {
                    kv2.Value.Dispose();
                }
            }
            _assetAsyncCache.Clear();

            foreach (var kv in _loaderCache) {
                kv.Value.ForceUnload();
                kv.Value.Dispose();
            }
            _loaderCache.Clear();

            foreach (var kv in _loadRequestCache) {
                kv.Value.Dispose();
            }
            _loadRequestCache.Clear();

            foreach (var kv in _loadRequestAsyncCache) {
                kv.Value.Dispose();
            }
            _loadRequestAsyncCache.Clear();
        }

        public override IAsset GetAsset(LoadRequest request, Type type) {
            Dictionary<Type, IAsset> assetCache;
            if (!_assetCache.TryGetValue(request, out assetCache))
                assetCache = _assetCache[request] = new Dictionary<Type, IAsset>();

            IAsset asset;
            if (!assetCache.TryGetValue(type, out asset))
                asset = assetCache[type] = new BundleAssetSync(request.AssetPath, type, request.Loader, _context);
            return asset;
        }

        public override IAssetAsync GetAssetAsync(LoadRequestAsync request, Type type) {
            Dictionary<Type, IAssetAsync> assetCache;
            if (!_assetAsyncCache.TryGetValue(request, out assetCache))
                assetCache = _assetAsyncCache[request] = new Dictionary<Type, IAssetAsync>();

            IAssetAsync asset;
            if (!assetCache.TryGetValue(type, out asset))
                asset = assetCache[type] = new BundleAssetAsync(request.AssetPath, type, request.Loader, _context);
            return asset;
        }

        public override List<BundleLoaderBase> GetLoaders() {
            return _loaderCache.Select(v => v.Value).ToList();
        }
    }
}