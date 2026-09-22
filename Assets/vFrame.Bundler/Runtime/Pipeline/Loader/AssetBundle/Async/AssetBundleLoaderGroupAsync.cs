// ------------------------------------------------------------
//         File: AssetBundleLoaderGroupAsync.cs
//        Brief: AssetBundleLoaderGroup variant that creates async child bundle loaders
//               (AssetBundleLoaderAsync) for non-blocking bundle loading.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:29:18
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Concrete <see cref="AssetBundleLoaderGroup"/> whose child bundle loaders are
    ///     <see cref="AssetBundleLoaderAsync"/> instances, enabling non-blocking loads.
    /// </summary>
    internal class AssetBundleLoaderGroupAsync : AssetBundleLoaderGroup
    {
        /// <summary>
        ///     Creates the group; child loaders are created as asynchronous loaders.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler contexts (manifest, loader registry, facade systems).</param>
        /// <param name="loaderContexts">Per-loader contexts (target asset path, parent loader, etc.).</param>
        public AssetBundleLoaderGroupAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>Creates an <see cref="AssetBundleLoaderAsync"/> for the given bundle path.</summary>
        /// <param name="bundlePath">Bundle path resolved from the manifest.</param>
        /// <returns>A new, unregistered <see cref="AssetBundleLoaderAsync"/> instance.</returns>
        protected override AssetBundleLoader CreateAssetBundleLoader(string bundlePath)
        {
            return new AssetBundleLoaderAsync(BundlerContexts, LoaderContexts, bundlePath);
        }
    }
}