// ------------------------------------------------------------
//         File: AssetBundleSceneLoaderAsync.cs
//        Brief: Concrete async scene loader for AssetBundle bundler mode; reuses the
//               RuntimeSceneLoaderAsync SceneManager.LoadSceneAsync behavior unchanged.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:29:22
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Async scene loader used in AssetBundle bundler mode.
    ///     Adds no behavior of its own; it exists as the concrete loader type registered
    ///     in the load pipeline, inheriting all scene-loading logic from
    ///     <see cref="RuntimeSceneLoaderAsync" />.
    /// </summary>
    internal class AssetBundleSceneLoaderAsync : RuntimeSceneLoaderAsync
    {
        /// <summary>
        ///     Create an async scene loader for AssetBundle mode.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts and systems.</param>
        /// <param name="loaderContexts">Per-load contexts describing the scene to load.</param>
        public AssetBundleSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}