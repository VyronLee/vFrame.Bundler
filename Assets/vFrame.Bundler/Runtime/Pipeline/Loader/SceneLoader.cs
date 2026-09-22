// ------------------------------------------------------------
//         File: SceneLoader.cs
//        Brief: Abstract base for scene loaders; captures the asset path and load mode from LoaderContexts
//               and exposes the loaded Scene.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:56:07
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    /// Abstract base for scene loaders; captures the target asset path and load mode from
    /// <see cref="LoaderContexts"/>, while subclasses perform the load and expose it via <see cref="SceneObject"/>.
    /// </summary>
    internal abstract class SceneLoader : Loader
    {
        /// <summary>
        /// Initializes the loader with the scene asset path and load mode taken from the load contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts passed to the base loader.</param>
        /// <param name="loaderContexts">Per-load contexts supplying <see cref="AssetPath"/> and <see cref="LoadSceneMode"/>.</param>
        protected SceneLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

            AssetPath = loaderContexts.AssetPath;
            LoadSceneMode = loaderContexts.SceneMode;
        }

        /// <summary>Bundler asset path of the scene to load; included in loader JSON snapshots.</summary>
        [JsonSerializableProperty]
        public string AssetPath { get; }

        /// <summary>Mode (single or additive) with which the scene is loaded; included in loader JSON snapshots.</summary>
        [JsonSerializableProperty]
        protected LoadSceneMode LoadSceneMode { get; }

        /// <summary>The scene produced by this loader once the load has finished.</summary>
        public abstract UnityEngine.SceneManagement.Scene SceneObject { get; }
    }
}