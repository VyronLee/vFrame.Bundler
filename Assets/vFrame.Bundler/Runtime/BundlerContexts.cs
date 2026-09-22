// ------------------------------------------------------------
//         File: BundlerContexts.cs
//        Brief: Shared Bundler context: options, manifest, and registries of handlers, loaders, pipelines, and links.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:51:05
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Shared context owned by a <see cref="Bundler"/> instance, exposing options, the bundle manifest,
    ///     and registries of handlers, loaders, pipelines, and links to framework components.
    /// </summary>
    internal class BundlerContexts
    {
        /// <summary>The owning <see cref="Bundler"/> instance.</summary>
        public Bundler Bundler { get; set; }
        /// <summary>Options the bundler was constructed with.</summary>
        public BundlerOptions Options { get; set; }
        /// <summary>The loaded bundle manifest.</summary>
        public BundlerManifest Manifest { get; set; }

        //============================================================
        // Handlers
        //============================================================

        /// <summary>Registered scene handlers indexed by asset path.</summary>
        private Dictionary<string, Scene> SceneHandlers { get; } = new Dictionary<string, Scene>();

        /// <summary>
        ///     Registers a loader handler. Only <see cref="Scene"/> handlers are tracked currently.
        /// </summary>
        /// <param name="loaderHandler">Handler to register.</param>
        /// <exception cref="ArgumentException">A handler for the same asset path is already registered.</exception>
        public void AddHandler(ILoaderHandler loaderHandler)
        {
            switch (loaderHandler) {
                case Scene scene:
                    SceneHandlers.Add(scene.SceneLoader.AssetPath, scene);
                    break;
            }
        }

        /// <summary>
        ///     Unregisters a loader handler. Only <see cref="Scene"/> handlers are tracked currently.
        /// </summary>
        /// <param name="loaderHandler">Handler to unregister.</param>
        public void RemoveHandler(ILoaderHandler loaderHandler)
        {
            switch (loaderHandler) {
                case Scene scene:
                    SceneHandlers.Remove(scene.SceneLoader.AssetPath);
                    break;
            }
        }

        /// <summary>Invokes <paramref name="action"/> for every registered handler.</summary>
        /// <param name="action">Action to invoke.</param>
        public void ForEachHandler(Action<ILoaderHandler> action)
        {
            foreach (var kv in SceneHandlers) {
                action(kv.Value);
            }
        }

        //============================================================
        // Loaders
        //============================================================

        /// <summary>Asset loaders indexed by load key.</summary>
        private Dictionary<AssetLoadKey, AssetLoader> AssetLoaders { get; } =
            new Dictionary<AssetLoadKey, AssetLoader>();
        /// <summary>Scene loaders indexed by asset path.</summary>
        private Dictionary<string, SceneLoader> SceneLoaders { get; } =
            new Dictionary<string, SceneLoader>();
        /// <summary>Bundle loaders indexed by bundle path.</summary>
        private Dictionary<string, AssetBundleLoader> AssetBundleLoaders { get; } =
            new Dictionary<string, AssetBundleLoader>();
        /// <summary>Bundle loader groups indexed by main asset path.</summary>
        private Dictionary<string, AssetBundleLoaderGroup> AssetBundleLoaderGroups { get; } =
            new Dictionary<string, AssetBundleLoaderGroup>();
        /// <summary>Random-delay loaders indexed by GUID.</summary>
        private Dictionary<string, RandomDelayLoader> RandomDelayLoaders { get; } =
            new Dictionary<string, RandomDelayLoader>();

        /// <summary>Registers a loader in its type-specific registry.</summary>
        /// <param name="loader">Loader to register.</param>
        /// <exception cref="ArgumentException">A loader with the same key is already registered.</exception>
        public void AddLoader(Loader loader)
        {
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

        /// <summary>Unregisters a loader from its type-specific registry.</summary>
        /// <param name="loader">Loader to unregister.</param>
        public void RemoveLoader(Loader loader)
        {
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

        /// <summary>
        ///     Finds a registered loader of type <typeparamref name="TType"/> matching <paramref name="key"/>,
        ///     probing loader groups, bundle loaders, asset loaders, scene loaders, and random-delay loaders
        ///     in that order.
        /// </summary>
        /// <typeparam name="TKey">Key type (<see cref="AssetLoadKey"/> or asset path string).</typeparam>
        /// <typeparam name="TType">Expected loader type.</typeparam>
        /// <param name="key">Lookup key.</param>
        /// <param name="value">Found loader, or <c>null</c>.</param>
        /// <returns><c>true</c> if a matching loader is registered.</returns>
        public bool TryGetLoader<TKey, TType>(TKey key, out TType value) where TType : Loader
        {
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

        /// <summary>
        ///     Invokes <paramref name="action"/> for every registered loader, visiting groups, bundle loaders,
        ///     asset loaders, scene loaders, and random-delay loaders in that order.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        public void ForEachLoader(Action<Loader> action)
        {
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

        /// <summary>
        ///     Retrieves a registered <see cref="AssetBundleLoader"/>. Fails when <typeparamref name="TT"/> is not
        ///     assignable to it, <typeparamref name="TK"/> is not <see cref="string"/>, or no match is registered.
        /// </summary>
        /// <exception cref="BundleArgumentNullException">The key is a null string.</exception>
        private bool TryGetAssetBundleLoader<TK, TT>(TK key, out TT value) where TT : Loader
        {
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

        /// <summary>
        ///     Retrieves a registered <see cref="AssetBundleLoaderGroup"/>. Fails when <typeparamref name="TT"/> is not
        ///     assignable to it, <typeparamref name="TK"/> is not <see cref="string"/>, or no match is registered.
        /// </summary>
        /// <exception cref="BundleArgumentNullException">The key is a null string.</exception>
        private bool TryGetAssetBundleLoaderGroup<TK, TT>(TK key, out TT value) where TT : Loader
        {
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

        /// <summary>
        ///     Retrieves a registered <see cref="AssetLoader"/>. Fails when <typeparamref name="TT"/> is not assignable
        ///     to it, <typeparamref name="TK"/> is not <see cref="AssetLoadKey"/>, or no match is registered.
        /// </summary>
        /// <exception cref="BundleArgumentNullException">The key is a null <see cref="AssetLoadKey"/>.</exception>
        private bool TryGetAssetLoader<TK, TT>(TK key, out TT value) where TT : Loader
        {
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

        /// <summary>
        ///     Retrieves a registered <see cref="SceneLoader"/>. Fails when <typeparamref name="TT"/> is not assignable
        ///     to it, <typeparamref name="TK"/> is not <see cref="string"/>, or no match is registered.
        /// </summary>
        /// <exception cref="BundleArgumentNullException">The key is a null string.</exception>
        private bool TryGetSceneLoader<TK, TT>(TK key, out TT value) where TT : Loader
        {
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

        /// <summary>
        ///     Retrieves a registered <see cref="RandomDelayLoader"/>. Fails when <typeparamref name="TT"/> is not
        ///     assignable to it, <typeparamref name="TK"/> is not <see cref="string"/>, or no match is registered.
        /// </summary>
        /// <exception cref="BundleArgumentNullException">The key is a null string.</exception>
        private bool TryGetRandomDelayLoader<TK, TT>(TK key, out TT value) where TT : Loader
        {
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

        /// <summary>Loader pipelines indexed by GUID.</summary>
        private Dictionary<string, LoaderPipeline> Pipelines { get; } = new Dictionary<string, LoaderPipeline>();

        /// <summary>Registers a pipeline. Duplicates with the same GUID are silently ignored.</summary>
        /// <param name="pipeline">Pipeline to register.</param>
        public void AddPipeline(LoaderPipeline pipeline)
        {
            if (Pipelines.ContainsKey(pipeline.Guid)) {
                return;
            }
            Pipelines.Add(pipeline.Guid, pipeline);
        }

        /// <summary>Unregisters a pipeline.</summary>
        /// <param name="pipeline">Pipeline to unregister.</param>
        public void RemovePipeline(LoaderPipeline pipeline)
        {
            Pipelines.Remove(pipeline.Guid);
        }

        /// <summary>Attempts to retrieve a registered pipeline by GUID.</summary>
        /// <param name="guid">Pipeline GUID.</param>
        /// <param name="pipeline">Found pipeline, or <c>null</c>.</param>
        /// <returns><c>true</c> if a pipeline with the GUID is registered.</returns>
        public bool TryGetPipeline(string guid, out LoaderPipeline pipeline)
        {
            return Pipelines.TryGetValue(guid, out pipeline);
        }

        /// <summary>Invokes <paramref name="action"/> for every registered pipeline.</summary>
        /// <param name="action">Action to invoke.</param>
        public void ForEachPipeline(Action<LoaderPipeline> action)
        {
            foreach (var kv in Pipelines) {
                action(kv.Value);
            }
        }

        //============================================================
        // Links
        //============================================================

        /// <summary>Registered links grouped by target <see cref="Object"/>, then by link type.</summary>
        private Dictionary<Object, Dictionary<Type, HashSet<LinkBase>>> Links { get; } =
            new Dictionary<Object, Dictionary<Type, HashSet<LinkBase>>>();

        /// <summary>
        ///     Registers a link on its target object, reusing pooled containers. Exclusive links permit only
        ///     one instance per target and link type.
        /// </summary>
        /// <param name="link">Link to register; its target is taken from <see cref="ILink.Target"/>.</param>
        /// <exception cref="BundleException">An exclusive link already exists for the target and link type.</exception>
        public void AddLink(LinkBase link)
        {
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

        /// <summary>
        ///     Removes all links of type <typeparamref name="T"/> from <paramref name="linkedTarget"/> and returns
        ///     them, with their container, to the pools.
        /// </summary>
        /// <typeparam name="T">Link type to remove.</typeparam>
        /// <param name="linkedTarget">Object the links are attached to.</param>
        public void RemoveLinksOfType<T>(Object linkedTarget) where T : LinkBase, new()
        {
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

        /// <summary>
        ///     Removes every link attached to <paramref name="linkedTarget"/> and returns containers to the pools.
        /// </summary>
        /// <param name="linkedTarget">Object to detach links from.</param>
        public void RemoveLinks(Object linkedTarget)
        {
            if (!Links.TryGetValue(linkedTarget, out var dict)) {
                return;
            }
            Links.Remove(linkedTarget);

            foreach (var kv in dict) {
                HashSetPool<LinkBase>.Return(kv.Value);
            }
            DictionaryPool<Type, HashSet<LinkBase>>.Return(dict);
        }

        /// <summary>
        ///     Attempts to get the set of links of type <typeparamref name="T"/> attached to the target.
        ///     The returned set is shared registry state; do not modify it.
        /// </summary>
        /// <typeparam name="T">Link type to look up.</typeparam>
        /// <param name="linkedTarget">Object the links are attached to.</param>
        /// <param name="links">Link set when found; <c>null</c> otherwise.</param>
        /// <returns><c>true</c> if any link of the type is attached to the target.</returns>
        public bool TryGetLinks<T>(Object linkedTarget, out HashSet<LinkBase> links) where T : LinkBase
        {
            if (Links.TryGetValue(linkedTarget, out var dict)) {
                if (dict.TryGetValue(typeof(T), out links)) {
                    return true;
                }
            }
            links = default;
            return false;
        }

        /// <summary>Invokes <paramref name="action"/> with the target object and each registered link.</summary>
        /// <param name="action">Action to invoke.</param>
        public void ForEachLinks(Action<Object, LinkBase> action)
        {
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