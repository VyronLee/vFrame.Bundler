using System;
using System.Collections.Generic;
using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    internal class BundlerContexts
    {
        public Bundler Bundler { get; set; }
        public BundlerOptions Options { get; set; }
        public BundlerManifest Manifest { get; set; }

        public Dictionary<AssetLoadKey, AssetLoader> AssetLoaders { get; } = new Dictionary<AssetLoadKey, AssetLoader>();
        public Dictionary<string, SceneLoader> SceneLoaders { get; } = new Dictionary<string, SceneLoader>();
        public Dictionary<string, AssetBundleLoader> AssetBundleLoaders { get; } = new Dictionary<string, AssetBundleLoader>();
        public Dictionary<string, AssetBundleLoaderGroup> AssetBundleLoaderGroups { get; } = new Dictionary<string, AssetBundleLoaderGroup>();
        public Dictionary<string, Scene> SceneHandlers { get; } = new Dictionary<string, Scene>();

        public void AddHandler(ILoaderHandler loaderHandler) {
            switch (loaderHandler) {
                case Scene scene:
                    SceneHandlers.Add(scene.SceneLoader.AssetPath, scene);
                    break;
            }
        }

        public void ForEachHandler(Action<ILoaderHandler> action) {
            foreach (var kv in SceneHandlers) {
                action(kv.Value);
            }
        }

        public void AddLoader(Loader loader) {
            switch (loader) {
                case AssetLoader assetLoader:
                    AssetLoaders.Add(assetLoader.AssetLoadKey, assetLoader);
                    break;
                case SceneLoader sceneLoader:
                    SceneLoaders.Add(sceneLoader.AssetPath, sceneLoader);
                    break;
                case AssetBundleLoader bundlerLoader:
                    AssetBundleLoaders.Add(bundlerLoader.BundlePath, bundlerLoader);
                    break;
                case AssetBundleLoaderGroup bundlerLoaderGroup:
                    AssetBundleLoaderGroups.Add(bundlerLoaderGroup.MainBundlePath, bundlerLoaderGroup);
                    break;
                default:
                    throw new BundleUnsupportedEnumException(loader.GetType().Name);
            }
        }

        public bool TryGetLoader<TKey, TType>(TKey key, out TType value) where TType: Loader {
            var ret = false;
            ret |= TryGetAssetBundleLoader(key, out value);
            ret |= TryGetAssetBundleLoaderGroup(key, out value);
            ret |= TryGetAssetLoader(key, out value);
            ret |= TryGetSceneLoader(key, out value);
            return ret;
        }

        public void ForEachLoader(Action<Loader> action) {
            foreach (var kv in AssetBundleLoaderGroups) {
                action(kv.Value);
            }
            foreach (var kv in AssetBundleLoaders) {
                action(kv.Value);
            }
            foreach (var kv in AssetLoaders) {
                action(kv.Value);
            }
            foreach (var kv in SceneLoaders) {
                action(kv.Value);
            }
        }

        private bool TryGetAssetBundleLoader<TK, TT>(TK key, out TT value) where TT: Loader {
            if (!typeof(AssetBundleLoader).IsAssignableFrom(typeof(TT))) {
                value = null;
                return false;
            }

            if (typeof(TK) == typeof(string)) {
                value = null;
                return false;
            }

            var str = key as string ?? throw new BundleArgumentNullException();
            var ret = AssetBundleLoaders.TryGetValue(str, out var loader);
            value = loader as TT;
            return ret;
        }

        private bool TryGetAssetBundleLoaderGroup<TK, TT>(TK key, out TT value) where TT: Loader {
            if (!typeof(AssetBundleLoaderGroup).IsAssignableFrom(typeof(TT))) {
                value = null;
                return false;
            }

            if (typeof(TK) == typeof(string)) {
                value = null;
                return false;
            }

            var str = key as string ?? throw new BundleArgumentNullException();
            var ret = AssetBundleLoaderGroups.TryGetValue(str, out var loader);
            value = loader as TT;
            return ret;
        }

        private bool TryGetAssetLoader<TK, TT>(TK key, out TT value) where TT: Loader {
            if (!typeof(AssetLoader).IsAssignableFrom(typeof(TT))) {
                value = null;
                return false;
            }

            if (typeof(TK) == typeof(AssetLoadKey)) {
                value = null;
                return false;
            }

            var loaderKey = key as AssetLoadKey ?? throw new BundleArgumentNullException();
            var ret = AssetLoaders.TryGetValue(loaderKey, out var loader);
            value = loader as TT;
            return ret;
        }

        private bool TryGetSceneLoader<TK, TT>(TK key, out TT value) where TT: Loader {
            if (!typeof(SceneLoader).IsAssignableFrom(typeof(TT))) {
                value = null;
                return false;
            }

            if (typeof(TK) == typeof(string)) {
                value = null;
                return false;
            }

            var str = key as string ?? throw new BundleArgumentNullException();
            var ret = SceneLoaders.TryGetValue(str, out var loader);
            value = loader as TT;
            return ret;
        }
    }
}