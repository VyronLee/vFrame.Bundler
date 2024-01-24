using System;
using System.Collections.Generic;
using UnityEngine;
using vFrame.Bundler.Exception;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class BundlerContexts
    {
        public Bundler Bundler { get; set; }
        public BundlerOptions Options { get; set; }
        public BundlerManifest Manifest { get; set; }

        //============================================================
        // Handlers
        //============================================================

        private Dictionary<string, Scene> SceneHandlers { get; } = new Dictionary<string, Scene>();

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

        //============================================================
        // Loaders
        //============================================================

        private Dictionary<AssetLoadKey, AssetLoader> AssetLoaders { get; } =
            new Dictionary<AssetLoadKey, AssetLoader>();
        private Dictionary<string, SceneLoader> SceneLoaders { get; } =
            new Dictionary<string, SceneLoader>();
        private Dictionary<string, AssetBundleLoader> AssetBundleLoaders { get; } =
            new Dictionary<string, AssetBundleLoader>();
        private Dictionary<string, AssetBundleLoaderGroup> AssetBundleLoaderGroups { get; } =
            new Dictionary<string, AssetBundleLoaderGroup>();

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
                    AssetBundleLoaderGroups.Add(bundlerLoaderGroup.MainAssetPath, bundlerLoaderGroup);
                    break;
            }
        }

        public void RemoveLoader(Loader loader) {
            switch (loader) {
                case AssetLoader assetLoader:
                    AssetLoaders.Remove(assetLoader.AssetLoadKey);
                    break;
                case SceneLoader sceneLoader:
                    SceneLoaders.Remove(sceneLoader.AssetPath);
                    break;
                case AssetBundleLoader bundlerLoader:
                    AssetBundleLoaders.Remove(bundlerLoader.BundlePath);
                    break;
                case AssetBundleLoaderGroup bundlerLoaderGroup:
                    AssetBundleLoaderGroups.Remove(bundlerLoaderGroup.MainAssetPath);
                    break;
            }
        }

        public bool TryGetLoader<TKey, TType>(TKey key, out TType value) where TType: Loader {
            if (TryGetAssetBundleLoaderGroup(key, out value)) {
                return true;
            }
            if (TryGetAssetBundleLoader(key, out value)) {
                return true;
            }
            if (TryGetAssetLoader(key, out value)) {
                return true;
            }
            if (TryGetSceneLoader(key, out value)) {
                return true;
            }
            value = default(TType);
            return false;
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

            if (typeof(TK) != typeof(string)) {
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

            if (typeof(TK) != typeof(string)) {
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

            if (typeof(TK) != typeof(AssetLoadKey)) {
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

            if (typeof(TK) != typeof(string)) {
                value = null;
                return false;
            }

            var str = key as string ?? throw new BundleArgumentNullException();
            var ret = SceneLoaders.TryGetValue(str, out var loader);
            value = loader as TT;
            return ret;
        }

        //============================================================
        // Proxies
        //============================================================

        private Dictionary<Component, Dictionary<Type, PropertyProxy>> Proxies { get; } =
            new Dictionary<Component, Dictionary<Type, PropertyProxy>>();
        private Dictionary<Loader, HashSet<Object>> LinkedObjects { get; } =
            new Dictionary<Loader, HashSet<Object>>();

        public void AddProxy(Component component, PropertyProxy proxy) {
            if (!Proxies.TryGetValue(component, out var dict)) {
                dict = Proxies[component] = DictionaryPool<Type, PropertyProxy>.Get();
            }
            if (dict.TryGetValue(proxy.GetType(), out var current)) {
                throw new ArgumentException("An element already exist with same type: " + proxy.GetType().Name);
            }
            dict.Add(proxy.GetType(), proxy);
        }

        public PropertyProxy RemoveProxy(Component component, Type type) {
            if (!Proxies.TryGetValue(component, out var dict)) {
                return null;
            }
            var proxy = dict[type];
            dict.Remove(type);

            if (dict.Count > 0) {
                return proxy;
            }
            Proxies.Remove(component);

            DictionaryPool<Type, PropertyProxy>.Return(dict);
            return proxy;
        }

        public bool TryGetProxy(Component component, Type type, out PropertyProxy proxy) {
            if (Proxies.TryGetValue(component, out var dict)) {
                return dict.TryGetValue(type, out proxy);
            }
            proxy = null;
            return false;
        }

        public void AddLinkedObject(Loader loader, Object target) {
            if (!LinkedObjects.TryGetValue(loader, out var set)) {
                set = LinkedObjects[loader] = HashSetPool<Object>.Get();
            }
            set.Add(target);
        }

        public void RemoveLinkedObject(Loader loader, Object target) {
            if (!LinkedObjects.TryGetValue(loader, out var set)) {
                return;
            }
            set.Remove(target);

            if (set.Count > 0) {
                return;
            }
            LinkedObjects.Remove(loader);

            HashSetPool<Object>.Return(set);
        }

        public void ForEachLinkedObject(Action<Loader, HashSet<Object>> action) {
            foreach (var kv in LinkedObjects) {
                action(kv.Key, kv.Value);
            }
        }
    }
}