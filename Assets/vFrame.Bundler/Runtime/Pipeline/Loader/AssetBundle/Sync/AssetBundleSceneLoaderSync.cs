// ------------------------------------------------------------
//         File: AssetBundleSceneLoaderSync.cs
//        Brief: Concrete synchronous scene loader for AssetBundle bundler mode; adds no behavior of its
//               own and inherits all scene-loading logic from RuntimeSceneLoaderSync.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:37:53
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Sync scene loader used in AssetBundle bundler mode.
    ///     Adds no behavior of its own; it exists as the concrete loader type registered
    ///     in the load pipeline, inheriting all scene-loading logic from
    ///     <see cref="RuntimeSceneLoaderSync" />.
    /// </summary>
    internal class AssetBundleSceneLoaderSync : RuntimeSceneLoaderSync
    {
        /// <summary>
        ///     Create a synchronous scene loader for AssetBundle mode.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts and systems.</param>
        /// <param name="loaderContexts">Per-load contexts describing the scene to load.</param>
        public AssetBundleSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}