using System;
using System.Collections.Generic;
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

        public void RemoveHandler(ILoaderHandler loaderHandler) {
            switch (loaderHandler) {
                case Scene scene:
                    SceneHandlers.Remove(scene.SceneLoader.AssetPath);
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
        private Dictionary<string, RandomDelayLoader> RandomDelayLoaders { get; } =
            new Dictionary<string, RandomDelayLoader>();

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
                case RandomDelayLoader randomDelayLoader:
                    RandomDelayLoaders.Add(randomDelayLoader.Guid, randomDelayLoader);
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
                case RandomDelayLoader randomDelayLoader:
                    RandomDelayLoaders.Remove(randomDelayLoader.Guid);
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
            if (TryGetRandomDelayLoader(key, out value)) {
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
            foreach (var kv in RandomDelayLoaders) {
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

        private bool TryGetRandomDelayLoader<TK, TT>(TK key, out TT value) where TT: Loader {
            if (!typeof(RandomDelayLoader).IsAssignableFrom(typeof(TT))) {
                value = null;
                return false;
            }

            if (typeof(TK) != typeof(string)) {
                value = null;
                return false;
            }

            var str = key as string ?? throw new BundleArgumentNullException();
            var ret = RandomDelayLoaders.TryGetValue(str, out var loader);
            value = loader as TT;
            return ret;
        }

        //============================================================
        // Pipelines
        //============================================================

        private Dictionary<string, LoaderPipeline> Pipelines { get; } = new Dictionary<string, LoaderPipeline>();

        public void AddPipeline(LoaderPipeline pipeline) {
            if (Pipelines.ContainsKey(pipeline.Guid)) {
                return;
            }
            Pipelines.Add(pipeline.Guid, pipeline);
        }

        public void RemovePipeline(LoaderPipeline pipeline) {
            Pipelines.Remove(pipeline.Guid);
        }

        public bool TryGetPipeline(string guid, out LoaderPipeline pipeline) {
            return Pipelines.TryGetValue(guid, out pipeline);
        }

        public void ForEachPipeline(Action<LoaderPipeline> action) {
            foreach (var kv in Pipelines) {
                action(kv.Value);
            }
        }

        //============================================================
        // Links
        //============================================================

        // Object => Link Type => Link Instances
        private Dictionary<Object, Dictionary<Type, HashSet<LinkBase>>> Links { get; } =
            new Dictionary<Object, Dictionary<Type, HashSet<LinkBase>>>();

        public void AddLink(LinkBase link) {
            var linkedTarget = ((ILink)link).Target;
            if (!Links.TryGetValue(linkedTarget, out var dict)) {
                dict = Links[linkedTarget] = DictionaryPool<Type, HashSet<LinkBase>>.Get();
            }
            if (!dict.TryGetValue(link.GetType(), out var links)) {
                links = dict[link.GetType()] = HashSetPool<LinkBase>.Get();
            }
            if (link.Exclusive) {
                if (links.Count > 0) {
                    throw new BundleException(
                        $"Cannot add multiple exclusive link({link.GetType().FullName}) to an object.");
                }
            }
            links.Add(link);
        }

        public void RemoveLinksOfType<T>(Object linkedTarget) where T: LinkBase, new() {
            if (!Links.TryGetValue(linkedTarget, out var dict)) {
                return;
            }
            if (dict.TryGetValue(typeof(T), out var links)) {
                foreach (var link in links) {
                    ObjectPool<T>.Return(link as T);
                }
                HashSetPool<LinkBase>.Return(links);
            }

            dict.Remove(typeof(T));
            if (dict.Count > 0) {
                return;
            }
            Links.Remove(linkedTarget);

            DictionaryPool<Type, HashSet<LinkBase>>.Return(dict);
        }

        public void RemoveLinks(Object linkedTarget) {
            if (!Links.TryGetValue(linkedTarget, out var dict)) {
                return;
            }
            Links.Remove(linkedTarget);

            foreach (var kv in dict) {
                HashSetPool<LinkBase>.Return(kv.Value);
            }
            DictionaryPool<Type, HashSet<LinkBase>>.Return(dict);
        }

        public bool TryGetLinks<T>(Object linkedTarget, out HashSet<LinkBase> links) where T: LinkBase {
            if (Links.TryGetValue(linkedTarget, out var dict)) {
                if (dict.TryGetValue(typeof(T), out links)) {
                    return true;
                }
            }
            links = default;
            return false;
        }

        public void ForEachLinks(Action<Object, LinkBase> action) {
            foreach (var kv in Links) {
                foreach (var dict in kv.Value) {
                    foreach (var linkBase in dict.Value) {
                        action(kv.Key, linkBase);
                    }
                }
            }
        }
    }
}