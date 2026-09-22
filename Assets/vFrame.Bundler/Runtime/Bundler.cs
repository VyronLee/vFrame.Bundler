// ------------------------------------------------------------
//         File: Bundler.cs
//        Brief: Core facade of the asset loading system: hosts embedded subsystems and exposes asset/scene load APIs.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:51:01
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Core facade of the asset loading system. Creates and owns the embedded subsystems and exposes
    ///     synchronous and asynchronous asset and scene loading APIs.
    /// </summary>
    public class Bundler : IBundler
    {
        /// <summary>Types of the systems embedded in every bundler, instantiated in this order at construction.</summary>
        private static readonly Type[] _embedSystems = {
            typeof(LogSystem),
            typeof(LoadSystem),
            typeof(LinkSystem),
            typeof(CollectSystem),
            typeof(ProfileSystem)
        };

        /// <summary>Shared context (options, manifest, bundler back-reference) handed to every system.</summary>
        private readonly BundlerContexts _contexts;
        /// <summary>Living system instances keyed by their concrete type.</summary>
        private readonly Dictionary<Type, BundlerSystem> _systems = new Dictionary<Type, BundlerSystem>();

        /// <summary>
        ///     Creates the bundler, builds the shared context and instantiates all embedded systems.
        /// </summary>
        /// <param name="manifest">Manifest describing the built assets; required.</param>
        /// <param name="options">Runtime options; a default instance is created when null.</param>
        public Bundler(BundlerManifest manifest, BundlerOptions options = null)
        {
            options = options ?? new BundlerOptions();
            _contexts = new BundlerContexts {
                Options = options,
                Manifest = manifest,
                Bundler = this
            };
            InitializeSystems();
        }

        /// <summary>
        ///     Instantiates every embedded system with the shared context and applies the custom log handler.
        /// </summary>
        /// <exception cref="ArgumentException">An embedded type does not derive from <see cref="BundlerSystem"/>.</exception>
        private void InitializeSystems()
        {
            var baseType = typeof(BundlerSystem);
            foreach (var type in _embedSystems) {
                if (!baseType.IsAssignableFrom(type)) {
                    throw new ArgumentException("Cannot create bundler system of type: " + type);
                }
                _systems[type] = Activator.CreateInstance(type, _contexts) as BundlerSystem;
            }

            if (null != _contexts.Options.LogHandler) {
                GetSystem<LogSystem>().SetLogHandler(_contexts.Options.LogHandler);
            }
        }

        /// <summary>
        ///     Destroys all systems and clears the system registry. The bundler must not be used afterwards.
        /// </summary>
        public void Destroy()
        {
            foreach (var system in _systems.Select(kv => kv.Value)) {
                system.Destroy();
            }
            _systems.Clear();
        }

        /// <summary>
        ///     Gets the registered system instance of the given concrete type.
        /// </summary>
        /// <typeparam name="T">Concrete <see cref="BundlerSystem"/> type.</typeparam>
        /// <returns>The system instance registered at construction time.</returns>
        /// <exception cref="KeyNotFoundException">No system of type <typeparamref name="T"/> is registered.</exception>
        internal T GetSystem<T>() where T : BundlerSystem
        {
            return _systems[typeof(T)] as T;
        }

        /// <summary>
        ///     Loads a single main asset at the given path.
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded asset alive.</returns>
        public Asset LoadAsset(string path, Type type)
        {
            return GetSystem<LoadSystem>().LoadAsset(path, type, AssetLoadType.LoadAsset);
        }

        /// <summary>
        ///     Loads a single main asset at the given path asynchronously.
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        public AssetAsync LoadAssetAsync(string path, Type type)
        {
            return GetSystem<LoadSystem>().LoadAssetAsync(path, type, AssetLoadType.LoadAsset);
        }

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets (e.g. sprites of a sheet, meshes of an FBX).
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the main asset to load.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded assets alive.</returns>
        public Asset LoadAssetWithSubAssets(string path, Type type)
        {
            return GetSystem<LoadSystem>().LoadAsset(path, type, AssetLoadType.LoadAssetWithSubAsset);
        }

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets asynchronously.
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the main asset to load.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        public AssetAsync LoadAssetWithSubAssetsAsync(string path, Type type)
        {
            return GetSystem<LoadSystem>().LoadAssetAsync(path, type, AssetLoadType.LoadAssetWithSubAsset);
        }

        /// <summary>
        ///     Loads a single main asset at the given path, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded asset alive.</returns>
        public Asset<T> LoadAsset<T>(string path) where T : Object
        {
            return GetSystem<LoadSystem>().LoadAsset<T>(path, AssetLoadType.LoadAsset);
        }

        /// <summary>
        ///     Loads a single main asset at the given path asynchronously, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        public AssetAsync<T> LoadAssetAsync<T>(string path) where T : Object
        {
            return GetSystem<LoadSystem>().LoadAssetAsync<T>(path, AssetLoadType.LoadAsset);
        }

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the main asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded assets alive.</returns>
        public Asset<T> LoadAssetWithSubAssets<T>(string path) where T : Object
        {
            return GetSystem<LoadSystem>().LoadAsset<T>(path, AssetLoadType.LoadAssetWithSubAsset);
        }

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets asynchronously, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the main asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        public AssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object
        {
            return GetSystem<LoadSystem>().LoadAssetAsync<T>(path, AssetLoadType.LoadAssetWithSubAsset);
        }

        /// <summary>
        ///     Loads the scene at the given path.
        /// </summary>
        /// <param name="path">Scene path as listed in the manifest.</param>
        /// <param name="mode">Scene loading mode, single or additive.</param>
        /// <returns>Wrapper for the loaded scene.</returns>
        public Scene LoadScene(string path, LoadSceneMode mode)
        {
            return GetSystem<LoadSystem>().LoadScene(path, mode);
        }

        /// <summary>
        ///     Loads the scene at the given path asynchronously.
        /// </summary>
        /// <param name="path">Scene path as listed in the manifest.</param>
        /// <param name="mode">Scene loading mode, single or additive.</param>
        /// <returns>Wrapper tracking the asynchronous scene load.</returns>
        public SceneAsync LoadSceneAsync(string path, LoadSceneMode mode)
        {
            return GetSystem<LoadSystem>().LoadSceneAsync(path, mode);
        }

        /// <summary>
        ///     Updates all systems; must be called every frame before <see cref="Collect"/>.
        /// </summary>
        public void Update()
        {
            foreach (var kv in _systems) {
                var system = kv.Value;
                system.Update();
            }
        }

        /// <summary>
        ///     Releases assets whose references dropped to zero; call every frame after <see cref="Update"/>.
        /// </summary>
        public void Collect()
        {
            GetSystem<CollectSystem>().Collect();
        }

        /// <summary>
        ///     Sets the verbosity level of the bundler logger.
        /// </summary>
        /// <param name="level">Log level value interpreted by the log system.</param>
        public void SetLogLevel(int level)
        {
            GetSystem<LogSystem>().SetLogLevel(level);
        }
    }
}