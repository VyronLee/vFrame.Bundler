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
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class LoadSystem : BundlerSystem
    {
        public LoadSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
        }

        protected override void OnDestroy() {

        }

        private BundlerMode BundlerMode => BundlerContexts.Options.Mode;

        public IAsset LoadAsset(string path, Type type, AssetLoadType loadType) {
            return CreateAssetLoadPipeline(path, type, loadType).Startup<Asset>();
        }

        public IAssetAsync LoadAssetAsync(string path, Type type, AssetLoadType loadType) {
            return CreateAssetLoadPipelineAsync(path, type, loadType).Startup<AssetAsync>();
        }

        public IAsset<T> LoadAsset<T>(string path, AssetLoadType loadType) where T : Object {
            return CreateAssetLoadPipeline(path, typeof(T), loadType).Startup<Asset<T>>();
        }

        public IAssetAsync<T> LoadAssetAsync<T>(string path, AssetLoadType loadType) where T : Object {
            return CreateAssetLoadPipelineAsync(path, typeof(T), loadType).Startup<AssetAsync<T>>();
        }

        private LoaderPipeline CreateAssetLoadPipeline(string path, Type type, AssetLoadType loadType) {
            var loaderContexts = new LoaderContexts {
                AssetLoadType = loadType,
                AssetPath = path,
                AssetType = type
            };
            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    break;
                case BundlerMode.Resources:
                    break;
                case BundlerMode.AssetBundle:
                    pipeline.Add<AssetBundleLoaderGroup>();
                    pipeline.Add<AssetBundleAssetLoader>();
                    break;
            }
            return pipeline;
        }

        private LoaderPipeline CreateAssetLoadPipelineAsync(string path, Type type, AssetLoadType loadType) {
            var loaderContexts = new LoaderContexts {
                AssetLoadType = loadType,
                AssetPath = path,
                AssetType = type
            };
            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    break;
                case BundlerMode.Resources:
                    break;
                case BundlerMode.AssetBundle:
                    pipeline.Add<AssetBundleLoaderGroupAsync>();
                    pipeline.Add<AssetBundleAssetLoaderAsync>();
                    break;
            }
            return pipeline;
        }

        private LoaderPipeline CreateSceneLoadPipeline(string path, Type type, LoadSceneMode sceneMode) {
            var loaderContexts = new LoaderContexts {
                AssetPath = path,
                SceneMode = sceneMode,
            };

            var pipeline = new LoaderPipeline(BundlerContexts, loaderContexts);
            switch (BundlerMode) {
                case BundlerMode.AssetDatabase:
                    break;
                case BundlerMode.Resources:
                    break;
                case BundlerMode.AssetBundle:
                    pipeline.Add<AssetBundleLoaderGroupAsync>();
                    pipeline.Add<AssetBundleAssetLoaderAsync>();
                    break;
            }
            return pipeline;
        }
    }
}