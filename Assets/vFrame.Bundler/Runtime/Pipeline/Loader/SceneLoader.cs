// ------------------------------------------------------------
//         File: SceneLoader.cs
//        Brief: Abstract base for scene loaders; holds AssetPath and LoadSceneMode and exposes the loaded Scene.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 21:27
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    internal abstract class SceneLoader : Loader
    {
        protected SceneLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

            AssetPath = loaderContexts.AssetPath;
            LoadSceneMode = loaderContexts.SceneMode;
        }

        [JsonSerializableProperty]
        public string AssetPath { get; }

        [JsonSerializableProperty]
        protected LoadSceneMode LoadSceneMode { get; }

        public abstract UnityEngine.SceneManagement.Scene SceneObject { get; }
    }
}