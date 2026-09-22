// ------------------------------------------------------------
//         File: LoadSystem.cs
//        Brief: Core load facade that builds mode-specific loader pipelines for asset and scene
//               requests, reuses cached loaders, and drives pipeline, loader and handler updates.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:01:39
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Core load facade that builds mode-specific loader pipelines for asset and scene requests,
    /// reuses cached loaders, and drives per-frame updates of pipelines, loaders and handlers.
    /// </summary>
    internal class LoadSystem : BundlerSystem
    {
        /// <summary>Cached delegate for <see cref="UpdateLoader"/>, avoids per-update allocation.</summary>
        private readonly Action<Loader> _updateLoaderAction;

        /// <summary>Cached delegate for <see cref="UpdateHandler"/>, avoids per-update allocation.</summary>
        private readonly Action<ILoaderHandler> _updateHandlerAction;

        /// <summary>Cached delegate for <see cref="UpdatePipeline"/>, avoids per-update allocation.</summary>
        private readonly Action<LoaderPipeline> _updatePipelineAction;

        /// <summary>
        /// Initializes the load system with the shared bundler contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared contexts of the bundler instance.</param>
        public LoadSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {
            _updateLoaderAction = UpdateLoader;
            _updateHandlerAction = UpdateHandler;
            _updatePipelineAction = UpdatePipeline;
        }

        /// <summary>
        /// Called when the system is destroyed. No explicit cleanup required; resources are
        /// owned by the bundler contexts.
        /// </summary>
        protected override void OnDestroy()
        {

        }

        /// <summary>Gets the bundler loading mode shortcut, deciding which loader types pipelines use.</summary>
        private BundlerMode BundlerMode => BundlerContexts.Options.Mode;

        /// <summary>
        /// Ensures the given asset path is managed by the bundler manifest (MainRules).
        /// </summary>
        /// <param name="path">Asset path to check.</param>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed.</exception>
        private void ThrowIfAssetNotManaged(string path)
        {
            if (BundlerContexts.Manifest.Assets.TryGetValue(path, out var mainBundle)) {
                return;
            }
            throw new BundleNoneConfigurationException($"Asset path is not managed by MainRules: {path}");
        }

        /// <summary>
        /// Loads an asset synchronously, reusing a cached loader for the same asset key when available.
        /// </summary>
        /// <param name="path">Managed asset path.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <param name="loadType">Asset loading strategy.</param>
        /// <returns>Asset handler wrapping the loaded asset.</returns>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed by MainRules.</exception>
        /// <exception cref="BundleAssetLoadFailedException">Thrown when the load pipeline fails.</exception>
        public Asset LoadAsset(string path, Type type, AssetLoadType loadType)
        {
            ThrowIfAssetNotManaged(path);
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, type), out AssetLoader loader)) {
                if (!CreateAssetLoadSyncPipeline(path, type, loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<Asset>(loader);
        }

        /// <summary>
        /// Loads an asset asynchronously, reusing a cached loader for the same asset key when available.
        /// </summary>
        /// <param name="path">Managed asset path.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <param name="loadType">Asset loading strategy.</param>
        /// <returns>Async asset handler wrapping the asset being loaded.</returns>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed by MainRules.</exception>
        /// <exception cref="BundleAssetLoadFailedException">Thrown when the load pipeline fails.</exception>
        public AssetAsync LoadAssetAsync(string path, Type type, AssetLoadType loadType)
        {
            ThrowIfAssetNotManaged(path);
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, type), out AssetLoader loader)) {
                if (!CreateAssetLoadAsyncPipeline(path, type, loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<AssetAsync>(loader);
        }

        /// <summary>
        /// Loads an asset synchronously, reusing a cached loader for the same asset key when available.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">Managed asset path.</param>
        /// <param name="loadType">Asset loading strategy.</param>
        /// <returns>Asset handler wrapping the loaded asset.</returns>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed by MainRules.</exception>
        /// <exception cref="BundleAssetLoadFailedException">Thrown when the load pipeline fails.</exception>
        public Asset<T> LoadAsset<T>(string path, AssetLoadType loadType) where T : Object
        {
            ThrowIfAssetNotManaged(path);
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, typeof(T)), out AssetLoader loader)) {
                if (!CreateAssetLoadSyncPipeline(path, typeof(T), loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<Asset<T>>(loader);
        }

        /// <summary>
        /// Loads an asset asynchronously, reusing a cached loader for the same asset key when available.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">Managed asset path.</param>
        /// <param name="loadType">Asset loading strategy.</param>
        /// <returns>Async asset handler wrapping the asset being loaded.</returns>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed by MainRules.</exception>
        /// <exception cref="BundleAssetLoadFailedException">Thrown when the load pipeline fails.</exception>
        public AssetAsync<T> LoadAssetAsync<T>(string path, AssetLoadType loadType) where T : Object
        {
            ThrowIfAssetNotManaged(path);
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, typeof(T)), out AssetLoader loader)) {
                if (!CreateAssetLoadAsyncPipeline(path, typeof(T), loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<AssetAsync<T>>(loader);
        }

        /// <summary>
        /// Loads a scene synchronously, reusing a cached loader for the same scene when available.
        /// </summary>
        /// <param name="path">Managed scene asset path.</param>
        /// <param name="loadSceneMode">Scene loading mode (single or additive).</param>
        /// <returns>Scene handler wrapping the loaded scene.</returns>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed by MainRules.</exception>
        /// <exception cref="BundleAssetLoadFailedException">Thrown when the load pipeline fails.</exception>
        public Scene LoadScene(string path, LoadSceneMode loadSceneMode)
        {
            ThrowIfAssetNotManaged(path);
            if (!BundlerContexts.TryGetLoader(path, out SceneLoader loader)) {
                if (!CreateSceneLoadSyncPipeline(path, loadSceneMode).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<Scene>(loader);
        }

        /// <summary>
        /// Loads a scene asynchronously, reusing a cached loader for the same scene when available.
        /// </summary>
        /// <param name="path">Managed scene asset path.</param>
        /// <param name="loadSceneMode">Scene loading mode (single or additive).</param>
        /// <returns>Async scene handler wrapping the scene being loaded.</returns>
        /// <exception cref="BundleNoneConfigurationException">Thrown when the path is not managed by MainRules.</exception>
        /// <exception cref="BundleAssetLoadFailedException">Thrown when the load pipeline fails.</exception>
        public SceneAsync LoadSceneAsync(string path, LoadSceneMode loadSceneMode)
        {
            ThrowIfAssetNotManaged(path);
            if (!BundlerContexts.TryGetLoader(path, out SceneLoader loader)) {
                if (!CreateSceneLoadAsyncPipeline(path, loadSceneMode).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<SceneAsync>(loader);
        }

        /// <summary>
        /// Builds a synchronous asset load pipeline configured for the current bundler mode,
        /// and registers it with the bundler contexts.
        /// </summary>
        /// <param name="path">Managed asset path.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <param name="loadType">Asset loading strategy.</param>
        /// <returns>The registered pipeline, ready to be started.</returns>
        private LoaderPipeline CreateAssetLoadSyncPipeline(string path, Type type, AssetLoadType loadType)
        {
            var loaderContexts = new LoaderContexts {
                AssetLoadType = loadType,
                AssetPath = path,
                AssetType = type
            };
            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    pipeline.Add<AssetDatabaseAssetLoaderSync>();
                    break;
                case BundlerMode.Resources:
                    pipeline.Add<ResourcesAssetLoaderSync>();
                    break;
                case BundlerMode.AssetBundle:
                    if (BundlerContexts.TryGetLoader(path, out AssetBundleLoaderGroup loaderGroup)) {
                        pipeline.Add(loaderGroup);
                    }
                    else {
                        pipeline.Add<AssetBundleLoaderGroupSync>();
                    }
                    pipeline.Add<AssetBundleAssetLoaderSync>();
                    break;
            }
            BundlerContexts.AddPipeline(pipeline);
            return pipeline;
        }

        /// <summary>
        /// Builds an asynchronous asset load pipeline configured for the current bundler mode,
        /// and registers it with the bundler contexts. In AssetDatabase mode a random delay
        /// loader is prepended to simulate real loading latency.
        /// </summary>
        /// <param name="path">Managed asset path.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <param name="loadType">Asset loading strategy.</param>
        /// <returns>The registered pipeline, ready to be started.</returns>
        private LoaderPipeline CreateAssetLoadAsyncPipeline(string path, Type type, AssetLoadType loadType)
        {
            var loaderContexts = new LoaderContexts {
                AssetLoadType = loadType,
                AssetPath = path,
                AssetType = type
            };
            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    pipeline.Add<RandomDelayLoader>();
                    pipeline.Add<AssetDatabaseAssetLoaderSync>();
                    break;
                case BundlerMode.Resources:
                    pipeline.Add<ResourcesAssetLoaderAsync>();
                    break;
                case BundlerMode.AssetBundle:
                    if (BundlerContexts.TryGetLoader(path, out AssetBundleLoaderGroup loaderGroup)) {
                        pipeline.Add(loaderGroup);
                    }
                    else {
                        pipeline.Add<AssetBundleLoaderGroupAsync>();
                    }
                    pipeline.Add<AssetBundleAssetLoaderAsync>();
                    break;
            }
            BundlerContexts.AddPipeline(pipeline);
            return pipeline;
        }

        /// <summary>
        /// Builds a synchronous scene load pipeline configured for the current bundler mode,
        /// and registers it with the bundler contexts.
        /// </summary>
        /// <param name="path">Managed scene asset path.</param>
        /// <param name="sceneMode">Scene loading mode (single or additive).</param>
        /// <returns>The registered pipeline, ready to be started.</returns>
        private LoaderPipeline CreateSceneLoadSyncPipeline(string path, LoadSceneMode sceneMode)
        {
            var loaderContexts = new LoaderContexts {
                AssetPath = path,
                SceneMode = sceneMode,
            };

            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    pipeline.Add<AssetDatabaseSceneLoaderSync>();
                    break;
                case BundlerMode.Resources:
                    pipeline.Add<ResourcesSceneLoaderSync>();
                    break;
                case BundlerMode.AssetBundle:
                    pipeline.Add<AssetBundleLoaderGroupSync>();
                    pipeline.Add<AssetBundleSceneLoaderSync>();
                    break;
            }
            BundlerContexts.AddPipeline(pipeline);
            return pipeline;
        }

        /// <summary>
        /// Builds an asynchronous scene load pipeline configured for the current bundler mode,
        /// and registers it with the bundler contexts. In AssetDatabase mode a random delay
        /// loader is prepended to simulate real loading latency.
        /// </summary>
        /// <param name="path">Managed scene asset path.</param>
        /// <param name="sceneMode">Scene loading mode (single or additive).</param>
        /// <returns>The registered pipeline, ready to be started.</returns>
        private LoaderPipeline CreateSceneLoadAsyncPipeline(string path, LoadSceneMode sceneMode)
        {
            var loaderContexts = new LoaderContexts {
                AssetPath = path,
                SceneMode = sceneMode,
            };

            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    pipeline.Add<RandomDelayLoader>();
                    pipeline.Add<AssetDatabaseSceneLoaderAsync>();
                    break;
                case BundlerMode.Resources:
                    pipeline.Add<ResourcesSceneLoaderAsync>();
                    break;
                case BundlerMode.AssetBundle:
                    pipeline.Add<AssetBundleLoaderGroupAsync>();
                    pipeline.Add<AssetBundleSceneLoaderAsync>();
                    break;
            }
            BundlerContexts.AddPipeline(pipeline);
            return pipeline;
        }

        /// <summary>
        /// Creates a loader handler bound to the given loader and registers it with the bundler contexts.
        /// </summary>
        /// <typeparam name="T">Type of handler to create.</typeparam>
        /// <param name="loader">Loader driving the handler.</param>
        /// <returns>The registered handler instance.</returns>
        private T CreateHandler<T>(Loader loader) where T : ILoaderHandler, new()
        {
            var ret = new T {
                Loader = loader,
                BundlerContexts = BundlerContexts
            };
            BundlerContexts.AddHandler(ret);
            return ret;
        }

        /// <summary>
        /// Drives per-frame updates of pipelines, loaders and handlers, in that order.
        /// </summary>
        protected override void OnUpdate()
        {
            BundlerContexts.ForEachPipeline(_updatePipelineAction);
            BundlerContexts.ForEachLoader(_updateLoaderAction);
            BundlerContexts.ForEachHandler(_updateHandlerAction);
        }

        /// <summary>
        /// Updates a single loader.
        /// </summary>
        /// <param name="loader">Loader to update.</param>
        private static void UpdateLoader(Loader loader)
        {
            loader.Update();
        }

        /// <summary>
        /// Updates a single loader handler.
        /// </summary>
        /// <param name="handler">Handler to update.</param>
        private static void UpdateHandler(ILoaderHandler handler)
        {
            handler.Update();
        }

        /// <summary>
        /// Updates a single loader pipeline.
        /// </summary>
        /// <param name="pipeline">Pipeline to update.</param>
        private static void UpdatePipeline(LoaderPipeline pipeline)
        {
            pipeline.Update();
        }
    }
}