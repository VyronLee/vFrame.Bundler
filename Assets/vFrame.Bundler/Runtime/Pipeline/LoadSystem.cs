// ------------------------------------------------------------
//         File: LoadSystem.cs
//        Brief: LoadSystem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 22:8
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Exception;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class LoadSystem : BundlerSystem
    {
        private readonly List<LoaderPipeline> _pipelines;
        private readonly Action<Loader> _updateLoaderAction;
        private readonly Action<ILoaderHandler> _updateHandlerAction;

        public LoadSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _pipelines = new List<LoaderPipeline>(512);
            _updateLoaderAction = UpdateLoader;
            _updateHandlerAction = UpdateHandler;
        }

        protected override void OnDestroy() {

        }

        private BundlerMode BundlerMode => BundlerContexts.Options.Mode;

        public Asset LoadAsset(string path, Type type, AssetLoadType loadType) {
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, type), out AssetLoader loader)) {
                if (!CreateAssetLoadSyncPipeline(path, type, loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<Asset>(loader);
        }

        public AssetAsync LoadAssetAsync(string path, Type type, AssetLoadType loadType) {
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, type), out AssetLoader loader)) {
                if (!CreateAssetLoadAsyncPipeline(path, type, loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<AssetAsync>(loader);
        }

        public Asset<T> LoadAsset<T>(string path, AssetLoadType loadType) where T : Object {
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, typeof(T)), out AssetLoader loader)) {
                if (!CreateAssetLoadSyncPipeline(path, typeof(T), loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<Asset<T>>(loader);
        }

        public AssetAsync<T> LoadAssetAsync<T>(string path, AssetLoadType loadType) where T : Object {
            if (!BundlerContexts.TryGetLoader((AssetLoadKey)(path, typeof(T)), out AssetLoader loader)) {
                if (!CreateAssetLoadAsyncPipeline(path, typeof(T), loadType).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<AssetAsync<T>>(loader);
        }

        public Scene LoadScene(string path, LoadSceneMode loadSceneMode) {
            if (!BundlerContexts.TryGetLoader(path, out SceneLoader loader)) {
                if (!CreateSceneLoadSyncPipeline(path, loadSceneMode).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<Scene>(loader);
        }

        public SceneAsync LoadSceneAsync(string path, LoadSceneMode loadSceneMode) {
            if (!BundlerContexts.TryGetLoader(path, out SceneLoader loader)) {
                if (!CreateSceneLoadAsyncPipeline(path, loadSceneMode).Startup(out loader)) {
                    throw new BundleAssetLoadFailedException(path);
                }
            }
            return CreateHandler<SceneAsync>(loader);
        }

        private LoaderPipeline CreateAssetLoadSyncPipeline(string path, Type type, AssetLoadType loadType) {
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
                    pipeline.Add<AssetBundleLoaderGroupSync>();
                    pipeline.Add<AssetBundleAssetLoaderSync>();
                    break;
            }
            _pipelines.Add(pipeline);
            return pipeline;
        }

        private LoaderPipeline CreateAssetLoadAsyncPipeline(string path, Type type, AssetLoadType loadType) {
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
                    pipeline.Add<AssetBundleLoaderGroupAsync>();
                    pipeline.Add<AssetBundleAssetLoaderAsync>();
                    break;
            }
            _pipelines.Add(pipeline);
            return pipeline;
        }

        private LoaderPipeline CreateSceneLoadSyncPipeline(string path, LoadSceneMode sceneMode) {
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
            _pipelines.Add(pipeline);
            return pipeline;
        }

        private LoaderPipeline CreateSceneLoadAsyncPipeline(string path, LoadSceneMode sceneMode) {
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
            _pipelines.Add(pipeline);
            return pipeline;
        }

        private T CreateHandler<T>(Loader loader) where T: ILoaderHandler, new() {
            var ret = new T {
                Loader = loader,
                BundlerContexts = BundlerContexts
            };
            BundlerContexts.AddHandler(ret);
            return ret;
        }

        protected override void OnUpdate() {
            UpdateAndRemovePipelines();
            BundlerContexts.ForEachLoader(_updateLoaderAction);
            BundlerContexts.ForEachHandler(_updateHandlerAction);
        }

        private void UpdateAndRemovePipelines() {
            for (var i = _pipelines.Count - 1; i >= 0; i--) {
                var pipeline = _pipelines[i];
                pipeline.Update();

                if (!pipeline.IsDone) {
                    continue;
                }
                _pipelines.RemoveAt(i);

                Facade.GetSystem<LogSystem>().LogInfo("Removing finished pipeline: {0}", pipeline.Last());
            }
        }

        private static void UpdateLoader(Loader loader) {
            loader.Update();
        }

        private static void UpdateHandler(ILoaderHandler handler) {
            handler.Update();
        }
    }
}